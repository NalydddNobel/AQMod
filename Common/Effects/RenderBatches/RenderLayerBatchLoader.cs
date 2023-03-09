using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Effects.RenderBatches
{
    public class RenderLayerBatchLoader : IPostSetupContent
    {
        public List<RenderLayerBatch> batches = new();

        public virtual void Load(Mod mod)
        {
        }

        public void PostSetupContent(Aequus aequus)
        {
            foreach (var batch in aequus.GetContent<RenderLayerBatch>())
            {
                batches.Add(batch);
            }
            foreach (var renderer in aequus.GetContent<ILayerRenderer>())
            {
                renderer.SetupBatchLayers();
            }
        }

        public virtual void Unload()
        {
        }
    }
}