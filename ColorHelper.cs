using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus
{
    public class ColorHelper
    {
        internal static Color GreenSlimeColor => ContentSamples.NpcsByNetId[NPCID.GreenSlime].color;
        internal static Color BlueSlimeColor => ContentSamples.NpcsByNetId[NPCID.BlueSlime].color;

        public interface IColorGradient
        {
            Color GetColor(float time);
            void OnUnload()
            {
            }
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