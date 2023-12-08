using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

// TODO -- Make inherit bullet effects?
public class EtOmniaVanitasBullet : ModProjectile {
    public EtOmniaVanitasParticle.Spawner _particleSpawner;

    public override string Texture => AequusTextures.Projectile(ProjectileID.Bullet);

    private enum ProjectileState : byte {
        Normal,
        Exploding
    }

    private int BulletProjType => (int)Projectile.ai[0];
    private ProjectileState State { get => (ProjectileState)Projectile.ai[1]; set => Projectile.ai[1] = (float)value; }

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = 0;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.timeLeft = 3600;
        Projectile.alpha = 255;
        _particleSpawner = new();
    }

    public override void AI() {
        switch (State) {
            case ProjectileState.Exploding: {
                    if (Projectile.ai[2] == 0f) {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                        Projectile.ai[2] = 1f;
                        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < Math.Max(12 - Main.player[Projectile.owner].ownedProjectileCounts[Type], 2); i++) {
                            var v = Main.rand.NextVector2Unit();
                            _particleSpawner.New(Projectile.Center + v * Main.rand.NextFloat(32f), v * Main.rand.NextFloat(6f));
                        }
                        _particleSpawner.Clear();
                    }

                    Projectile.timeLeft = 30;
                    Projectile.extraUpdates = 0;
                    if (Projectile.numUpdates == -1) {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter++ > 6 || Projectile.frame < 2) {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                            if (Projectile.frame >= ExplosionFrames) {
                                Projectile.Kill();
                            }
                        }
                    }
                    Projectile.alpha = 0;
                }
                break;

            default: {
                    if (Projectile.alpha == 255) {
                        Projectile.extraUpdates += (int)(Projectile.velocity.Length() / 16f);
                        Projectile.velocity /= Projectile.MaxUpdates;
                    }
                    int chance = Projectile.MaxUpdates;
                    float speed = Projectile.velocity.Length() * Projectile.MaxUpdates;
                    float chanceMultiplier = Math.Max(8f - MathF.Pow(speed / 20f, 2f), 2f);
                    int finalChance = Math.Max((int)(Projectile.MaxUpdates * chanceMultiplier - Projectile.localAI[0]), 2);
                    if (Main.rand.NextBool(finalChance) && Projectile.alpha < 220) {
                        _particleSpawner.New(Projectile.Center, Main.rand.NextVector2Square(-speed, speed) * 0.06f);
                        Projectile.localAI[0] = 0f;
                    }
                    if (Projectile.alpha > 0) {
                        Projectile.alpha -= 20;
                        if (Projectile.alpha <= 0) {
                            Projectile.alpha = 0;
                        }
                    }
                    Projectile.localAI[0]++;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                break;
        }

    }

    public override void OnKill(int timeLeft) {
        _particleSpawner.Clear();
    }

    public override bool PreDraw(ref Color lightColor) {
        switch (State) {
            case ProjectileState.Exploding:
                DrawExplosion();
                break;

            default: {
                    var texture = TextureAssets.Extra[ExtrasID.BlackBolt].Value;
                    Projectile.GetDrawInfo(out _, out var offset, out _, out _, out int trailLength);

                    var drawCoordinates = Projectile.position + offset - Main.screenPosition;
                    var scale = new Vector2(0.2f, 1f);
                    var trailColor = new Color(50, 100, 140, 100) * Projectile.Opacity;
                    for (int i = 0; i < trailLength; i++) {
                        float progress = 1f - 1f / trailLength * i;
                        Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 4f).ToRotationVector2() * 2f, null, trailColor * 0.6f * progress, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, scale * progress, SpriteEffects.None, 0f);
                    }
                    for (int i = 0; i < 4; i++) {
                        Main.EntitySpriteDraw(texture, drawCoordinates - Projectile.velocity + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 4f).ToRotationVector2() * 2f, null, trailColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
                    }
                    Main.EntitySpriteDraw(texture, drawCoordinates, null, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, scale * 0.8f, SpriteEffects.None, 0f);
                }
                break;
        }
        return false;
    }

    #region Explosion
    public const int ExplosionFrames = 6;

    public override bool? CanDamage() {
        return State == ProjectileState.Normal;
    }

    public override bool ShouldUpdatePosition() {
        return State != ProjectileState.Exploding;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (Projectile.penetrate != 1) {
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f), Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, BulletProjType, (float)State, 0f);
        }
        else {
            EnterExplodeState();
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        EnterExplodeState();
        return false;
    }

    private void EnterExplodeState() {
        Projectile.ai[2] = 0f;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        State = ProjectileState.Exploding;
        Projectile.netUpdate = true;
        Projectile.position += Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f);
    }

    public void DrawExplosion() {
        var texture = AequusTextures.EtOmniaVanitasExplosionSmall.Value;
        var frame = texture.Frame(verticalFrames: ExplosionFrames, frameY: Projectile.frame);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, frame.Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
    }
    #endregion
}