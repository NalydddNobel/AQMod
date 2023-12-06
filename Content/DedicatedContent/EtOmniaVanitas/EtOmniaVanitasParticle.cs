using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

public class EtOmniaVanitasParticle : BaseParticle<EtOmniaVanitasParticle> {
    public class Spawner : IDisposable {
        private EtOmniaVanitasParticle _lastSpawned2;
        private EtOmniaVanitasParticle _lastSpawned;

        public void New(Vector2 position, Vector2 velocity) {
            var newParticle = ParticleSystem.New<EtOmniaVanitasParticle>(ParticleLayer.AboveNPCs)
                .Setup(position, velocity);
            newParticle.preventDespawn = true;
            newParticle.Prev = _lastSpawned;
            if (_lastSpawned != null) {
                if (_lastSpawned2 != null) {
                    _lastSpawned2.Next = _lastSpawned;
                    _lastSpawned2.preventDespawn = false;
                }
                _lastSpawned2 = _lastSpawned;
                _lastSpawned.Next = newParticle;
            }
            _lastSpawned = newParticle;
        }

        public void Clear() {
            if (_lastSpawned != null) {
                _lastSpawned.preventDespawn = false;
                _lastSpawned = null;
            }
            if (_lastSpawned2 != null) {
                _lastSpawned2.preventDespawn = false;
                _lastSpawned2 = null;
            }
        }

        public void Dispose() {
            Dispose();
            GC.SuppressFinalize(this);
        }

        // Clear particle caches in the object finalizer (GC Collector)
        ~Spawner() {
            Clear();
        }
    }

    public EtOmniaVanitasParticle Next;
    public EtOmniaVanitasParticle Prev;
    public bool preventDespawn;
    public byte frameCounter;

    protected override void SetDefaults() {
        if (Prev != null && Prev.Next == this) {
            Prev.Next = null;
        }
        Next = null;
        texture = AequusTextures.EtOmniaVanitasParticle;
        frame = texture.Frame(horizontalFrames: 3, verticalFrames: 4, frameX: Main.rand.Next(3), frameY: 0);
        origin = frame.Size() / 2f;
        frameCounter = 0;
        preventDespawn = false;
    }

    public override void Update(ref ParticleRendererSettings settings) {
        Velocity *= 0.92f;
        if (frameCounter > 100) {
            preventDespawn = false;
        }
        if (!preventDespawn) {
            Scale -= 0.03f - Velocity.Length() / 1000f;
        }
        Color = new Color(5, 30, 200, 200) * Math.Min(frameCounter / 20f, 1f);
        if (Scale <= 0.1f || float.IsNaN(Scale)) {
            if (!preventDespawn) {
                RestInPool();
                return;
            }

            Scale = 0.1f;
        }
        if (!dontEmitLight) {
            Lighting.AddLight(Position, Color.ToVector3() * 0.5f);
        }

        Position += Velocity;
        if (frameCounter++ % 6 == 0) {
            frame.Y += frame.Height;
            frame.Y %= texture.Height;
        }
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        if (Next != null) {
            DrawHelper.DrawLine(Position - Main.screenPosition, Next.Position - Main.screenPosition, 2f, Next.Color * Next.Scale * Scale);
            DrawHelper.DrawLine(Position - Main.screenPosition, Next.Position - Main.screenPosition, 6f, Next.Color * Next.Scale * Scale);
        }
        spritebatch.Draw(AequusTextures.EtOmniaVanitasParticle, Position - Main.screenPosition, frame, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
}