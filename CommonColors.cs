using AQMod.Common.Utilities.Colors;
using Microsoft.Xna.Framework;

namespace AQMod
{
    public static class CommonColors
    {
        internal static Color BossMessage => new Color(175, 75, 255, 255);
        internal static Color EventMessage => new Color(50, 255, 130, 255);
        internal static Color Furniture => new Color(191, 142, 111);

        public static IColorGradient AquaticGrad = new ColorWaveGradient(4f, new Color(111, 111, 190, 255), new Color(144, 144, 255, 255));
        public static IColorGradient AtmosphericGrad = new ColorWaveGradient(4f, new Color(200, 150, 10, 255) * 0.8f, new Color(255, 230, 70, 255) * 0.8f);
        public static IColorGradient CosmicGrad = new ColorWaveGradient(4f, new Color(90, 30, 200, 255), new Color(190, 120, 255, 255));
        public static IColorGradient DemonicGrad = new ColorWaveGradient(4f, new Color(222, 100, 10, 255) * 0.8f, new Color(255, 255, 120, 255) * 0.8f);
        public static IColorGradient OrganicGrad = new ColorWaveGradient(4f, new Color(120, 255, 60, 255), new Color(180, 250, 90, 255));
        public static IColorGradient UltimateGrad = new ColorWaveGradient(8f, new Color(150, 255, 255, 255), new Color(255, 150, 255, 255));
    }
}