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

        public static ParticleRenderer BehindProjs { get; private set; }
        /// <summary>
        /// Use this instead of <see cref="Main.ParticleSystem_World_BehindPlayers"/>. Due to it not refreshing old modded particles when you build+reload
        /// </summary>
        public static ParticleRenderer BehindPlayers { get; private set; }
        /// <summary>
        /// Use this instead of <see cref="Main.ParticleSystem_World_OverPlayers"/>. Due to it not refreshing old modded particles when you build+reload
        /// </summary>
        public static ParticleRenderer AbovePlayers { get; private set; }

        public static DrawIndexCache ProjsBehindTiles { get; private set; }
        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }

        public static ScreenShake Shake { get; private set; }

        public override void Load()
        {
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
            Shake = new ScreenShake();
            EffectRand = new MiniRandom("Split".GetHashCode(), capacity: 256 * 4);
            BehindProjs = new ParticleRenderer();
            BehindPlayers = new ParticleRenderer();
            AbovePlayers = new ParticleRenderer();
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
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
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
            if (Main.dedServ)
            {
                return;
            }
            BehindProjs = new ParticleRenderer();
            BehindPlayers = new ParticleRenderer();
            AbovePlayers = new ParticleRenderer();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                FrozenNPCEffect.ResetCounts();

                Shake.Update();
                BehindProjs.Update();
                BehindPlayers.Update();
                AbovePlayers.Update();
            }
        }

        internal static void UpdateScreenPosition()
        {
            Main.screenPosition += Shake.GetScreenOffset() * ClientConfig.Instance.ScreenshakeIntensity;
        }
    }
}