using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.Utilities
{
    internal sealed class ImageFunctionAlphaFixer
    {
        public Texture2D sample;
        public ImageFunctionAlphaFixer(string texture)
        {
            sample = ModContent.GetTexture(texture);
        }

        public Texture2D CreateImage(int width, int height)
        {
            var graphics = Main.instance.GraphicsDevice;
            Color[] colors = new Color[width * height];
            Texture2D output = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
            SetColor(ref colors, width, height);
            output.SetData(colors);
            return output;
        }

        private void SetColor(ref Color[] colors, int width, int height)
        {
            Color[] colors2 = new Color[width * height];
            sample.GetData(colors2);
            Color[,] color3 = new Color[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    color3[i, j] = colors2[j * width + i];
                }
            }
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int index = j * width + i;
                    byte alpha = color3[i, j].A;
                    float a = alpha / 255f;
                    colors[index] = color3[i, j] * a;
                    colors[index].A = alpha;
                }
            }
        }
    }
}