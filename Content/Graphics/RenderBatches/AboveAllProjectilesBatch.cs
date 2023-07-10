using Aequus.Common.Graphics.RenderBatches;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Content.Graphics.RenderBatches {
    public class AboveAllProjectilesBatch : RenderLayerBatch {
        public static AboveAllProjectilesBatch Instance { get; private set; }

        public override void OnLoad() {
            Instance = this;
        }

        public override void OnUnload() {
            Instance = null;
        }

        public override void Begin(SpriteBatch spriteBatch) {
            spriteBatch.Begin_World(shader: false);
        }

        public override void End(SpriteBatch spriteBatch) {
            spriteBatch.End();
        }
    }
}