using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace ProceduralMapGenerator.Scripts
{
    [Tool]
    public class Generator : Node
    {
        // Adjustable variables for Unity Inspector
        [Export]
        int Width = 256;
        [Export]
        int Height = 256;
        [Export]
        int TerrainOctaves = 6;
        [Export]
        double TerrainFrequency = 1.25;

        // Noise generator module
        // Unity: ImplicitFractal HeightMap;
        Noise HeightMap;

        // Height map data
        MapData HeightData;

        // Final Objects
        Tile[,] Tiles;

        // Our texture output (unity component)
        // MeshRenderer HeightMapRenderer;

        // Our texture output (godot component)
        MeshInstance HeightMapRenderer;

        // TEST
        [Export]
        float ZOOM = 8f;

        public override void _Ready()
        {
            // Get the mesh we are rendering our output to
            // Unity: HeightMapRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
            //HeightMapRenderer = GetNode<MeshInstance>("./HeightTexture");
            //var sprite = GetNode<MeshInstance>("./MeshInstance");

            // Initialize the generator
            Initialize();

            // Build the height map
            GetData(HeightMap, ref HeightData);

            // Build our final objects based on our data
            LoadTiles();

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
            // Generate and Display 2D Texture
            var sprite = GetNode<Sprite>("./Sprite");
            var texture = TextureGenerator.GetTexture(Width, Height, Tiles);
            sprite.Texture = texture;
        }

        private void Initialize()
        {
            // Initialize the HeightMap Generator
            /* Unity: 
            HeightMap = new ImplicitFractal(FractalType.MULTI,
                                           BasisType.SIMPLEX,
                                           InterpolationType.QUINTIC,
                                           TerrainOctaves,
                                           TerrainFrequency,
                                           UnityEngine.Random.Range(0, int.MaxValue));
            */
            HeightMap = new Noise(7543453);
        }

        // Extract data from a noise module
        private void GetData(Noise noise, ref MapData mapData)
        {
            mapData = new MapData(Width, Height);

            // loop through each x,y point - get height value
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    //Sample the noise at smaller intervals
                    float x1 = x / (float)Width;
                    float y1 = y / (float)Height;

                    // Unity: float value = (float)HeightMap.Get(x1, y1);
                    float value = (float)HeightMap.Perlin(x1 * ZOOM, y1 * ZOOM);

                    //keep track of the max and min values found
                    if (value > mapData.Max) mapData.Max = value;
                    if (value < mapData.Min) mapData.Min = value;

                    mapData.Data[x, y] = value;
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

                    float value = HeightData.Data[x, y];

                    //normalize our value between 0 and 1
                    value = (value - HeightData.Min) / (HeightData.Max - HeightData.Min);

                    t.HeightValue = value;

                    Tiles[x, y] = t;
                }
            }
        }
    }
}
