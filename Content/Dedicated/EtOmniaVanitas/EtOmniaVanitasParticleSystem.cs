using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Dedicated.EtOmniaVanitas;

public class EtOmniaVanitasParticleSystem : ParticleArray<EtOmniaVanitasParticleSystem.Particle> {
    public override int ParticleCount => 1500;

    public override void Update() {
        Texture2D texture = AequusTextures.EtOmniaVanitasParticle;

        Active = false;
        for (int i = 0; i < Particles.Length; i++) {
            Particle p = Particles[i];
            if (p == null || !p.Active) {
                continue;
            }

            Active = true;
            p.Velocity *= 0.92f;
            p.Velocity = p.Velocity.RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.06f * p.Scale);
            if (p.frameCounter > 100) {
                p.preventDespawn = false;
            }
            if (!p.preventDespawn) {
                p.Scale -= 0.03f - p.Velocity.Length() / 1000f;
            }
            p.Color = new Color(90, 90, 220, 200) * Math.Min(p.frameCounter / 20f, 1f);
            if (p.Scale <= 0.1f || float.IsNaN(p.Scale)) {
                if (!p.preventDespawn) {
                    p.Active = false;
                    continue;
                }

                p.Scale = 0.1f;
            }

            Lighting.AddLight(p.Location, p.Color.ToVector3() * 0.5f);

            p.Location += p.Velocity;
            if (p.frameCounter++ % 6 == 0) {
                p.frame.Y += p.frame.Height;
                p.frame.Y %= texture.Height;
            }
        }
    }

    private readonly Queue<int> _drawQueue = new(20);

    public override void Draw(SpriteBatch spriteBatch) {
        Vector2 origin = new Vector2(11f, 11f);
        for (int i = 0; i < Particles.Length; i++) {
            Particle p = Particles[i];
            if (p == null || !p.Active) {
                continue;
            }

            if (p.Next != null) {
                Vector2 start = p.Location - Main.screenPosition;
                Vector2 end = p.Next.Location - Main.screenPosition;
                float scale = p.Scale;
                float nextScale = p.Next.Scale;
                DrawHelper.DrawLine(start, end, 6f * scale, Color.Cyan with { A = 0 } * 0.2f * nextScale * scale);
                DrawHelper.DrawLine(start, end, 2f * scale, Color.LightCyan * 0.66f * nextScale * scale);
            }
            _drawQueue.Enqueue(i);
        }

        while (_drawQueue.TryDequeue(out int i)) {
            Particle p = Particles[i];
            Vector2 drawCoordinates = p.Location - Main.screenPosition;
            Rectangle frame = p.frame;
            float scale = p.Scale;
            spriteBatch.Draw(AequusTextures.EtOmniaVanitasParticle, drawCoordinates, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }

    public override void OnActivate() {
        DrawLayers.Instance.PostDrawLiquids += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostDrawLiquids -= Draw;
    }

    public class Particle : IParticle {
        public Vector2 Location;
        public Vector2 Velocity;
        public Color Color;
        public Particle Next;
        public Particle Prev;
        public Spawner Spawner;

        public bool preventDespawn;
        public byte frameCounter;

        public Rectangle frame;

        public float Scale;

        public bool Active { get; set; }
    }

    public class Spawner : IDisposable {
        private Particle _lastSpawned2;
        private Particle _lastSpawned;

        public void New(Vector2 position, Vector2 velocity) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            var newParticle = ModContent.GetInstance<EtOmniaVanitasParticleSystem>().New();
            newParticle.Location = position;
            newParticle.Velocity = velocity;
            newParticle.preventDespawn = true;
            newParticle.Scale = 1f;
            newParticle.Spawner = this;
            newParticle.Prev = _lastSpawned;
            newParticle.Next = null;
            newParticle.frame = AequusTextures.EtOmniaVanitasParticle.Frame(horizontalFrames: 3, verticalFrames: 4, frameX: Main.rand.Next(3), frameY: 0);
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
            Clear();
            GC.SuppressFinalize(this);
        }

        // Clear particle caches in the object finalizer (GC Collector)
        ~Spawner() {
            Clear();
        }
    }
}