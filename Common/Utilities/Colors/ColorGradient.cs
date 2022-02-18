using Microsoft.Xna.Framework;

namespace AQMod.Common.Utilities.Colors
{
    public struct ColorGradient : IColorGradient
    {
        public ColorGradient(float timeBetweenColors, params Color[] colors)
        {
            Time = timeBetweenColors;
            Colors = colors;
        }

        public readonly float Time;
        public readonly Color[] Colors;

        public Color GetColor(float time)
        {
            return InternalHelperGetColor(Colors, time, Time);
        }

        internal static Color InternalHelperGetColor(Color[] arr, float time, float timeBetweenColors)
        {
            int index = (int)(time * timeBetweenColors);
            return Color.Lerp(arr[index % arr.Length], arr[(index + 1) % arr.Length], time % 1f);
        }
    }
}