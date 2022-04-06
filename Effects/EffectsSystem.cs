using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Effects
{
    public sealed class EffectsSystem : ModSystem
    {
        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }
        public static DrawIndexCache ProjsBehindTiles { get; private set; }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ModContent.GetInstance<ModEffects>().UpdateFilters();
            }
        }

        public override void Load()
        {
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
            ApplyHooks();
        }
        internal void ApplyHooks()
        {
            On.Terraria.Main.DrawNPCs += Hook_OnDrawNPCs;
        }

        public override void Unload()
        {
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles = null;
        }

        internal static void Hook_OnDrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            try
            {
                NPCsBehindAllNPCs.renderingNow = true;
                for (int i = 0; i < NPCsBehindAllNPCs.Count; i++)
                {
                    Main.instance.DrawNPC(NPCsBehindAllNPCs.Index(i), behindTiles);
                }
                NPCsBehindAllNPCs.Clear();
            }
            catch
            {
                NPCsBehindAllNPCs?.Clear();
                NPCsBehindAllNPCs = new DrawIndexCache();
            }
            //if (!behindTiles)
            //{
            //    GoreNestRenderer.Render();
            //    UltimateSwordRenderer.Render();
            //}
            orig(self, behindTiles);
            if (behindTiles)
            {
                try
                {
                    ProjsBehindTiles.renderingNow = true;
                    if (ProjsBehindTiles != null)
                    {
                        for (int i = 0; i < ProjsBehindTiles.Count; i++)
                        {
                            Main.instance.DrawProj(ProjsBehindTiles.Index(i));
                        }
                    }
                    ProjsBehindTiles.Clear();
                }
                catch
                {
                    ProjsBehindTiles?.Clear();
                    ProjsBehindTiles = new DrawIndexCache();
                }
            }
        }
    }
}