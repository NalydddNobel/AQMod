using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Assets.Effects
{
    public sealed partial class EffectsSystem : ModSystem
    {
        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }
        public static DrawIndexCache ProjsBehindTiles { get; private set; }

        public override void OnWorldLoad()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                GameCamera.Instance.Initialize();
                GameEffects.Instance.Initialize();
            }
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                GameEffects.Instance.UpdateFilters();
            }
        }

        public override void Load()
        {
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
            LoadHooks();
        }

        public override void Unload()
        {
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles = null;
        }
    }
}