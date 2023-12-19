using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent;

namespace Aequus.Content.Items.Weapons.Classless.StunGun;

public class StunGunProj : ModProjectile {
    public override string Texture => AequusTextures.Extra(ExtrasID.RainbowRodTrailShape);

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 100;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.extraUpdates = 40;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 2;
        Projectile.friendly = true;
        Projectile.timeLeft = 400;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 2;
        height = 2;
        return true;
    }

    private void OnHitAnythingAtAll() {
        Projectile.friendly = false;
        Projectile.velocity = Vector2.Zero;
        Projectile.tileCollide = false;
    }

    public override void AI() {
        if (Projectile.numUpdates == -1) {
            Projectile.Opacity = 1f - (Projectile.ai[2] - 50f) / 350f;
            Projectile.friendly = false;
            Projectile.velocity = Vector2.Zero;
            Projectile.extraUpdates = 10;
        }

        if (!Projectile.tileCollide || Projectile.velocity == Vector2.Zero) {
            return;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height)) {
            OnHitAnythingAtAll();
        }
        if (Projectile.ai[0] == 0f || Projectile.ai[1] == 0f) {
            Projectile.ai[0] = Projectile.velocity.X;
            Projectile.ai[1] = Projectile.velocity.Y;
        }
        Projectile.ai[2]++;
        var velocity = new Vector2(Projectile.ai[0], Projectile.ai[1]);
        if (Projectile.ai[2] > 6f) {
            Projectile.velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
        }
        if (Projectile.friendly) {
            int target = Projectile.FindTargetWithLineOfSight(100f);
            if (target != -1) {
                velocity = Vector2.Lerp(velocity, Projectile.DirectionTo(Main.npc[target].Center) * 16f, 0.3f);
                Projectile.ai[0] = velocity.X;
                Projectile.ai[1] = velocity.Y;
            }
        }
        else if (Projectile.hostile) {
            velocity = Vector2.Lerp(velocity, Projectile.DirectionTo(Main.player[Projectile.owner].Center) * 16f, 0.1f);
            Projectile.ai[0] = velocity.X;
            Projectile.ai[1] = velocity.Y;
        }
        Projectile.ai[0] *= 0.98f;
        Projectile.ai[1] *= 0.98f;
        if (Projectile.ai[2] > 4f && (Projectile.friendly || Projectile.hostile) && Main.rand.NextBool(Math.Max(Projectile.MaxUpdates / 15, 1))) {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Square(-2f, 2f), DustID.Electric, Scale: 0.75f);
            d.velocity *= Main.rand.NextFloat(0.1f, 0.2f);
            d.velocity += Projectile.velocity * Main.rand.NextFloat(0f, 0.2f);
            d.noGravity = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        OnHitAnythingAtAll();
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        modifiers.SetMaxDamage(target.lifeMax / 5);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (target.reflectsProjectiles || target.type == NPCID.ShimmerSlime) {
            target.ReflectProjectile(Projectile);
            Projectile.penetrate = 2;
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
            Projectile.ai[0] = Projectile.velocity.X;
            Projectile.ai[1] = Projectile.velocity.Y;
            return;
        }
        OnHitAnythingAtAll();
        target.AddBuff(ModContent.BuffType<StunGunDebuff>(), StunGun.DebuffTime);
        Projectile.Center = target.Center;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        OnHitAnythingAtAll();
        target.AddBuff(ModContent.BuffType<StunGunDebuff>(), StunGun.DebuffTime);
        Projectile.Center = target.Center;
    }

    public override bool PreDraw(ref Color lightColor) {
        float opacity = Projectile.Opacity;
        DrawHelper.DrawBasicVertexLine(TextureAssets.MagicPixel.Value, Projectile.oldPos, Projectile.oldRot,
            (p) => Color.Cyan with { A = 0 } * opacity * p,
            (p) => 2f + MathF.Sin(p * MathHelper.Pi) * (2f * opacity),
            -Main.screenPosition + Projectile.Size / 2f
        );
        return false;
    }
}