using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Common.Particles;

public class MovementParticle : BaseParticle<MovementParticle> {
    public float Opacity;

    protected override void SetDefaults() {
        SetFramedTexture(AequusTextures.Movement, 3);
        Opacity = 1f;
    }

    public override void Update(ref ParticleRendererSettings settings) {
        if (Opacity <= 0f || Scale <= 0.1f || float.IsNaN(Scale)) {
            RestInPool();
            return;
        }

        Rotation = Velocity.ToRotation() + MathHelper.Pi;
        Position += Velocity;
        Velocity *= 0.85f;
        if (Velocity.LengthSquared() <= 32f) {
            Opacity *= 0.95f;
            Opacity -= 0.01f;
        }
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        var color = Utils.MultiplyRGBA(GetParticleColor(ref settings) * 0.66f * Opacity, LightHelper.GetLightColor(Position));
        for (int i = 0; i < 7; i++) {
            spritebatch.Draw(texture, Position - Main.screenPosition - Velocity, frame, color * 0.1f, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
        spritebatch.Draw(texture, Position - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
}