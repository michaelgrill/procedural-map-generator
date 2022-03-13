using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralMapGenerator.Scripts
{
    public class Tile
    {
        public static readonly Layer[] LAYER = { 
            new Layer() { Name="DeepWater",    MaxHeight=0.2f, Color=PredefindColors.darkblue }
           ,new Layer() { Name="ShallowWater", MaxHeight=0.4f, Color=PredefindColors.blue }
           ,new Layer() { Name="Sand",         MaxHeight=0.5f, Color=PredefindColors.yellow }
           ,new Layer() { Name="Grass",        MaxHeight=0.7f, Color=PredefindColors.green }
           ,new Layer() { Name="Forest",       MaxHeight=0.8f, Color=PredefindColors.darkgreen }
           ,new Layer() { Name="Rock",         MaxHeight=0.9f, Color=PredefindColors.gray }
           ,new Layer() { Name="Snow",         MaxHeight=1.0f, Color=PredefindColors.white }};


        public float HeightValue { get; set; }
        public int X, Y;

        public Tile Left;
        public Tile Right;
        public Tile Top;
        public Tile Bottom;

        public Layer Layer
        {
            get
            {
                foreach (var item in LAYER)
                {
                    if(HeightValue <= item.MaxHeight)
                    {
                        return item;
                    }
                };
                return LAYER[LAYER.Length];
            }
        }

        public uint Bitmask
        {
            get
            {
                uint count = 0;
                var layer = Layer;
                if (Top.Layer == layer)
                    count += 1;
                if (Right.Layer == layer)
                    count += 2;
                if (Bottom.Layer == layer)
                    count += 4;
                if (Left.Layer == layer)
                    count += 8;

                return count;
            }
        }

        public Tile()
        {
        }

        

    }
    public class Layer
    {
        public string Name { get; set; }
        public float MaxHeight { get; set; }

        public Color Color { get; set; }
    }
}
