using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.NPCs.BossMonsters.Crabson.Effects;

public class CrabsonSlamParticle : BaseParticle<CrabsonSlamParticle> {

    public int frameY;
    public int frameCounter;

    public static CrabsonSlamParticle New(int i, int j, float opacity) {
        return ParticleSystem.New<CrabsonSlamParticle>(ParticleLayer.AboveDust)
            .Setup(
            new(i * 16f + 8f, j * 16f - 6f),
            -Vector2.UnitY,
            Color.White, 1f, 0f
        );
    }

    public override CrabsonSlamParticle CreateBaseInstance() {
        return new();
    }

    protected override void SetDefaults() {
        SetFramedTexture(AequusTextures.SlamEffect0, 4, 0);
        frameY = 0;
        dontEmitLight = true;
    }

    public override void Update(ref ParticleRendererSettings settings) {

        if (frameCounter++ > 5) {

            frameY++;
            if (frameY > 4) {
                ShouldBeRemovedFromRenderer = true;
                return;
            }
            frameCounter = 0;
            frame.Y = frame.Height * frameY;
        }

        Velocity *= 0.9f;
        Position += Velocity;
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
}