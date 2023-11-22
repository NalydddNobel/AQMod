﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Common.Particles; 

public abstract class BaseBloomParticle<T> : BaseParticle<T> where T : BaseBloomParticle<T>, new() {
    public Color BloomColor;
    public float BloomScale = 1f;

    public Vector2 bloomOrigin;

    public T Setup(Vector2 position, Vector2 velocity, Color color = default(Color), Color bloomColor = default(Color), float scale = 1f, float bloomScale = 1f, float rotation = 0f) {
        Position = position;
        Velocity = velocity;
        Color = color;
        Scale = scale;
        Rotation = rotation;
        BloomColor = bloomColor;
        BloomScale = bloomScale;
        dontEmitLight = false;
        SetDefaults();
        bloomOrigin = AequusTextures.Bloom.Size() / 2f;
        return (T)this;
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        spritebatch.Draw(AequusTextures.Bloom, Position - Main.screenPosition, null, BloomColor, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
        base.Draw(ref settings, spritebatch);
    }
}