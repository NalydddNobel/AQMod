using System;
using Terraria.Graphics.Renderers;

namespace Aequus.Common.Particles;

// TODO -- Port this over to newer particle system.
public class EmberParticles : BaseParticle<EmberParticles> {
    public float blackness;
    public float opacity;

    public static void FromTexture(Vector2 topLeftSpawnPosition, Texture2D texture, Rectangle frame, int pixelSizeX, int pixelSizeY, ParticleLayer particleLayer = ParticleLayer.AbovePlayers, Action<EmberParticles> onSpawnAction = null) {
        int x = 0;
        for (int i = frame.X; i < frame.X+frame.Width + pixelSizeX; i += pixelSizeX) {
            int y = 0;
            for (int j = frame.Y; j < frame.Y + frame.Height + pixelSizeY; j += pixelSizeY) {
                var particle = ParticleSystem.New<EmberParticles>(particleLayer).Setup(
                    topLeftSpawnPosition + new Vector2(x, y),
                    Main.rand.NextVector2Square(-2f, 2f),
                    Main.LocalPlayer.hairColor,
                    1f,
                    0f
                );
                particle.Velocity *= 0.4f;
                particle.Velocity.Y = Math.Abs(particle.Velocity.Y);
                particle.texture = texture;
                particle.frame = new Rectangle(i, j, pixelSizeX, pixelSizeY);
                particle.origin = Vector2.Zero;
                particle.dontEmitLight = true;
                particle.blackness = 0f;
                particle.opacity = 1f;

                onSpawnAction?.Invoke(particle);

                y += pixelSizeY;
            }
            x += pixelSizeX;
        }
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        var color = GetParticleColor(ref settings);
        color = Color.Lerp(color, Color.Black, Math.Clamp(blackness, 0f, 1f));
        color *= opacity;
        spritebatch.Draw(texture, Position - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
    }

    public override void Update(ref ParticleRendererSettings settings) {
        if (Scale <= 0.1f || float.IsNaN(Scale)) {
            RestInPool();
            return;
        }
        blackness += Main.rand.NextFloat(0.02f);
        if (blackness > 2f) {
            Scale -= 0.01f;
            Velocity.Y += 0.03f;
            Velocity.X *= 0.9f;
        }
        opacity -= 0.005f;
        float playerAnchor = MathF.Pow(Math.Clamp(2f - blackness, 0f, 1f), 3f);
        Position += (Main.LocalPlayer.position - Main.LocalPlayer.oldPosition) * playerAnchor;
        Position += Velocity * (1f - playerAnchor);
        Rotation = 0f;
    }
}