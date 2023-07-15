using Aequus.Common.Effects;
using Aequus.Common.Particles;
using Aequus.Content.DronePylons;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Graphics.RenderBatches;
using Aequus.NPCs.Monsters.BossMonsters.DustDevil;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics {
    public partial class AequusDrawing : ModSystem {
        private static BasicEffect _basicEffect;
        public static VertexStrip VertexStrip { get; private set; }

        private static void LoadShaders() {
            _basicEffect = new(Main.graphics.GraphicsDevice);
        }
        private static void UnloadShaders() {
            _basicEffect = null;
        }

        public override void Load() {
            VertexStrip = new();
            Main.QueueMainThreadAction(LoadShaders);

            Load_HealthbarDrawing();
            LoadHooks();
        }

        public override void Unload() {
            VertexStrip = null;
            Main.QueueMainThreadAction(UnloadShaders);
        }

        public override void ClearWorld() {
        }

        #region Methods
        public static void GetWorldViewProjection(out Matrix view, out Matrix projection) {
            int width = Main.graphics.GraphicsDevice.Viewport.Width;
            int height = Main.graphics.GraphicsDevice.Viewport.Height;
            projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                Matrix.CreateTranslation(width / 2f, height / -2f, 0) * Matrix.CreateRotationZ(MathHelper.Pi) *
                Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
        }

        public static void ApplyBasicEffect(Texture2D texture = default, bool vertexColorsEnabled = true) {
            GetWorldViewProjection(out var view, out var projection);

            _basicEffect.VertexColorEnabled = vertexColorsEnabled;
            _basicEffect.Projection = projection;
            _basicEffect.View = view;

            if (_basicEffect.TextureEnabled = texture != null) {
                _basicEffect.Texture = texture;
            }

            foreach (var pass in _basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
            }
        }
        #endregion

        #region Hooks
        private void LoadHooks() {
            On_Main.DrawNPCs += Main_DrawNPCs;
            On_Main.DrawProjectiles += Main_DrawProjectiles;
        }

        private static void Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self) {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
            SurgeRodProj.DrawResultTexture();
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            ParticleSystem.GetLayer(ParticleLayer.BehindProjs).Draw(Main.spriteBatch);

            LegacyEffects.ProjsBehindProjs.renderingNow = true;
            for (int i = 0; i < LegacyEffects.ProjsBehindProjs.Count; i++) {
                Main.instance.DrawProj(LegacyEffects.ProjsBehindProjs.Index(i));
            }
            LegacyEffects.ProjsBehindProjs.Clear();

            Main.spriteBatch.End();
            orig(self);
            AboveAllProjectilesBatch.Instance.FullRender(Main.spriteBatch);
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