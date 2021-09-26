using Microsoft.Xna.Framework;
using System;

namespace AQMod.Common.DeveloperTools
{
    internal class SpotlightCircle : ImageCreationFunction
    {
        public override void SetColors(ref Color[] colors, int width, int height)
        {
            int x = width / 2;
            int y = height / 2;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int index = j * width + i;
                    double dX = (i - x) / (double)x;
                    double dY = (j - y) / (double)y;
                    double c = Math.Sqrt(dX * dX + dY * dY);
                    if (c == 0)
                    {
                        colors[index] = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        float value = 1f - (float)c;
                        colors[index] = new Color(value, value, value, value);
                    }
                }
            }
        }
    }
}