using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public sealed class Starite : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 6;
            projectile.minionSlots = 1f;
            projectile.extraUpdates = 1;
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.stariteMinion = false;
            if (aQPlayer.stariteMinion)
                projectile.timeLeft = 2;

            if (!projectile.tileCollide)
            {
                var difference = Main.player[projectile.owner].Center - projectile.Center;
                if (difference.Length() < 10f || Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                {
                    projectile.tileCollide = true;
                }
                Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, difference, 0.1f)) * Math.Max(6f, projectile.velocity.Length());
                projectile.velocity = lerpedVelocity;
                return;
            }

            int target = -1;
            float dist = 2000f;
            if (player.HasMinionAttackTargetNPC)
            {
                var difference = projectile.Center - center;
                if ((float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y) < dist && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.position, projectile.width, projectile.height))
                {
                    target = player.MinionAttackTargetNPC;
                }
            }
            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];
                    if (NPC.CanBeChasedBy())
                    {
                        var difference = projectile.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.position, projectile.width, projectile.height))
                            c *= 12;
                        if (c < dist)
                        {
                            target = i;
                            dist = c;
                        }
                    }
                }
            }

            Vector2 gotoPosition = target == -1 ? player.Center : Main.npc[target].Center;

            if ((int)projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f)
                {
                    projectile.ai[1] = Main.rand.Next(30, 70);
                    projectile.netUpdate = true;
                }
                if (projectile.ai[1] == 1f)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
                float turnSpeed = MathHelper.Clamp(projectile.ai[1] / 10000f, 0f, 1f);
                projectile.ai[1]--;
                if (projectile.localAI[0] > 0)
                    projectile.localAI[0]--;
                if (turnSpeed != 0f)
                {
                    float length = projectile.velocity.Length();
                    Vector2 difference = gotoPosition - center;
                    Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, difference, turnSpeed)) * length;
                    projectile.velocity = lerpedVelocity;
                }
            }
            else if ((int)projectile.ai[0] == 1f)
            {
                if (projectile.localAI[0] == 0f && projectile.localAI[1] == 0f)
                {
                    var gotoVelo = new Vector2(Main.rand.NextFloat(4f, 6.5f) + 2f, 0f).RotatedBy((gotoPosition - center).ToRotation());
                    projectile.localAI[0] = gotoVelo.X;
                    projectile.localAI[1] = gotoVelo.Y;
                }
                else
                {
                    var gotoVelo = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                    float length = gotoVelo.Length();
                    projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, gotoVelo, 0.08f)) * length;
                    bool xCloseEnough = (projectile.velocity.X - gotoVelo.X).Abs() < 0.1f;
                    bool yCloseEnough = (projectile.velocity.Y - gotoVelo.Y).Abs() < 0.1f;
                    if (xCloseEnough && yCloseEnough)
                    {
                        projectile.velocity.X = gotoVelo.X;
                        projectile.velocity.Y = gotoVelo.Y;
                        projectile.ai[0] = 0f;
                        projectile.localAI[0] = 0f;
                        projectile.localAI[1] = 0f;
                        projectile.netUpdate = true;
                    }
                }
            }

            if (target == -1)
            {
                if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                {
                    projectile.tileCollide = false;
                }
            }
        }

        public override void PostAI()
        {
            float size = new Vector2(projectile.width, projectile.height).Length();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (projectile.whoAmI != i && Main.projectile[i].active && (Main.projectile[i].type == projectile.type || Main.projectile[i].type == projectile.type))
                {
                    var difference = projectile.Center - Main.projectile[i].Center;
                    float length = difference.Length();
                    if (length < size)
                    {
                        projectile.velocity += difference * 0.01f;
                    }
                }
            }
            if ((projectile.Center - Main.player[projectile.owner].Center).Length() > 2000f)
            {
                projectile.Center = Main.player[projectile.owner].Center;
            }
            if (Main.rand.NextBool(40))
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
                Main.dust[d].velocity = projectile.velocity * 0.01f;
                Main.dust[d].noGravity = true;
            }
            if (Main.rand.NextBool(80))
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 58);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].noGravity = true;
            }
            if (Main.rand.NextBool(80))
            {
                int g = Gore.NewGore(projectile.position + new Vector2(Main.rand.Next(projectile.width - 4), Main.rand.Next(projectile.height - 4)), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));
            projectile.rotation += projectile.velocity.Length() * 0.0157f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int dustAmount = damage / 10;
            if (dustAmount < 1)
            {
                dustAmount = 1;
            }
            if (crit)
            {
                dustAmount *= 2;
            }
            if (target.life > 0 && !target.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] && Main.rand.NextBool(12))
            {
                dustAmount *= 2;
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.Play(SoundType.NPCHit, "inflict_bluefire", target.Center, 0.8f);
                }
            }
            for (int i = 0; i < dustAmount; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.MonoSparkleDust>(),
                    Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(150, 170, 200, 100)).noGravity = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X.Abs() > 2f)
                    projectile.velocity.X = -oldVelocity.X * 0.8f;
                projectile.localAI[0] *= -0.8f;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y.Abs() > 2f)
                    projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                projectile.localAI[1] *= -0.8f;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 250);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 offset = new Vector2(projectile.width / 2, projectile.height / 2);
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color(10, 10, 150, 40);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                float progress = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type] * i;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, frame, color * (1f - progress), projectile.rotation, origin, Math.Max(projectile.scale * (1f - progress), 0.1f), effects, 0f);
            }

            float time = Main.GameUpdateCount / 240f + Main.GlobalTime * 0.04f;
            float globalTimeWrappedHourly2 = Main.GlobalTime;
            globalTimeWrappedHourly2 %= 5f;
            globalTimeWrappedHourly2 /= 2.5f;
            if (globalTimeWrappedHourly2 >= 1f)
            {
                globalTimeWrappedHourly2 = 2f - globalTimeWrappedHourly2;
            }
            globalTimeWrappedHourly2 = globalTimeWrappedHourly2 * 0.5f + 0.5f;

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition + new Vector2(0f, 8f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(30, 30, 80, 50), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            for (float f = 0f; f < 1f; f += 0.34f)
            {
                spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition + new Vector2(0f, 4f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(80, 80, 180, 127), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0f, 0.5f), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}