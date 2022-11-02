using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Friendly
{
    public class ShieldOfCthulhuBoost : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            this.SetTrail(20);
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 180;
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
            return new Color(255, 255, 255, 128);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.scale = 0.1f;
            }
            if (Projectile.numUpdates == -1)
                Projectile.ai[0]++;
            var player = Main.player[Projectile.owner];
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Main.player[Projectile.owner].buffTime[i] > 4 && Main.debuff[player.buffType[i]])
                {
                    Main.player[Projectile.owner].buffTime[i] -= 2;
                    if (Main.player[Projectile.owner].buffTime[i] < 4)
                    {
                        Main.player[Projectile.owner].buffTime[i] = 4;
                    }
                }
            }
            Projectile.direction = Main.player[Projectile.owner].direction;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.velocity.X = Projectile.direction;
            Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(Main.player[Projectile.owner].width / 2f * Main.player[Projectile.owner].direction * 1.25f, 0f);
            float velocityLength = Main.player[Projectile.owner].velocity.Length();
            if (Main.player[Projectile.owner].eocDash > 10 && Math.Sign(Main.player[Projectile.owner].velocity.X) == Main.player[Projectile.owner].direction)
            {
                float minVelocity = 10f;
                if (Main.player[Projectile.owner].velocity.Y.Abs() > 1f)
                {
                    minVelocity = 5f;
                }
                if (velocityLength < minVelocity)
                {
                    velocityLength = minVelocity;
                    Main.player[Projectile.owner].velocity = Vector2.Normalize(Main.player[Projectile.owner].velocity).UnNaN() * minVelocity;
                }
                Projectile.timeLeft = Math.Max(Projectile.timeLeft, 60);
            }
            if (velocityLength < 1f || Main.player[Projectile.owner].eocDash <= 0)
            {
                Projectile.alpha += 25;
                Projectile.scale -= 0.06f - Projectile.scale * 0.02f;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.scale < 1f)
                {
                    Projectile.scale += 0.1f + Projectile.scale * 0.2f;
                    if (Projectile.scale > 1f)
                    {
                        Projectile.scale = 1f;
                    }
                }
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 30;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                    }
                }
                if (Main.rand.NextBool())
                {
                    var d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-50f, 0f)) * Projectile.scale, ModContent.DustType<MonoSparkleDust>(), -Main.player[Projectile.owner].velocity * 0.5f, 0, new Color(200, 50, 50, 20), 1.5f);
                    d.noGravity = true;
                    d.velocity += Main.player[Projectile.owner].velocity * 0.6f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
                }
                if (Main.rand.NextBool())
                {
                    var d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(0f, 50f)) * Projectile.scale,
                        ModContent.DustType<MonoSparkleDust>(), -Main.player[Projectile.owner].velocity * 0.5f, 0, new Color(100, 255, 100, 20), Main.rand.NextFloat(0.6f, 1f));
                    d.noGravity = true;
                    d.velocity += Main.player[Projectile.owner].velocity * 0.6f;
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
            offset.Y += 1f + Main.player[Projectile.owner].gfxOffY;
            var effects = (-Projectile.spriteDirection).ToSpriteEffect();
            for (int i = 0; i < trailLength; i++)
            {
                if (trailLength < 10)
                {
                    float progress = AequusHelpers.CalcProgress(10, i);
                    Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(90, 90, 90, 30) * Projectile.Opacity * progress,
                        Projectile.oldRot[i], origin, Projectile.scale * (0.8f + progress * 0.2f), effects, 0);
                }
            }

            Main.EntitySpriteDraw(TextureCache.Bloom[0].Value, Projectile.position + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * 0.8f, Projectile.rotation, TextureCache.Bloom[0].Value.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(Projectile.scale, Projectile.scale * 1.25f), effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(Projectile.scale, Projectile.scale * 1.25f), effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * 0.5f * Projectile.Opacity, Projectile.rotation, origin, new Vector2(Projectile.scale * 1.25f, Projectile.scale), effects, 0);
            return false;
        }
    }
}