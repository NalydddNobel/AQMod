using Aequus.Common.Graphics.RenderBatches;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics.LayerRenderers {
    public interface ILayerRenderer : ILoadable {
        bool IsReady { get; }

        void SetupBatchLayers();

        void DrawToLayer(RenderLayerBatch layer, SpriteBatch spriteBatch);

        void OnUpdate() {
        }

        void OnClearWorld() {
        }
    }
}