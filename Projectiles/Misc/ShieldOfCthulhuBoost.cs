using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public sealed class ShieldOfCthulhuBoost : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            this.SetTrail(6);
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 120;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(85, 85, 85, 0);
        }

        public override void AI()
        {
            Projectile.direction = Main.player[Projectile.owner].direction;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.velocity.X = Projectile.direction;
            Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(Main.player[Projectile.owner].width / 2f * Main.player[Projectile.owner].direction * 1.25f, 0f);
            if (Main.player[Projectile.owner].velocity.Length() < 1f || Main.player[Projectile.owner].eocDash <= 0)
            {
                Projectile.alpha += 25;
                Projectile.scale -= 0.06f;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                    }
                }
                if (Main.rand.NextBool())
                {
                    var d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-50f, 50f) * Projectile.scale, ModContent.DustType<MonoSparkleDust>(), -Main.player[Projectile.owner].velocity * 0.5f, 0, new Color(200, 50, 50, 20), 1.5f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].cShield);
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            var effects = (-Projectile.spriteDirection).ToSpriteEffect();
            for (int i = 0; i < trailLength; i++)
            {
                float progress = AequusHelpers.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(60, 60, 60, 30) * Projectile.Opacity * progress, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + progress * 0.2f), effects, 0);
            }
            Main.EntitySpriteDraw(Images.Bloom[0].Value, Projectile.position + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * 0.8f, Projectile.rotation, Images.Bloom[0].Value.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}