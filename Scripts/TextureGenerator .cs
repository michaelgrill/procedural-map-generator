using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace ProceduralMapGenerator.Scripts
{
    public static class TextureGenerator
    {
        public static Texture GetTexture(int width, int height, Tile[,] tiles)
        {
            var texture = new ImageTexture();// = new Texture2D(width, height);
            texture.Create(width, height, Image.Format.Rgb8);
            var pixels = new Color[width * height];

            var image = new Image();
            image.Create(width, height, false, Image.Format.Rgb8);
            image.Lock();
            //texture.SetData(image);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = tiles[x, y].HeightValue;

                    //Set color range, 0 = black, 1 = white
                    //var color = Color.Lerp(Color.black, Color.white, value);
                    var color = new Color(0,0,0);
                    color = color.LinearInterpolate(new Color(1, 1, 1), value);
                    pixels[x + y * width] = color;
                    //texture.GetData().SetPixel(x, y, color);
                    image.SetPixel(x, y, color);
                }
            }
            //texture.SetPixels(pixels);
            //texture.wrapMode = TextureWrapMode.Clamp;
            image.SavePng("res://test_output.png");
            image.Unlock();
            texture.SetData(image);
            return texture;
        }

    }
}
