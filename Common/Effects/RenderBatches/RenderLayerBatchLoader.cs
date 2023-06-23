using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Effects.RenderBatches {
    public class RenderLayerBatchLoader : ModSystem {
        public List<RenderLayerBatch> batches = new();
        public List<ILayerRenderer> renderers = new();

        public override void PostSetupContent() {
            foreach (var batch in Mod.GetContent<RenderLayerBatch>()) {
                batches.Add(batch);
            }
            foreach (var renderer in Mod.GetContent<ILayerRenderer>()) {
                renderers.Add(renderer);
                renderer.SetupBatchLayers();
            }
        }

        public override void PostUpdateDusts() {
            foreach (var list in renderers) {
                list.OnUpdate();
            }
        }

        public override void ClearWorld() {
            foreach (var list in renderers) {
                list.OnClearWorld();
            }
        }
    }
}