using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AQMod.Common.DeveloperTools
{
    internal class Aura : ImageCreationFunction
    {
        public Texture2D sample;
        public int maxLength;
        public Vector3 tint;

        public Aura(Texture2D texture, int maxLength = 4)
        {
            sample = texture;
            this.maxLength = maxLength;
            tint = new Vector3(1f, 1f, 1f);
        }

        public Aura(Texture2D texture, Vector3 tint, int maxLength = 4)
        {
            sample = texture;
            this.maxLength = maxLength;
            this.tint = tint;
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
                    if (color3[i, j].A != 0)
                    {
                        colors[index] = color3[i, j];
                        continue;
                    }
                    if (i < maxLength || i > width - maxLength - 1 || j < maxLength || j > height - maxLength - 1)
                    {
                        colors[index] = new Color(0f, 0f, 0f, 0f);
                        continue;
                    }
                    bool any = false;
                    double minLength = maxLength;
                    for (int k = 0; k < maxLength * 2; k++)
                    {
                        for (int l = 0; l < maxLength * 2; l++)
                        {
                            int i2 = i - maxLength + k;
                            int j2 = j - maxLength + l;
                            if (color3[i2, j2].A != 0)
                            {
                                double x2 = i - i2;
                                double y2 = j - j2;
                                double length = Math.Sqrt((x2 * x2) + (y2 * y2));
                                if (length < minLength)
                                {
                                    any = true;
                                    minLength = length;
                                }
                            }
                        }
                    }
                    if (!any)
                    {
                        colors[index] = new Color(0f, 0f, 0f, 0f);
                        continue;
                    }
                    double c = minLength / maxLength;
                    if (c == 0)
                    {
                        colors[index] = new Color(1f * tint.X, 1f * tint.Y, 1f * tint.Z, 1f);
                    }
                    else
                    {
                        float value = 1f - (float)c;
                        colors[index] = new Color(value * tint.X, value * tint.Y, value * tint.Z, value);
                    }
                }
            }
        }
    }
}