using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralMapGenerator.Scripts
{
    public class HeatType
    {
        public static readonly HeatType[] TYPES = {
            new HeatType() { Name="Coldest", Value=0.05f, Color=new Color(0,1,1,1)}
           ,new HeatType() { Name="Colder", Value=0.18f, Color=new Color(170/255f, 1, 1, 1)}
           ,new HeatType() { Name="Cold", Value=0.4f, Color=new Color(0, 229/255f, 133/255f, 1)}
           ,new HeatType() { Name="Warm", Value=0.6f, Color=new Color(1, 1, 100/255f, 1)}
           ,new HeatType() { Name="Warmer", Value=0.8f, Color=new Color(1, 100/255f, 0, 1)}
           ,new HeatType() { Name="Warmest", Value=float.MaxValue, Color=new Color(241/255f, 12/255f, 0, 1)}};
  
        /// <summary>
        /// The Name of the HeatType.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The max value for this HeatType.
        /// </summary>
        public float Value { get; set; }
        public Color Color { get; set; }

        public static HeatType GetHeatType(float value)
        {
            foreach (var item in TYPES)
            {
                if(value <= item.Value)
                {
                    return item;
                }
            }
            return TYPES[TYPES.Length-1];
        }
    }
}
