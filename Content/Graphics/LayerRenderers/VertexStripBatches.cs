using Aequus.Common.Graphics;
using Aequus.Common.Graphics.LayerRenderers;
using Aequus.Common.Graphics.RenderBatches;
using Aequus.Content.Graphics.RenderBatches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace Aequus.Content.Graphics.LayerRenderers {
    public class VertexStripBatches : ILayerRenderer {        
        private record struct VertexStripInfo(Texture2D Texture, Vector2[] OldPositions, float[] OldRotations, VertexStrip.StripColorFunction ColorFunction, VertexStrip.StripHalfWidthFunction WidthFunction, Vector2 Offset = default) {
        }

        public static VertexStripBatches Instance { get; private set; }

        private bool _isReady;
        private readonly Dictionary<RenderLayerBatch, List<VertexStripInfo>> _renders = new();

        public bool IsReady => _isReady;

        public void AddStrip<T>(Texture2D Texture, Vector2[] OldPositions, float[] OldRotations, VertexStrip.StripColorFunction ColorFunction, VertexStrip.StripHalfWidthFunction WidthFunction, Vector2 Offset = default) where T : RenderLayerBatch {
            _renders[ModContent.GetInstance<T>()].Add(new(Texture, OldPositions, OldRotations, ColorFunction, WidthFunction, Offset));
            _isReady = true;
        }

        public void DrawToLayer(RenderLayerBatch layer, SpriteBatch spriteBatch) {
            if (!_renders.TryGetValue(layer, out var list) || list == null || list.Count == 0) {
                return;
            }

            foreach (var stripData in list) {
                AequusDrawing.ApplyBasicEffect(stripData.Texture);

                AequusDrawing.VertexStrip.PrepareStripWithProceduralPadding(stripData.OldPositions, stripData.OldRotations, stripData.ColorFunction, stripData.WidthFunction,
                    -Main.screenPosition + stripData.Offset, true, true);

                AequusDrawing.VertexStrip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            list.Clear();
        }

        public void SetupBatchLayers() {
            foreach (var batch in RenderLayerBatchLoader.Instance.Batches) {
                batch.Renderers.Add(this);
                _renders[batch] = new();
            }
        }

        public void Load(Mod mod) {
            Instance = this;
        }

        public void Unload() {
            _renders.Clear();
            Instance = null;
        }
    }
}