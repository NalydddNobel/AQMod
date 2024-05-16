using Aequus.Content.Dusts;
using Aequus.DataSets;
using Aequus.Old.Content.StatusEffects;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Items.Weapons.Glimmer.SuperStarSword;

public class SuperStarSwordSlash : ModProjectile {
    public override string Texture => AequusTextures.SlashVanillaAlt.Path;

    private bool _didEffects;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 18;
        ProjectileDataSet.PushableByTypeId.Add(Type);
        ProjectileDataSet.DealsHeatDamage.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 102;
        Projectile.height = 102;
        Projectile.timeLeft = 120;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.extraUpdates = 3;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 16 * 6;

        _didEffects = false;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(166, 160, 255, 0);
    }

    public override void AI() {
        var r = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width / 4f, Projectile.height / 4f));
        if (Collision.SolidCollision(r.TopLeft(), r.Width, r.Height)) {
            Projectile.velocity *= 0.95f;
            Projectile.damage -= 1;
            if (Projectile.timeLeft > 32) {
                Projectile.timeLeft--;
            }

            if (Projectile.damage < 0) {
                if (Projectile.velocity.Length() > 2f) {
                    Projectile.velocity *= 0.9f;
                }

                Projectile.damage = 0;
            }
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        if (Projectile.timeLeft < 32) {
            Projectile.alpha += 8;
            Projectile.scale -= 0.008f;
            if (Projectile.alpha > 255) {
                Projectile.Kill();
            }
        }
        else {
            Projectile.velocity *= 0.975f;
            if (Main.rand.NextBool((int)Math.Max(8f - Projectile.velocity.Length(), 2f))) {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-50f, 50f) * Projectile.scale,
                    ModContent.DustType<MonoDust>(), Projectile.velocity * 0.2f, 0, Main.rand.Next(SuperStarSwordProj.DustColors) with { A = 0 }, Main.rand.NextFloat(0.9f, 1.45f));
                d.noGravity = true;
            }
            else {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1.3f, 1.3f))) * 38f * Projectile.scale,
                    ModContent.DustType<MonoDust>(), Projectile.velocity, 0, Main.rand.Next(SuperStarSwordProj.DustColors) with { A = 0 }, Main.rand.NextFloat(0.9f, 1.45f));
                d.noGravity = true;
            }

            if (Main.rand.NextBool(4)) {
                var p = ModContent.GetInstance<BlueFireSparkle>().New();
                p.Location = Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1.3f, 1.3f))) * 38f * Projectile.scale;
                p.Velocity = Projectile.velocity;
                p.Color = new Color(255, 255, 255, 0);
                p.Scale = Main.rand.NextFloat(0.6f, 1f);
                p.animation = Main.rand.Next(20);
            }
        }

        if (!_didEffects) {
            _didEffects = true;
            if (Main.netMode != NetmodeID.Server) {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Pitch = 0.33f }, Projectile.Center);
            }
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        float _ = float.NaN;
        var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
            Projectile.Center + normal * -46f, Projectile.Center + normal * 46f, 32f * Projectile.scale, ref _);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * 0.9f);
        target.AddBuff(ModContent.BuffType<BlueFire>(), 240);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        target.AddBuff(ModContent.BuffType<BlueFire>(), 240);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out _, out _, out int trailLength);
        float slashRotation = Projectile.rotation;
        Color glowColor = (Main.tenthAnniversaryWorld ? Color.HotPink with { A = 0 } * 0.45f : Color.CornflowerBlue * 0.9f) * Projectile.Opacity;
        lightColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        Rectangle frame = texture.Frame(horizontalFrames: 2, verticalFrames: 4, frameX: 1, frameY: 0);
        Vector2 origin = frame.Size() / 2f;
        Vector2 rotationVector = Projectile.rotation.ToRotationVector2();
        Vector2 slashCoordinates = Projectile.position + offset - Main.screenPosition + rotationVector * 30f;

        Texture2D bloom = AequusTextures.Bloom;
        Vector2 bloomOrigin = bloom.Size() / 2f;
        float slashScale = Projectile.scale * 0.75f;

        Main.EntitySpriteDraw(texture, slashCoordinates, frame, glowColor, slashRotation, origin, slashScale, SpriteEffects.FlipHorizontally, 0);
        frame = texture.Frame(horizontalFrames: 2, verticalFrames: 4, frameX: 1, frameY: 3);
        Main.EntitySpriteDraw(texture, slashCoordinates + rotationVector * -5f, frame, lightColor, slashRotation, origin, slashScale * 1.1f, SpriteEffects.FlipHorizontally, 0);
        Main.EntitySpriteDraw(bloom, Projectile.position + offset - Main.screenPosition - rotationVector * 12f, null, glowColor with { A = 0 } * Projectile.Opacity * 0.8f, Projectile.rotation + MathHelper.PiOver2, bloomOrigin, new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.FlipHorizontally, 0);

        return false;
    }
}