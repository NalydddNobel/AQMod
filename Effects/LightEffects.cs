using Terraria;

namespace AQMod.Effects
{
    public static class LightEffects
    {
        public static void CastLightCircle(int x, int y, float r, float g, float b)
        {
            Lighting.AddLight(x, y, r, g, b);
        }
    }
}