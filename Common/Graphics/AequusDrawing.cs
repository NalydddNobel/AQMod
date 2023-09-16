using Aequus.Common.NPCs;
using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics {
    public partial class AequusDrawing : ModSystem {
        private static BasicEffect _basicEffect;
        public static VertexStrip VertexStrip { get; private set; }

        public static readonly RasterizerState RasterizerState_BestiaryUI = new() {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public static SpriteBatchCache SpriteBatchCache { get; private set; }

        public static Matrix WorldViewPointMatrix {
            get {
                GraphicsDevice graphics = Main.graphics.GraphicsDevice;
                Vector2 screenZoom = Main.GameViewMatrix.Zoom;
                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;

                var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                    Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                    Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
                var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                return zoom * projection;
            }
        }

        private static void LoadShaders() {
            _basicEffect = new(Main.graphics.GraphicsDevice);
        }
        private static void UnloadShaders() {
            _basicEffect = null;
        }

        public override void Load() {
            if (Main.dedServ) {
                return;
            }

            SpriteBatchCache = new();
            VertexStrip = new();
            Load_Hooks();
            Main.QueueMainThreadAction(LoadShaders);
        }

        public override void Unload() {
            SpriteBatchCache = null;
            VertexStrip = null;
            Main.QueueMainThreadAction(UnloadShaders);
        }

        public override void ClearWorld() {
        }

        #region Methods
        /// <summary>
        /// Make sure to call <see cref="ApplyCurrentTechnique"/> directly after this method to have the effects render properly.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="lineSegments"></param>
        /// <param name="lineRotations"></param>
        /// <param name="getColor"></param>
        /// <param name="getWidth"></param>
        /// <param name="offset"></param>
        /// <param name="includeBacksides"></param>
        /// <param name="tryStoppingOddBug"></param>
        public static void DrawBasicVertexLine(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) {
            ApplyBasicEffect(texture);

            VertexStrip.PrepareStripWithProceduralPadding(lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug);
            VertexStrip.DrawTrail();
        }

        public static void ApplyCurrentTechnique() {
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

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
        private void Load_Hooks() {
            On_Main.DrawNPC += On_Main_DrawNPC;
            On_Main.DrawNPCs += On_Main_DrawNPCs;
            On_Main.DrawItems += On_Main_DrawItems;
        }

        private static void On_Main_DrawItems(On_Main.orig_DrawItems orig, Main main) {
            orig(main);
            ParticleSystem.GetLayer(ParticleLayer.AboveItems).Draw(Main.spriteBatch);
        }

        private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main main, bool behindTiles) {
            if (!behindTiles) {
                orig(main, behindTiles);
                ParticleSystem.GetLayer(ParticleLayer.AboveNPCs).Draw(Main.spriteBatch);
            }
            else {
                ParticleSystem.GetLayer(ParticleLayer.BehindAllNPCsBehindTiles).Draw(Main.spriteBatch);
                orig(main, behindTiles);
            }
        }

        private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main main, int iNPCIndex, bool behindTiles) {
            if (!Main.npc[iNPCIndex].TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
                return; // Risky?
            }

            aequusNPC.DrawBehindNPC(iNPCIndex, behindTiles);
            orig(main, iNPCIndex, behindTiles);
            aequusNPC.DrawAboveNPC(iNPCIndex, behindTiles);
        }
        #endregion
    }
}