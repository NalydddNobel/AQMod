using Aequus;
using Aequus.Content.DataSets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.Swords.Slice;

public class SliceBulletProj : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileSets.Pushable.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 180;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 20;
        Projectile.scale = 1f;
        //Projectile.tileCollide = false;
    }

    public override void AI() {
        if ((int)Projectile.ai[0] == 0) {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.ai[0]++;
        }

        if (Projectile.alpha > 40) {
            if (Projectile.extraUpdates > 0) {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.scale > 1f) {
                Projectile.scale -= 0.02f;
                if (Projectile.scale < 1f) {
                    Projectile.scale = 1f;
                }
            }
        }
        Projectile.velocity *= 0.99f;
        Projectile.rotation += Projectile.velocity.Length() * 0.03f * Projectile.direction;
        int size = 90;
        bool collding = Collision.SolidCollision(Projectile.position + new Vector2(size / 2f, size / 2f), Projectile.width - size, Projectile.height - size);
        if (collding) {
            Projectile.alpha += 8;
            Projectile.velocity *= 0.8f;
        }
        Projectile.alpha += 8;
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
        if (Projectile.velocity.X != oldVelocity.X) {
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -oldVelocity.X, 0.75f);
        }

        if (Projectile.velocity.Y != oldVelocity.Y) {
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -oldVelocity.Y, 0.75f);
        }

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * 0.5f);
        target.AddBuff(BuffID.Frostburn2, 480);
        SliceOnHitEffect.SpawnOnNPC(Projectile, target);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        target.AddBuff(BuffID.Frostburn2, 480);
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

        Main.EntitySpriteDraw(texture, Projectile.position + drawOffset, frame, Projectile.GetAlpha(lightColor) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

        var swish = AequusTextures.SlashVanillaSmall;
        var swishFrame = swish.Frame(verticalFrames: 4);
        var swishColor = new Color(60, 120, 255, 0) * opacity;
        var swishOrigin = swishFrame.Size() / 2f;
        float swishScale = Projectile.scale * 1f;
        var swishPosition = Projectile.position + drawOffset;

        var flare = AequusTextures.Flare.Value;
        var flareOrigin = flare.Size() / 2f;
        float flareOffset = swish.Width / 2f - 4f;
        var flareDirectionNormal = Vector2.Normalize(Projectile.velocity) * flareOffset;
        float flareDirectionDistance = 200f;
        for (int i = 0; i < 2; i++) {
            float swishRotation = Projectile.rotation + MathHelper.Pi * i;
            Main.EntitySpriteDraw(
                swish,
                swishPosition,
                swishFrame,
                swishColor,
                swishRotation,
                swishOrigin,
                swishScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(
                swish,
                swishPosition,
                swishFrame.Frame(0, 3),
                swishColor,
                swishRotation,
                swishOrigin,
                swishScale, SpriteEffects.None, 0);

            for (int j = 1; j < 2; j++) {
                var flarePosition = (swishRotation + 0.6f * (j - 1)).ToRotationVector2() * flareOffset;
                float flareIntensity = Math.Max(flareDirectionDistance - Vector2.Distance(flareDirectionNormal, flarePosition), 0f) / flareDirectionDistance;
                Main.EntitySpriteDraw(
                    flare,
                    swishPosition + flarePosition,
                    null,
                    swishColor * flareIntensity * 3f * 0.4f,
                    0f,
                    flareOrigin,
                    new Vector2(swishScale * 0.7f, swishScale * 2f) * flareIntensity, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(
                    flare,
                    swishPosition + flarePosition,
                    null,
                    swishColor * flareIntensity * 3f * 0.4f,
                    MathHelper.PiOver2,
                    flareOrigin,
                    new Vector2(swishScale * 0.8f, swishScale * 2.5f) * flareIntensity, SpriteEffects.None, 0);
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