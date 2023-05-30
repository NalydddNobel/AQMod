using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Aequus.Common.Effects.RenderBatches {
    public interface ILayerRenderer : ILoadable
    {
        bool IsReady { get; }

        void SetupBatchLayers();

        void DrawToLayer(RenderLayerBatch layer, SpriteBatch spriteBatch);
    }
}