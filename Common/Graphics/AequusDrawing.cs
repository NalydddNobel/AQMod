using Aequus.Common.Effects;
using Aequus.Common.Effects.RenderBatches;
using Aequus.Content.DronePylons;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.NPCs.Monsters.BossMonsters.DustDevil;
using Aequus.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics {
    public class AequusDrawing : ModSystem {
        public static VertexStrip VertexStrip { get; private set; }

        public override void Load() {
            VertexStrip = new();

            LoadHooks();
        }

        public override void Unload() {
            VertexStrip = null;
        }

        public override void ClearWorld() {
        }

        #region Hooks
        private void LoadHooks() {
            On_Main.DrawNPCs += Main_DrawNPCs;
        }

        private static void DrawBehindTilesBehindNPCs() {
            ParticleSystem.GetLayer(ParticleLayer.BehindAllNPCs).Draw(Main.spriteBatch);
            LegacyEffects.NPCsBehindAllNPCs.renderNow = true;
            for (int i = 0; i < LegacyEffects.NPCsBehindAllNPCs.Count; i++) {
                Main.instance.DrawNPC(LegacyEffects.NPCsBehindAllNPCs[i].whoAmI, true);
            }
            LegacyEffects.NPCsBehindAllNPCs.renderNow = false;
            LegacyEffects.NPCsBehindAllNPCs.Clear();

            ModContent.GetInstance<BehindAllNPCsBatch>().FullRender(Main.spriteBatch);
            ModContent.GetInstance<BehindAllNPCsNoWorldScaleBatch>().FullRender(Main.spriteBatch);
        }

        private static void DrawBehindTilesAboveNPCs() {
            LegacyEffects.ProjsBehindTiles.renderingNow = true;
            for (int i = 0; i < LegacyEffects.ProjsBehindTiles.Count; i++) {
                Main.instance.DrawProj(LegacyEffects.ProjsBehindTiles.Index(i));
            }
            LegacyEffects.ProjsBehindTiles.Clear();
        }

        private static void DrawBehindNPCs(ref ParticleRendererSettings particleSettings) {
            GlimmerSceneEffect.DrawUltimateSword();

            foreach (var p in DustDevilParticleSystem.CachedBackParticles) {
                p.Draw(ref particleSettings, Main.spriteBatch);
            }

            DustDevil.LegacyDrawBack.renderingNow = true;
            for (int i = 0; i < DustDevil.LegacyDrawBack.Count; i++) {
                Main.instance.DrawProj(DustDevil.LegacyDrawBack.Index(i));
            }
            DustDevil.LegacyDrawBack.Clear();

            if (HealerDroneRenderer.Instance.IsReady) {
                HealerDroneRenderer.Instance.DrawOntoScreen(Main.spriteBatch);
            }
        }

        private static void DrawAboveNPCs(ref ParticleRendererSettings particleSettings) {
            ParticleSystem.GetLayer(ParticleLayer.AboveNPCs).Draw(Main.spriteBatch);
            foreach (var p in DustDevilParticleSystem.CachedFrontParticles) {
                p.Draw(ref particleSettings, Main.spriteBatch);
            }

            DustDevil.LegacyDrawFront.renderingNow = true;
            for (int i = 0; i < DustDevil.LegacyDrawFront.Count; i++) {
                Main.instance.DrawProj(DustDevil.LegacyDrawFront.Index(i));
            }
            DustDevil.LegacyDrawFront.Clear();
        }

        internal static void Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles) {
            ParticleRendererSettings particleSettings = new();
            try {
                if (!behindTiles) {
                    DrawBehindNPCs(ref particleSettings);
                }
                else {
                    DrawBehindTilesBehindNPCs();
                }
            }
            catch {
                LegacyEffects.NPCsBehindAllNPCs?.Clear();
                DustDevil.LegacyDrawBack?.Clear();
                DustDevil.LegacyDrawBack = new LegacyDrawList();
            }

            orig(self, behindTiles);

            try {
                if (!behindTiles) {
                    DrawAboveNPCs(ref particleSettings);
                }
                else {
                    DrawBehindTilesAboveNPCs();
                }
            }
            catch {
                LegacyEffects.ProjsBehindTiles?.Clear();
                LegacyEffects.ProjsBehindTiles = new LegacyDrawList();
                DustDevil.LegacyDrawFront?.Clear();
                DustDevil.LegacyDrawFront = new LegacyDrawList();
            }
        }
        #endregion
    }
}