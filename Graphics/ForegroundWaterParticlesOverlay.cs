using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;

namespace Aequus.Graphics
{
    public class ForegroundWaterParticlesOverlay : Overlay
    {
        public const string Key = "Aequus:ForegroundWater";
        private bool isVisible;

        public ForegroundWaterParticlesOverlay(EffectPriority priority, RenderLayers layer) : base(priority, layer)
        {
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isVisible = true;
        }

        public override void Deactivate(params object[] args)
        {
            isVisible = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            EffectsSystem.ParticlesAboveLiquids.Draw(spriteBatch);
        }

        public override bool IsVisible()
        {
            return isVisible;
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}