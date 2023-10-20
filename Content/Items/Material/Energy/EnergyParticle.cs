using Aequus;
using Aequus.Common.Particles;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Content.Items.Material.Energy;

public class EnergyParticle : EnergyParticle<EnergyParticle> { }

public class EnergyParticle<T> : BaseParticle<T> where T : EnergyParticle<T>, new() {
    private float animationTime;
    private int frameNumber;

    public virtual Asset<Texture2D> Texture => AequusTextures.EnergyParticle;

    protected override void SetDefaults() {
        SetFramedTexture(Texture, 5, 0);
        animationTime = Main.rand.NextFloat(-20f, -6f);
    }

    public override void Update(ref ParticleRendererSettings settings) {
        if (animationTime < 0f) {
            Velocity *= 1.05f;
        }
        if (animationTime > 16f) {
            Velocity *= Main.rand.NextFloat(0.9f, 1f);
        }
        frameNumber = (int)(animationTime / 6);
        frame.Y = frame.Height * frameNumber;
        if (frameNumber >= 6) {
            RestInPool();
            return;
        }
        animationTime++;
        Position += Velocity;
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        float intensity = MathF.Sin(animationTime / 36f * MathHelper.Pi);
        spritebatch.Draw(AequusTextures.BloomStrong, Position - Main.screenPosition + new Vector2(0f, -4f), null, Color with { A = 0 } * 0.3f * intensity, Rotation, AequusTextures.BloomStrong.Size() / 2f, Scale * 0.4f, SpriteEffects.None, 0f);
        spritebatch.Draw(texture, Position - Main.screenPosition, frame, GetParticleColor(ref settings) * intensity, Rotation, origin, new Vector2(0.8f + 0.2f * intensity, 1f) * Scale, SpriteEffects.None, 0f);
    }
}