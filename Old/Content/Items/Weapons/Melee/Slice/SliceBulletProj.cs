using Aequus.DataSets;
using System;
using Terraria.GameContent;

namespace Aequus.Old.Content.Items.Weapons.Melee.Slice;

public class SliceBulletProj : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileDataSet.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 30;
        Projectile.alpha = 240;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 20;
        Projectile.scale = 2f;
        Projectile.tileCollide = false;
    }

    public override void AI() {
        Projectile.spriteDirection = Projectile.direction;
        if ((int)Projectile.ai[0] == 0) {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        if (Projectile.ai[0] < 12f) {
            Projectile.velocity *= 0.85f;
            if (Projectile.alpha > 0) {
                Projectile.alpha -= 36;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
        }
        if (Projectile.ai[0] < 18f) {
            Projectile.scale -= 1f / 18f;
            Projectile.position += new Vector2(2f);
            Projectile.width -= 2;
            Projectile.height -= 2;
        }
        Projectile.ai[0]++;

        Projectile.rotation += Projectile.velocity.Length() * 0.03f * Projectile.direction;
        bool collding = Projectile.ai[1] > 0f || Collision.SolidCollision(Projectile.position + Projectile.Size / 4f, Projectile.width / 2, Projectile.height / 2);
        if (collding) {
            Projectile.ai[1] = 1f;
            Projectile.alpha += 8;
            Projectile.velocity *= 0.8f;
        }
        else {
            Projectile.position += Main.player[Projectile.owner].velocity;
        }
        if (Projectile.timeLeft < 16) {
            Projectile.alpha += 16;
        }
        if (Projectile.alpha >= 255) {
            Projectile.Kill();
        }
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        //if (Projectile.velocity.X != oldVelocity.X) {
        //    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -oldVelocity.X, 0.75f);
        //}

        //if (Projectile.velocity.Y != oldVelocity.Y) {
        //    Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -oldVelocity.Y, 0.75f);
        //}

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * Slice.ProjectilePiercingPenalty);
        target.AddBuff(BuffID.Frostburn2, Slice.ProjectileDebuffDuration);
        SliceOnHitEffect.SpawnOnNPC(Projectile, target);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        target.AddBuff(BuffID.Frostburn, Slice.ProjectileDebuffDuration);
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var drawPosition = Projectile.Center;
        var drawOffset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
        lightColor = Projectile.GetAlpha(lightColor);
        var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
        frame.Height -= 2;
        var origin = frame.Size() / 2f;
        float opacity = Projectile.Opacity;

        Main.EntitySpriteDraw(texture, Projectile.position + drawOffset, frame, Projectile.GetAlpha(lightColor) * opacity, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);

        var swish = AequusTextures.SlashVanillaSmall;
        var swishFrame = swish.Frame(verticalFrames: 4);
        var swishColor = new Color(60, 120, 255, 0) * opacity;
        var swishOrigin = swishFrame.Size() / 2f;
        float swishScale = Projectile.scale * 1f;
        var swishPosition = Projectile.position + drawOffset;

        var flare = AequusTextures.Flare.Value;
        var flareOrigin = flare.Size() / 2f;
        float flareOffset = (swish.Width() / 2f - 4f) * Projectile.scale;
        var flareDirectionNormal = Vector2.Normalize(Projectile.velocity) * flareOffset;
        float flareDirectionDistance = 100f * Projectile.scale;
        var swishEffect = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        for (int i = 0; i < 2; i++) {
            float swishRotation = Projectile.rotation + MathHelper.Pi * i;
            Main.EntitySpriteDraw(swish, swishPosition, swishFrame, swishColor, swishRotation, swishOrigin, swishScale, swishEffect, 0);
            Main.EntitySpriteDraw(swish, swishPosition, swishFrame.Frame(0, 3), swishColor, swishRotation, swishOrigin, swishScale, swishEffect, 0);

            for (int j = 1; j < 2; j++) {
                var flarePosition = (swishRotation + 0.5f * Projectile.spriteDirection + 0.6f * (j - 1)).ToRotationVector2() * flareOffset;
                float flareIntensity = Math.Max(flareDirectionDistance - Vector2.Distance(flareDirectionNormal, flarePosition), 0f) / flareDirectionDistance;
                Main.EntitySpriteDraw(flare, swishPosition + flarePosition, null, swishColor * flareIntensity * 3f * 0.4f, 0f, flareOrigin, new Vector2(swishScale * 0.7f, swishScale * 2f) * flareIntensity, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(flare, swishPosition + flarePosition, null, swishColor * flareIntensity * 3f * 0.4f, MathHelper.PiOver2, flareOrigin, new Vector2(swishScale * 0.8f, swishScale * 2.5f) * flareIntensity, SpriteEffects.None, 0);
            }
        }
        return false;
    }

    public override void OnKill(int timeLeft) {
        if (Projectile.alpha < 200) {
            for (int i = 0; i < 30; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, newColor: new Color(80, 155, 255, 128), Scale: 2f);
                d.velocity *= 0.4f;
                d.velocity += Projectile.velocity * 0.5f;
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                d.scale *= Projectile.scale * 0.6f;
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
        }
    }
}