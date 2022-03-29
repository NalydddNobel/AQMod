using Terraria;

namespace Aequus.Assets.Effects
{
    partial class EffectsSystem
    {
        internal void LoadHooks()
        {
            On.Terraria.Main.DrawNPCs += OnDrawNPCs;
        }

        internal static void OnDrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
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