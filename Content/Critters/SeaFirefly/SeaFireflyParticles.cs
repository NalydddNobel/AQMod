using Aequus.Core.Graphics;
using Aequus.Core.Particles;

namespace Aequus.Content.Critters.SeaFirefly;

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

            if (t.Frame == 1) {
                Lighting.AddLight(t.Where, new Vector3(0f, 0f, 0.1f));
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
                    tileCoordinates.Y--;
                    Tile above = Framing.GetTileSafely(tileCoordinates);
                    if (above.LiquidAmount != 255) {
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
        }

        Vector2 oldVelocity = velocity;
        velocity = Collision.TileCollision(t.Where - new Vector2(3f), velocity, 6, 6, true, true);

        if (velocity.X != oldVelocity.X) {
            velocity.X = -oldVelocity.X;
        }
        if (velocity.Y != oldVelocity.Y) {
            velocity.Y = -oldVelocity.Y;
        }

        t.Where += velocity.RotatedBy(Main.rand.NextFloat(-0.03f, 0.03f));
    }

    public override void Draw(SpriteBatch spriteBatch, Particle t) {
        Color lightColor = ExtendLight.Get(t.Where);
        float rotation = t.Velocity.X * 0.2f;
        SpriteEffects effects = t.Velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        SeaFirefly.DrawSeaFirefly(spriteBatch, t.Where, Main.screenPosition, lightColor, t.Opacity, t.LightOpacity * 0.7f, rotation, effects, 1f, t.Frame * 2, t._randomSeed, t.IsLit);
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
        public bool IsLit => CycleTime % SeaFirefly.CycleTime > SeaFirefly.DarkTime;
        public Vector2 Velocity => Where - WhereLast;

        public void Setup(Vector2 where, byte frame, int time) {
            Where = where;
            WhereLast = where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.35f);
            Frame = frame;

            do {
                time += Main.rand.Next(-15, 15);
            }
            while (Main.rand.NextBool(3));

            CycleTime = time;

            ExistenceTime = 0;

            _randomSeed = (byte)Main.rand.Next(byte.MaxValue);
        }
    }
}
