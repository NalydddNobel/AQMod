using Aequu2.Core.Concurrent;
using Aequu2.Core.Graphics;
using Aequu2.Core.Particles;
using System;

namespace Aequu2.Content.Critters.SeaFirefly;

public class SeaFireflyClusters : ConcurrentParticles<SeaFireflyClusters.Particle> {
    public override void Activate() {
        DrawLayers.Instance.PostDrawLiquids += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostDrawLiquids -= Draw;
    }

    public override void Update(Particle t) {
        t.CycleTime++;

        if (t.IsLit) {
            if (t.ExistenceTime < 30) {
                t.ExistenceTime++;
            }
            if (t.LightOpacity < 1f) {
                t.LightOpacity += 0.1f;
                if (t.LightOpacity > 1f) {
                    t.LightOpacity = 1f;
                }
            }

            if (t.Frame < 2) {
                try {
                    ParallelLighting.Instance.Add(t.Where, t.Current.GetLightColor(t.Where) * 0.33f);
                }
                catch (Exception ex) {
                    Log.Error(ex);
                }
            }
        }
        else {
            t.ExistenceTime++;

            if (t.LightOpacity > 0f) {
                t.LightOpacity -= 0.1f;
                if (t.LightOpacity < 0f) {
                    t.LightOpacity = 0f;
                }
            }
        }

        if (t.ExistenceTime < 120 && t.ExistenceTime >= 30 && t.Opacity < 1f) {
            t.Opacity += 0.01f;
            if (t.Opacity > 1f) {
                t.Opacity = 1f;
            }
        }
        if (t.ExistenceTime > 360 && t.Opacity > 0f) {
            t.Opacity -= 0.01f;
            if (t.Opacity < 0f) {
                t.Active = false;
            }
        }

        if (!Collision.WetCollision(t.Where, 2, 2)) {
            t.Active = false;
        }

        Vector2 velocity = t.Velocity;
        t.WhereLast = t.Where;

        switch (t.CycleTime % 5) {
            case 0: {
                    Point tileCoordinates = t.Where.ToTileCoordinates();
                    TileHelper.ScanUp(tileCoordinates, 2, out Point top, TileHelper.LiquidNoMax);
                    Tile tile = Framing.GetTileSafely(top);

                    int waterLine = TileHelper.GetWaterLine(top);
                    if (t.Where.Y <= waterLine + 4) {
                        velocity.Y += 0.04f;
                    }
                }
                break;

            case 1: {
                    if (velocity.LengthSquared() > 1f) {
                        velocity *= 0.9f;
                    }
                }
                break;

            case 2:
                velocity.Y *= 0.97f;
                break;

            case 3:
                if (Math.Abs(velocity.X) < 0.05f) {
                    velocity.X *= 1.1f;
                }
                break;
        }

        Vector2 oldVelocity = velocity;
        velocity = Collision.TileCollision(t.Where - new Vector2(3f), velocity, 6, 6, true, true);

        if (velocity.X != oldVelocity.X) {
            velocity.X = -oldVelocity.X;
            t.ExistenceTime *= 4;
        }
        if (velocity.Y != oldVelocity.Y) {
            velocity.Y = -oldVelocity.Y;
            t.ExistenceTime *= 4;
        }

        t.Where += velocity.RotatedBy(Main.rand.NextFloat(-0.03f, 0.03f));
    }

    public override void Draw(SpriteBatch spriteBatch, Particle t) {
        Color lightColor = ExtendLight.Get(t.Where);
        float rotation = t.Velocity.X * 0.2f;
        SpriteEffects effects = t.Velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        SeaFirefly.DrawSeaFirefly(t.Current, spriteBatch, t.Where, Main.screenPosition, lightColor, t.Opacity, t.LightOpacity * 0.7f, rotation, effects, 1f, t.Frame * 2, t._randomSeed, t.IsLit);
    }

    public class Particle : IParticle {
        public bool Active { get; set; }

        public Vector2 Where;
        public Vector2 WhereLast;
        public byte Frame;
        public int ExistenceTime;
        public int CycleTime;
        public float Opacity;
        public float LightOpacity;
        public byte _randomSeed;
        public byte Color;

        public bool IsLit => CycleTime % SeaFirefly.CycleTime > SeaFirefly.DarkTime;

        public Vector2 Velocity => Where - WhereLast;

        public ISeaFireflyInstanceData Current => SeaFireflyRegistry.GetPalette(Color);

        public void Setup(Vector2 where, byte frame, int time, byte color) {
            Where = where;
            WhereLast = where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.35f);
            Frame = frame;

            do {
                time += Main.rand.Next(-15, 15);
            }
            while (Main.rand.NextBool(3));

            CycleTime = time;

            ExistenceTime = 0;

            Color = color;
            _randomSeed = (byte)Main.rand.Next(byte.MaxValue);
        }
    }
}
