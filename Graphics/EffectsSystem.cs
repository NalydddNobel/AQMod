using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed partial class EffectsSystem : ModSystem
    {
        public static NecromancyScreenRenderer[] necromancyRenderers;

        public static MiniRandom EffectRand { get; private set; }

        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }
        public static ParticleRenderer BehindProjs { get; private set; }
        public static DrawIndexCache ProjsBehindTiles { get; private set; }

        public static ScreenShake Shake { get; private set; }

        public override void Load()
        {
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
            Shake = new ScreenShake();
            EffectRand = new MiniRandom("Split".GetHashCode(), capacity: 256 * 4);
            BehindProjs = new ParticleRenderer();
            necromancyRenderers = new NecromancyScreenRenderer[]
            { 
                new NecromancyScreenRenderer(0, NecromancyScreenRenderer.TargetIDs.LocalPlayer, () => Color.White),
                new NecromancyScreenRenderer(-1, NecromancyScreenRenderer.TargetIDs.FriendlyZombie, () => new Color(100, 149, 237, 255)),
                new NecromancyScreenRenderer(-1, NecromancyScreenRenderer.TargetIDs.FriendlyRevenant, () => new Color(40, 100, 237, 255)),
                new NecromancyScreenRenderer(-1, NecromancyScreenRenderer.TargetIDs.FriendlyOsiris, () => new Color(255, 128, 20, 255)),
                new NecromancyScreenRenderer(-1, NecromancyScreenRenderer.TargetIDs.FriendlyInsurgent, () => new Color(80, 255, 200, 255)),
                new NecromancyScreenRenderer(-1, NecromancyScreenRenderer.TargetIDs.FriendlyBloodSacrifice, () => new Color(255, 10, 10, 255)),
            };
            LoadHooks();
        }
        private void LoadHooks()
        {
            On.Terraria.Main.DoDraw_UpdateCameraPosition += Main_DoDraw_UpdateCameraPosition;
            On.Terraria.Main.DrawDust += Hook_OnDrawDust;
            On.Terraria.Main.DrawProjectiles += Hook_OnDrawProjs;
            On.Terraria.Main.DrawNPCs += Hook_OnDrawNPCs;
        }

        public override void Unload()
        {
            necromancyRenderers = null;
            BehindProjs = null;
            Shake = null;
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles = null;
        }

        public override void OnWorldLoad()
        {
            BehindProjs = new ParticleRenderer();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                FrozenNPCEffect.ResetCounts();

                Shake.Update();
                BehindProjs.Update();
            }
        }

        internal static void UpdateScreenPosition()
        {
            Main.screenPosition += Shake.GetScreenOffset() * ClientConfig.Instance.ScreenshakeIntensity;
        }
    }
}