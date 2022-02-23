using AQMod.Common.ID;
using AQMod.Common.Utilities.Debugging;
using Microsoft.Xna.Framework.Graphics;

namespace AQMod.Assets
{
    public static class LegacyTextureCache
    {
        public const string None = "Assets/empty";
        public const string Error = "Assets/error";

        public static Texture2D[] Particles { get; private set; }
        public static Texture2D[] Lights { get; private set; }
        public static Texture2D[] Trails { get; private set; }

        internal static void Load()
        {
            if (DebugUtilities.LogTextureLoading)
                DebugUtilities.SupressLogAccessMessage();
            Particles = FillArray<ParticleTex>("Particles/Particle");
            Lights = FillArray<LightTex>("Lights/Light");
            Trails = FillArray<TrailTex>("Trails/Trail");
            if (DebugUtilities.LogTextureLoading)
                DebugUtilities.RepairLogAccessMessage();
        }

        private static Texture2D[] FillArray<T>(string pathWithoutNumbers) where T : class
        {
            int count = SetConstantsIdentityAttribute.GetCount<T>();
            var t = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                t[i] = AQMod.LoggableTexture("AQMod/Assets/" + pathWithoutNumbers + "_" + i);
            }
            return t;
        }

        internal static void Unload()
        {
            Particles = null;
            Lights = null;
            Trails = null;
        }
    }
}