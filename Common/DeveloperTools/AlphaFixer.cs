using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AQMod.Common.DeveloperTools
{
    internal class AlphaFixer : ImageCreationFunction
    {
        public Texture2D sample;
        public AlphaFixer(string texture)
        {
            sample = ModContent.GetTexture(texture);
        }

        public override void SetColors(ref Color[] colors, int width, int height)
        {
            Color[] colors2 = new Color[width * height];
            sample.GetData(colors2);
            Color[,] color3 = new Color[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    color3[i, j] = colors2[(j * width) + i];
                }
            }
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int index = (j * width) + i;
                    byte alpha = color3[i, j].A;
                    float a = alpha / 255f;
                    colors[index] = color3[i, j] * a;
                    colors[index].A = alpha;
                }
            }
        }
    }
}