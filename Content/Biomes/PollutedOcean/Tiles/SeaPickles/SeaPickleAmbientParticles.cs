using Aequus.Common.Particles;
using Aequus.Core;
using Aequus.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria.DataStructures;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

public class SeaPickleAmbientParticles : ParticleBatch {
    private readonly Dictionary<Point16, List<AmbientParticle>> _particleAnchorPairs = new();

    public class AmbientParticle : IPoolable {
        public Vector2 position;
        public Vector2 oldPosition;

        public Vector2 Velocity => position - oldPosition;

        public short animation;
        public float scale;

        public Rectangle _frame;

        public bool Resting { get; set; }
    }

    public void New(Point16 where) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var particle = InstancePool<AmbientParticle>.Get();
        particle.animation = (short)Main.rand.Next(-120, 0);
        particle.scale = 0.25f;
        particle.position = Main.rand.NextVector2FromRectangle(new(where.X * 16, where.Y * 16, 16, 16));
        particle.oldPosition = particle.position - Main.rand.NextVector2Unit() * 0.5f;
        particle._frame = new Rectangle(Main.rand.Next(2) * 8, Main.rand.Next(3) * 8, 6, 6);

        (CollectionsMarshal.GetValueRefOrAddDefault(_particleAnchorPairs, where, out _) ??= new()).Add(particle);
    }

    public override void Load() {
        DrawLayers.PostDrawLiquids += Draw;
    }

    public override void Update() {
        lock (_particleAnchorPairs) {
            foreach (var p in _particleAnchorPairs) {
                var tileAnchor = p.Key;
                var tileWorldAnchor = tileAnchor.ToWorldCoordinates() + new Vector2(0f, -32f) + Main.GlobalTimeWrappedHourly.ToRotationVector2() * 32f;
                for (int i = 0; i < p.Value.Count; i++) {
                    var particle = p.Value[i];

                    if (particle.animation < 0) {
                        if (particle.scale < 1f) {
                            particle.scale += 0.04f;
                        }
                    }
                    else if (particle.animation > 240) {
                        particle.scale -= Main.rand.NextFloat(0.01f);
                    }

                    var tileCoordinate = particle.position.ToTileCoordinates();
                    if (particle.scale <= 0.1f || !WorldGen.InWorld(tileCoordinate.X, tileCoordinate.Y, 10)) {
                        p.Value.RemoveAt(i);
                        particle.Rest();
                        i--;
                        continue;
                    }

                    var velocity = particle.Velocity;
                    particle.oldPosition = particle.position;
                    velocity = Vector2.Lerp(velocity, (tileWorldAnchor - particle.position) / 16f, 0.001f);
                    if (WorldGen.SolidTile(tileCoordinate.X - 1, tileCoordinate.Y)) {
                        velocity.X += 0.02f;
                    }
                    if (WorldGen.SolidTile(tileCoordinate.X + 1, tileCoordinate.Y)) {
                        velocity.X -= 0.02f;
                    }

                    if (WorldGen.SolidTile(tileCoordinate.X, tileCoordinate.Y + 1)) {
                        velocity.Y -= 0.02f;
                    }
                    if (WorldGen.SolidTile(tileCoordinate.X, tileCoordinate.Y - 1) || Main.tile[tileCoordinate.X, tileCoordinate.Y - 1].LiquidAmount < 100) {
                        velocity.Y += 0.02f;
                    }

                    float speed = velocity.Length();
                    if (speed < 0.3f) {
                        velocity.Normalize();
                        velocity *= 0.3f;
                    }
                    else if (speed > 1f) {
                        velocity *= 0.96f;
                    }

                    particle.position += velocity;

                    particle.animation++;
                    if (Collision.SolidCollision(particle.position, 2, 2)) {
                        particle.animation = Math.Max(particle.animation, (short)240);
                        particle.scale *= 0.96f;
                    }
                }
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch) {
        var texture = AequusTextures.SmallLightParticle;
        var origin = new Vector2(3f, 3f);
        var bloomTexture = AequusTextures.BloomStrong;
        var bloomOrigin = bloomTexture.Size() / 2f;
        lock (_particleAnchorPairs) {
            foreach (var pair in _particleAnchorPairs) {
                //var drawColor = new Color(20, 255, 150) * 0.6f; // Turquoise
                //var drawColor = new Color(255, 255, 30) * 0.5f; // Yellow
                SeaPicklesTileBase.GetDrawData(pair.Key.X, pair.Key.Y, out var drawColor);

                var bloomColor = drawColor with { A = 0 } * 0.03f;

                var lightColor = Lighting.GetColor(pair.Key.ToPoint());
                float intensity = Math.Min((lightColor.R + lightColor.G + lightColor.B) / 300f, 1f);
                foreach (var p in pair.Value) {
                    var frame = p._frame;
                    spriteBatch.Draw(bloomTexture, p.position - Main.screenPosition, null, bloomColor * intensity * p.scale, 0f, bloomOrigin, p.scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, p.position - Main.screenPosition, frame, drawColor * intensity, 0f, origin, p.scale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}