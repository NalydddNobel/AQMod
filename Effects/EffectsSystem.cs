using Aequus.Common.Configuration;
using Aequus.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Effects
{
    public sealed partial class EffectsSystem : ModSystem
    {
        public static MiniRandom EffectRand { get; private set; }

        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }
        public static DrawIndexCache ProjsBehindTiles { get; private set; }

        public static ScreenShakeData Shake { get; private set; }

        public override void Load()
        {
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
            Shake = new ScreenShakeData();
            LoadHooks();
        }
        private void LoadHooks()
        {
            On.Terraria.Main.DrawNPCs += Hook_OnDrawNPCs;
        }

        public override void Unload()
        {
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles = null;
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Shake.Update();
            }
        }

        internal static void UpdateScreenPosition()
        {
            Main.screenPosition += Shake.GetScreenOffset() * ClientConfiguration.Instance.ScreenshakeIntensity;
        }
    }
}