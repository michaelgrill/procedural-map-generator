using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalNoise;
using Godot;

namespace ProceduralMapGenerator.Scripts
{
    [Tool]
    public abstract class Generator : Node
    {
        // Adjustable variables for Unity Inspector
        [Export]
        public int Width = 512;
        [Export]
        public int Height = 512;
        [Export]
        public int TerrainOctaves = 6;
        [Export]
        public double TerrainFrequency = 1.25;

        // Height map data
        protected MapData HeightData;
        protected MapData HeatData;

        // Final Objects
        protected Tile[,] Tiles;

        // TEST
        [Export]
        public float ZOOM = 1f;

        protected readonly int _seed = 1;

        public override void _Ready()
        {
            // Get the mesh we are rendering our output to
            // Unity: HeightMapRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
            //HeightMapRenderer = GetNode<MeshInstance>("./HeightTexture");
            //var sprite = GetNode<MeshInstance>("./MeshInstance");

            // Initialize the generator
            Initialize();

            // Build the height map
            GetData();

            // Build our final objects based on our data
            LoadTiles();

            UpdateNeighbors();

            // TODO Render a 3D texture representation of our map AND move to TextureGenerator
            //HeightMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(Width, Height, Tiles);
            //HeightMapRenderer.
            /* 3D
            var st = new SurfaceTool();
            st.Begin(Mesh.PrimitiveType.TriangleFan);

            var mat = new SpatialMaterial();
            st.SetMaterial(mat);
            foreach (var item in Tiles)
            {
                var color = new Color(0, 0, 0);
                color = color.LinearInterpolate(new Color(1, 1, 1), item.HeightValue);
                st.AddColor(color);
                st.AddVertex(new Vector3(item.X, item.HeightValue, item.Y));
            }
            var tmpMesh = st.Commit();
            HeightMapRenderer.Mesh = tmpMesh;*/
            AfterReady();
        }

        protected virtual void AfterReady() { }

        protected abstract void Initialize();
        protected abstract void GetData();

        private void UpdateNeighbors()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Tile t = Tiles[x, y];

                    t.Top = Tiles[t.X, MathHelper.Mod(t.Y - 1, Height)];
                    t.Bottom = Tiles[t.X, MathHelper.Mod(t.Y + 1, Height)];
                    t.Left = Tiles[MathHelper.Mod(t.X - 1, Width), t.Y];
                    t.Right = Tiles[MathHelper.Mod(t.X + 1, Width), t.Y];
                }
            }
        }


        // Build a Tile array from our data
        private void LoadTiles()
        {
            Tiles = new Tile[Width, Height];

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Tile t = new Tile();
                    t.X = x;
                    t.Y = y;

                    float heightValue = HeightData.Get(x, y);
                    heightValue = (heightValue - HeightData.Min) / (HeightData.Max - HeightData.Min);
                    t.HeightValue = heightValue;


                    // Set heat value
                    float heatValue = HeatData.Get(x, y);
                    heatValue = (heatValue - HeatData.Min) / (HeatData.Max - HeatData.Min);
                    t.BaseHeatValue = heatValue;

                    /* set heat type
                    if (heatValue < ColdestValue) t.HeatType = HeatType.Coldest;
                    else if (heatValue < ColderValue) t.HeatType = HeatType.Colder;
                    else if (heatValue < ColdValue) t.HeatType = HeatType.Cold;
                    else if (heatValue < WarmValue) t.HeatType = HeatType.Warm;
                    else if (heatValue < WarmerValue) t.HeatType = HeatType.Warmer;
                    else t.HeatType = HeatType.Warmest;
                    */
                    Tiles[x, y] = t;
                }
            }
        }

    }
}
