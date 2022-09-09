using Godot;
using System;

namespace ProceduralMapGenerator.Scripts {
	public class MapData
	{

		private float[,] Data;
		public float Min { get; private set; }
		public float Max { get; private set; }

		public MapData(int width, int height)
		{
			Data = new float[width, height];
			Min = float.MaxValue;
			Max = float.MinValue;
		}

		public void Add(int x, int y, float value)
        {
			Data[x, y] = value;
			SetMin(value);
			SetMax(value);
		}

        public float Get(int x, int y) 
		{
			return Data[x, y];
		}

		/// <summary>
		/// If the current Min value is higher than the passed 
		/// one the current value gets overwritten by the new one
		/// </summary>
		/// <param name="min"></param>
        public void SetMin(float min)
		{
			if(min < Min)
            {
				Min = min;
			}
		}

		/// <summary>
		/// If the current Max value is lower than the passed 
		/// one the current value gets overwritten by the new one
		/// </summary>
		/// <param name="max"></param>
		public void SetMax(float max)
		{
			if (max > Max)
			{
				Max = max;
			}
		}
	}
}
