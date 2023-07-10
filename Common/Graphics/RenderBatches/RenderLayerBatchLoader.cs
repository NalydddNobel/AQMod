using Aequus.Common.Graphics.LayerRenderers;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics.RenderBatches {
    public class RenderLayerBatchLoader : ModSystem {
        public static RenderLayerBatchLoader Instance { get; private set; }

        public readonly List<RenderLayerBatch> Batches = new();
        public readonly List<ILayerRenderer> Renderers = new();

        public override void Load() {
            Instance = this;
        }

        public override void PostSetupContent() {
            foreach (var batch in Mod.GetContent<RenderLayerBatch>()) {
                Batches.Add(batch);
            }
            var renderers = Mod.GetContent<ILayerRenderer>();
            foreach (var renderer in renderers) {
                Renderers.Add(renderer);
            }
            foreach (var renderer in renderers) {
                renderer.SetupBatchLayers();
            }
        }

        public override void Unload() {
            Batches.Clear();
            Renderers.Clear();
            Instance = null;
        }

        public override void PostUpdateDusts() {
            foreach (var list in Renderers) {
                list.OnUpdate();
            }
        }

        public override void ClearWorld() {
            foreach (var list in Renderers) {
                list.OnClearWorld();
            }
        }
    }
}