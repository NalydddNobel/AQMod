using Microsoft.Xna.Framework;

namespace AQMod.Common.Utilities.Colors
{
    public struct ColorWaveGradient : IColorGradient
    {
        public ColorWaveGradient(float waveTime, params Color[] colors)
        {
            Time = waveTime;
            Colors = colors;
        }

        public readonly float Time;
        public readonly Color[] Colors;

        public Color GetColor(float time)
        {
            return ColorGradient.InternalHelperGetColor(Colors, AQUtils.Wave(time * Time, 0f, Colors.Length - 1f), 1f);
        }
    }
}
