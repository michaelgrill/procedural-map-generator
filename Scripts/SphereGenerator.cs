using AccidentalNoise;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralMapGenerator.Scripts
{
    public class SphereGenerator : Generator
    {
        // Heat variables
        private readonly int HeatOctaves = 4;
        private readonly double HeatFrequency = 3.0;

        // Moisture variables
        private readonly int MoistureOctaves = 4;
        private readonly double MoistureFrequency = 3.0;
        private readonly float DryerValue = 0.27f;
        private readonly float DryValue = 0.4f;
        private readonly float WetValue = 0.6f;
        private readonly float WetterValue = 0.8f;
        private readonly float WettestValue = 0.9f;

        private ImplicitFractal HeightMap;
        private ImplicitCombiner HeatMap;


        // Our texture output (unity component)
        // MeshRenderer HeightMapRenderer;

        // Our texture output (godot component)
        private MeshInstance HeightMapRenderer;


        public SphereGenerator() : base()
        {
            
        }

        protected override void AfterReady()
        {
            base.AfterReady();

            // Generate and Display 2D Texture
            var sprite = GetNode<Sprite>("./Sprite");
            var texture = TextureGenerator.GetTexture(Width, Height, Tiles);
            //var texture = TextureGenerator.GetHeatMapTexture(Width, Height, Tiles);
            sprite.Texture = texture;
        }

        protected override void Initialize()
        {
            // Height Map
            HeightMap = new ImplicitFractal(FractalType.MULTI,
                                           BasisType.SIMPLEX,
                                           InterpolationType.QUINTIC,
                                           TerrainOctaves,
                                           TerrainFrequency,
                                           _seed);

            // Heat Map
            ImplicitGradient gradient = new ImplicitGradient(1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1);
            ImplicitFractal heatFractal = new ImplicitFractal(FractalType.MULTI,
                                                              BasisType.SIMPLEX,
                                                              InterpolationType.QUINTIC,
                                                              HeatOctaves,
                                                              HeatFrequency,
                                                              _seed);

            HeatMap = new ImplicitCombiner(CombinerType.MULTIPLY);
            HeatMap.AddSource(gradient);
            HeatMap.AddSource(heatFractal);

            // TODO Moisture Map
        }


        // Extract data from a noise module
        protected override void GetData()
        {
            HeightData = new MapData(Width, Height);
            HeatData = new MapData(Width, Height);

            // Define our map area in latitude/longitude
            float southLatBound = -180;
            float northLatBound = 180;
            float westLonBound = -90;
            float eastLonBound = 90;

            float lonExtent = eastLonBound - westLonBound;
            float latExtent = northLatBound - southLatBound;

            float xDelta = lonExtent / (float)Width;
            float yDelta = latExtent / (float)Height;

            float curLon = westLonBound;
            float curLat = southLatBound;
            // loop through each x,y point - get height value
            for (var x = 0; x < Width; x++)
            {

                curLon = westLonBound;
                for (var y = 0; y < Height; y++)
                {
                    float x1 = 0, y1 = 0, z1 = 0;
                    //LatLonToXYZ(x, y, ref x1, ref y1, ref z1, ref w1);
                    LatLonToXYZ(curLat, curLon, ref x1, ref y1, ref z1);

                    // Height Data
                    float heightValue = (float)HeightMap.Get(x1, y1, z1);
                    HeightData.Add(x, y, heightValue);

                    // Heat Data
                    float sphereValue = (float)HeatMap.Get(x1, y1, z1);
                    HeatData.Add(x, y, sphereValue);

                    /*
                    float coldness = Math.Abs(curLon) / 90f;
                    float heat = 1 - Math.Abs(curLon) / 90f;
                    HeatData.Data[x, y] += heat;
                    HeatData.Data[x, y] -= coldness;
                    
                    float coldness = Math.Abs(y) / 90f;
                    float heat = 1 - Math.Abs(y) / 90f;
                    HeatData.Data[x, y] += heat;
                    HeatData.Data[x, y] -= coldness;
*/
                    curLon += xDelta;
                }
                curLat += yDelta;
            }
        }

        private void LatLonToXYZ(float lat, float lon, ref float x, ref float y, ref float z)
        {

            /* No Wrapping 
            //Sample the noise at smaller intervals
            float x1 = x / (float)Width;
            float y1 = y / (float)Height;

            // Unity: float value = (float)HeightMap.Get(x1, y1);
            //float value = (float)HeightMap.Perlin(x1 * ZOOM, y1 * ZOOM);
            float value = (float)module.Get(x1 * ZOOM, y1 * ZOOM);

            //keep track of the max and min values found
            if (value > mapData.Max) mapData.Max = value;
            if (value < mapData.Min) mapData.Min = value;

            mapData.Data[x, y] = value;
            */

            /*/ Wrapping the Map on One Axis 
            //Noise range
            float x1 = 0, x2 = 1;
            float y1 = 0, y2 = 1;
            float dx = x2 - x1;
            float dy = y2 - y1;

            //Sample noise at smaller intervals
            float s = x / (float)Width;
            float t = y / (float)Height;

            // Calculate our 3D coordinates
            float nx = (float)(x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
            float ny = (float)(x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI));
            float nz = t;

            float heightValue = (float)HeightMap.Get(nx, ny, nz);

            // keep track of the max and min values found
            if (heightValue > mapData.Max)
                mapData.Max = heightValue;
            if (heightValue < mapData.Min)
                mapData.Min = heightValue;

            mapData.Data[x, y] = heightValue;
            

            // Wrapping the Map on Both Axis
            // Noise range
            float x1 = 0, x2 = 2;
            float y1 = 0, y2 = 2;
            float dx = x2 - x1;
            float dy = y2 - y1;

            // Sample noise at smaller intervals
            float s = realX / (float)Width;
            float t = realY / (float)Height;

            // Calculate our 4D coordinates
            float nx = (float)(x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
            float ny = (float)(y1 + Math.Cos(t * 2 * Math.PI) * dy / (2 * Math.PI));
            float nz = (float)(x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI));
            float nw = (float)(y1 + Math.Sin(t * 2 * Math.PI) * dy / (2 * Math.PI));

            x = nx * ZOOM;
            y = ny * ZOOM;
            z = nz * ZOOM;
            w = nw * ZOOM;, ref float w
            */

            var deg2Rad = Math.PI / 180; // 0,0174533
            float r = (float)Math.Cos(deg2Rad * lon);
            x = (float)(r * Math.Cos(deg2Rad * lat));
            y = (float)Math.Sin(deg2Rad * lon);
            z = (float)(r * Math.Sin(deg2Rad * lat));
        }



    }
}
