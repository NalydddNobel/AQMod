using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Common.Utilities
{
    public sealed class Gradients : ILoadable
    {
        public static IColorGradient aquaticGrad;
        public static IColorGradient atmosphericGrad;
        public static IColorGradient cosmicGrad;
        public static IColorGradient demonicGrad;
        public static IColorGradient organicGrad;
        public static IColorGradient ultimateGrad;
        public static IColorGradient nalydGradient;

        void ILoadable.Load(Mod mod)
        {
            aquaticGrad = new ColorWaveGradient(4f, new Color(111, 111, 190, 0), new Color(144, 144, 255, 0));
            atmosphericGrad = new ColorWaveGradient(4f, new Color(200, 150, 10, 0) * 0.8f, new Color(255, 230, 70, 0) * 0.8f);
            cosmicGrad = new ColorWaveGradient(4f, new Color(90, 30, 200, 0), new Color(190, 120, 255, 0));
            demonicGrad = new ColorWaveGradient(4f, new Color(222, 100, 10, 0) * 0.8f, new Color(255, 255, 120, 0) * 0.8f);
            organicGrad = new ColorWaveGradient(4f, new Color(120, 255, 60, 0), new Color(180, 250, 90, 0));
            ultimateGrad = new ColorWaveGradient(8f, new Color(150, 255, 255, 0), new Color(255, 150, 255, 0));
            nalydGradient = new ColorWaveGradient(10f, Color.Violet, Color.MediumPurple);
        }

        void ILoadable.Unload()
        {
            aquaticGrad = null;
            atmosphericGrad = null;
            cosmicGrad = null;
            demonicGrad = null;
            organicGrad = null;
            ultimateGrad = null;
        }

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
                return ColorGradient.InternalHelperGetColor(Colors, AequusHelpers.Wave(time * Time, 0f, Colors.Length - 1f), 1f);
            }
        }
    }
}