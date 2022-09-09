using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using static Godot.Texture;

namespace ProceduralMapGenerator.Scripts
{
    public static class TextureGenerator
    {
        private static int _count = 0;




        public static Texture GetTexture(int width, int height, Tile[,] tiles, int resolutionMultiplicator = 1)
        {
            var texture = new ImageTexture();// = new Texture2D(width, height);
            texture.Create(width * resolutionMultiplicator
                , height * resolutionMultiplicator, Image.Format.Rgb8);

            // for no interpolation
            texture.Flags = (uint)FlagsEnum.Mipmaps;

            var pixels = new Color[width * height];

            var image = new Image();
            image.Create(width * resolutionMultiplicator
                , height * resolutionMultiplicator
                , false, Image.Format.Rgb8);

            

            image.Lock();
            //texture.SetData(image);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Tile tile = tiles[x, y];
                    var thisLayer = tile.Layer;
                    var thisColor = thisLayer.Color;
                    //Set color range, 0 = black, 1 = white
                    //var color = Color.Lerp(Color.black, Color.white, value);
                    //var color = PredefindColors.white.LinearInterpolate(PredefindColors.black, tile.HeightValue);
                    var color = thisColor;
                    var layer = thisLayer;

                    //darken the color if a edge tile
                    if (tiles[x, y].Bitmask != 15)
                    {
                        color = color.LinearInterpolate(PredefindColors.black, 0.4f);
                    }

                    if (resolutionMultiplicator > 1)
                    {
                        FillTop(tile, resolutionMultiplicator, x, y, image);
                        FillBottom(tile, resolutionMultiplicator, x, y, image);
                        FillLeft(tile, resolutionMultiplicator, x, y, image);
                        FillRight(tile, resolutionMultiplicator, x, y, image);
                    }

                    if (resolutionMultiplicator % 2 != 0)
                    {
                        color = tile.Layer.Color;
                        if (tiles[x, y].Bitmask != 15)
                        {
                            color = color.LinearInterpolate(PredefindColors.black, 0.4f);
                        }
                        int centerX = x * resolutionMultiplicator + resolutionMultiplicator / 2;
                        int centerY = y * resolutionMultiplicator + resolutionMultiplicator / 2;
                        image.SetPixel(centerX, centerY, color);
                    }

                    pixels[x + y * width] = color;
                    //texture.GetData().SetPixel(x, y, color);
                    //image.SetPixel(x, y, color);
                }
            }
            //texture.SetPixels(pixels);
            //texture.wrapMode = TextureWrapMode.Clamp;
            image.SavePng($"res://output_{_count++}.png");
            image.Unlock();

            //image.GenerateMipmaps(false);
            

            texture.SetData(image);
            return texture;
        }

        
        public static Texture GetHeatMapTexture(int width, int height, Tile[,] tiles)
        {
            int resolutionMultiplicator = 1;
            var texture = new ImageTexture();
            texture.Create(width * resolutionMultiplicator
                , height * resolutionMultiplicator, Image.Format.Rgb8);

            // for no interpolation
            texture.Flags = (uint)FlagsEnum.Mipmaps;


            var image = new Image();
            image.Create(width * resolutionMultiplicator
                , height * resolutionMultiplicator
                , false, Image.Format.Rgb8);

            image.Lock();
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var tile = tiles[x, y];
                    // red <-> blue
                    var color = PredefindColors.blue.LinearInterpolate(PredefindColors.red, tile.HeatValue);
                   
                    // red <-> yellow <-> green <-> blue
                    color = HeatType.GetHeatType(tile.HeatValue).Color;

                    //darken the color if a edge tile
                    if (tiles[x, y].Bitmask != 15)
                    {
                        color = color.LinearInterpolate(PredefindColors.black, 0.4f);
                    }

                    image.SetPixel(x, y, color);
                }
            }

            image.SavePng($"res://output_heatmap_{_count++}.png");
            image.Unlock();

            texture.SetData(image);
            return texture;
        }
        
        private static void FillTop(Tile tile, int resolutionMultiplicator, int x, int y, Image image)
        {
            // top
            var layer = tile.Top.Layer;
            var color = layer.Color;
            if (layer.MaxHeight < tile.Layer.MaxHeight)
            {
                color = layer.Color;
            }
            if ((tile.Bitmask & 1) != 1)
            {
                color = color.LinearInterpolate(PredefindColors.black, 0.4f);
            }
            for (int down = 0; down < resolutionMultiplicator / 2; down++)
            {
                for (int side = 0 + down; side < resolutionMultiplicator - 1 - down; side++)
                {
                    var startX = resolutionMultiplicator * x + side;
                    var startY = resolutionMultiplicator * y + down;
                    image.SetPixel(startX, startY, color);
                }
            }
        }
        private static void FillBottom(Tile tile, int resolutionMultiplicator, int x, int y, Image image)
        {
            var layer = tile.Bottom.Layer;
            var color = layer.Color;
            if (layer.MaxHeight < tile.Layer.MaxHeight)
            {
                color = layer.Color;
            }
            if ((tile.Bitmask & 4) != 4)
            {
                color = color.LinearInterpolate(PredefindColors.black, 0.4f);
            }
            for (int down = 0; down < resolutionMultiplicator / 2 + 1; down++)
            {
                for (int side = 1 + down; side < resolutionMultiplicator - down; side++)
                {
                    var startX = resolutionMultiplicator * x + side;
                    var startY = resolutionMultiplicator * y - down + resolutionMultiplicator - 1;
                    image.SetPixel(startX, startY, color);
                }
            }
        }
        private static void FillLeft(Tile tile, int resolutionMultiplicator, int x, int y, Image image)
        {
            var layer = tile.Left.Layer;
            var color = layer.Color;
            if (layer.MaxHeight < tile.Layer.MaxHeight)
            {
                color = layer.Color;
            }
            if ((tile.Bitmask & 8) != 8)
            {
                color = color.LinearInterpolate(PredefindColors.black, 0.4f);
            }
            for (int side = 0; side < resolutionMultiplicator / 2; side++)
            {
                for (int down = 1 + side; down < resolutionMultiplicator - side; down++)
                {
                    var startX = resolutionMultiplicator * x + side;
                    var startY = resolutionMultiplicator * y + down;
                    image.SetPixel(startX, startY, color);
                }
            }

        }
        private static void FillRight(Tile tile, int resolutionMultiplicator, int x, int y, Image image)
        {
            var layer = tile.Right.Layer;
            var color = layer.Color;
            if (layer.MaxHeight < tile.Layer.MaxHeight)
            {
                color = layer.Color;
            }
            if ((tile.Bitmask & 2) != 2)
            {
                color = color.LinearInterpolate(PredefindColors.black, 0.4f);
            }
            for (int side = 0; side < resolutionMultiplicator / 2; side++)
            {
                for (int down = 0 + side; down < resolutionMultiplicator - side - 1; down++)
                {
                    var startX = resolutionMultiplicator * x - side + resolutionMultiplicator - 1;
                    var startY = resolutionMultiplicator * y + down;
                    image.SetPixel(startX, startY, color);
                }
            }
        }
    }
}
