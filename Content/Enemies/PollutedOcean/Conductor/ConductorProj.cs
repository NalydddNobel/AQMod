using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

public class ConductorProj : ModProjectile {
    public override string Texture => AequusTextures.ScrapBlockItem.Path;

    public int Parent => (int)Projectile.ai[2];
    public bool WaterSphere => Projectile.localAI[1] == 1f;

    public override void SetDefaults() {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
    }

    public override void AI() {
        int timer = (int)Projectile.ai[1];
        Projectile.ai[1]++;

        if (++Projectile.frameCounter > 6) {
            Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            Projectile.frameCounter = 0;
        }

        Projectile.localAI[0] += 1f;
        Projectile.rotation += Projectile.direction * 0.25f;
        if (!Main.npc[Parent].active) {
            Projectile.ai[0] = 2f;
        }
        switch ((int)Projectile.ai[0]) {
            case 0: {
                    Conductor.GetAttackTimings(out _, out _, out int attackTime);

                    int closestPlayer = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
                    bool collision = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
                    Projectile.tileCollide = false;
                    if (collision) {
                        Projectile.velocity += Projectile.DirectionTo(Main.player[closestPlayer].Center) * 0.4f;

                        float speedCap = 2f;
                        if (Main.expertMode) {
                            speedCap = 4f;
                        }
                        if (Main.getGoodWorld) {
                            speedCap = 24f;
                        }
                        if (Projectile.velocity.Length() > speedCap) {
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= speedCap;
                        }
                    }
                    else {
                        Projectile.velocity.X *= 0.9f;
                    }
                    if (timer == 0) {
                        Projectile.velocity.Y = -5f;
                        if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height)) {
                            Projectile.localAI[1] = 1f;
                        }
                    }
                    else if (timer < 40 && Projectile.velocity.Y < 0f) {
                        Projectile.velocity.Y += 0.22f;
                    }
                    else if (Main.npc[Parent].ai[1] > attackTime && !collision) {
                        Projectile.ai[0] = 1f;
                        float speed = Conductor.ATTACK_SHOOT_VELOCITY_CLASSIC;
                        if (Main.expertMode) {
                            speed = Conductor.ATTACK_SHOOT_VELOCITY_EXPERT;
                        }
                        Projectile.velocity = Projectile.DirectionTo(Main.player[closestPlayer].Center) * speed;
                        Projectile.netUpdate = true;
                    }
                    else {
                        Projectile.velocity *= 0.85f;
                    }
                    Projectile.hostile = false;

                    if (timer % 12 == 0) {
                        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror);
                        d.noGravity = true;
                        d.velocity *= 0.8f;
                        d.fadeIn = d.scale + 0.2f;
                    }
                    if (timer % 9 == 0 && Main.netMode != NetmodeID.MultiplayerClient) {
                        Projectile.velocity += Main.rand.NextVector2Unit() * 1f;
                        Projectile.netUpdate = true;
                    }
                    Projectile.timeLeft = Math.Max(Projectile.timeLeft, 70);
                }
                break;

            case 1: {
                    Projectile.tileCollide = true;
                    Projectile.hostile = true;
                    Projectile.UpdateShimmerReflection();
                }
                break;

            case 2: {
                    if (WaterSphere) {
                        Projectile.Kill();
                    }
                    Projectile.tileCollide = true;
                    Projectile.hostile = false;
                    Projectile.UpdateShimmerReflection();
                    Projectile.velocity.X *= 0.94f;
                    Projectile.velocity.Y += 0.2f;
                }
                return;
        }

        if (Main.rand.NextBool()) {
            Vector2 randomVector2 = Main.rand.NextVector2Unit();
            Dust d = Dust.NewDustPerfect(Projectile.Center + randomVector2 * Projectile.Size / 2f, DustID.MagicMirror, Velocity: randomVector2 - Projectile.velocity * 0.25f);
            d.noGravity = true;
        }

        if (Projectile.ai[2] == 1f && Main.rand.NextBool()) {
            int dustType = Dust.dustWater();

            Dust d = Dust.NewDustPerfect(Projectile.Center, dustType, Scale: 1f);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.1f;
            d.customData = this;
        }
    }

    public override void OnKill(int timeLeft) {
        if (!WaterSphere) {
            for (int i = 0; i < 20; i++) {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, i % 2 == 0 ? DustID.Copper : DustID.Tin, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
            }
        }
        SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out _);
        Vector2 drawCoordinates = Projectile.position + offset - Main.screenPosition;
        SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        if ((int)Projectile.ai[0] != 2) {
            Main.EntitySpriteDraw(AequusTextures.Bloom, drawCoordinates, null, lightColor * 0.2f, Projectile.rotation, AequusTextures.Bloom.Size() / 2f, Projectile.scale * 0.66f, effects, 0f);
        }

        //Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor, 0f, origin, Projectile.scale, effects, 0f);
        if (!WaterSphere) {
            Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0f);
        }
        else {
            int dustType = Dust.dustWater();
            Texture2D dustTexture;
            Rectangle dustFrame;
            if (dustType < DustID.Count) {
                dustTexture = TextureAssets.Dust.Value;
                dustFrame = new Rectangle(10 * (dustType % 100), 30 * (dustType / 100), 8, 8);
            }
            else {
                dustTexture = DustLoader.GetDust(dustType).Texture2D.Value;
                dustFrame = new Rectangle(0, 0, 8, 8);
            }
            int maxDust = Math.Max((int)(70 * Main.gfxQuality), 20);
            Color dustColor = Color.Lerp(lightColor, Color.White with { A = 200 }, 0.5f) * 0.6f;
            for (int i = 0; i < maxDust; i++) {
                float animation = (i / (float)maxDust * 4f + Main.GlobalTimeWrappedHourly * 2f) % 4f;
                if (animation > 1f) {
                    continue;
                }
                Vector2 vector = (i * i * 10f + Projectile.whoAmI).ToRotationVector2();
                float particleDistance = 19f + MathF.Sin(i) * 9f;
                float scale = 2f + MathF.Sin(i * 1.33f) * 0.4f;
                Main.EntitySpriteDraw(dustTexture, drawCoordinates + vector * particleDistance * MathF.Pow(1f - animation, 2f), dustFrame, dustColor * MathF.Sin(animation * MathHelper.Pi), Projectile.rotation + i, dustFrame.Size() / 2f, Projectile.scale * scale, effects, 0f);
            }
        }
        return false;
    }
}
