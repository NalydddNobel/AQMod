using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class PowPunch : ModProjectile
    {
        public static Vector2 PowRestingPosition(Projectile projectile, Player player)
        {
            return player.Center + new Vector2(projectile.width * 1.5f * player.direction * projectile.ai[1], -projectile.height);
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.manualDirectionChange = true;
        }

        private void Blast(Vector2 velocity)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PowPunchExplosion>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
            Vector2 position = projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;
            var bvelo = -velocity * 0.4f;
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(Main.projectile[p].position, bvelo * 0.2f, 61 + Main.rand.Next(3));
            }
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, 31);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
        }

        private void updateDirToPlayer(Player player)
        {
            int direction = projectile.position.X + projectile.width / 2f > player.position.X + player.width / 2f ? 1 : -1;
            player.ChangeDir(direction);
            projectile.direction = direction;
            projectile.spriteDirection = -direction;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead)
            {
                projectile.Kill();
                return;
            }
            Vector2 difference = player.MountedCenter - projectile.Center;
            float length = difference.Length();
            bool holding = player.channel || player.HeldItem.type == ModContent.ItemType<Items.Weapons.Melee.PowPunch>();
            projectile.timeLeft = 2;
            if ((int)projectile.ai[0] == 0)
            {
                player.itemAnimation = 10;
                player.itemTime = 10;
                if (projectile.localAI[0] > 0f)
                    projectile.localAI[0]--;
                if ((int)projectile.localAI[0] == 0)
                    updateDirToPlayer(player);
                int target = AQNPC.FindTarget(projectile.position, 200f / player.meleeSpeed);
                float maxLength = 260f;
                if (target != -1)
                {
                    maxLength *= 1.5f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.npc[target].Center - projectile.Center) * (player.HeldItem.shootSpeed / player.meleeSpeed), 0.02f / player.meleeSpeed);
                }
                projectile.tileCollide = true;
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X = projectile.velocity.X * 0.1f;
                Main.dust[d].velocity.Y = projectile.velocity.Y * 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(0.75f, 2f);
                if (Main.myPlayer == player.whoAmI && Main.mouseRight && Main.mouseRightRelease)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
                projectile.velocity.Y += 0.5f;
                if (length > maxLength || projectile.velocity.Length() < 15f && target == -1)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                    Blast(projectile.velocity);
                    projectile.velocity = -projectile.velocity * 1.5f;
                }
                else if (!player.channel)
                {
                    if (projectile.velocity.Y < 0f)
                        projectile.velocity.Y *= 0.9f;
                    projectile.velocity.Y += 1.5f;
                    projectile.velocity.X *= 0.98f;
                }
                projectile.rotation = difference.ToRotation() - MathHelper.PiOver2;
            }
            else if ((int)projectile.ai[0] == 1)
            {
                if (holding)
                {
                    projectile.tileCollide = false;
                    projectile.direction = player.direction;
                    projectile.spriteDirection = player.direction;
                    var gotoPosition = PowRestingPosition(projectile, player);
                    projectile.rotation = projectile.rotation.AngleLerp(0f, 0.025f);
                    difference = gotoPosition - projectile.Center;
                    length = difference.Length();
                    bool gotoPlayer = true;
                    if (length > 2000f)
                    {
                        projectile.Kill();
                        return;
                    }
                    if (length < 100f && player.ownedProjectileCounts[ModContent.ProjectileType<PowPunchExplosion>()] <= 0)
                    {
                        if (player.controlUseItem)
                        {
                            gotoPlayer = false;
                            if (Main.myPlayer == player.whoAmI)
                            {
                                projectile.ai[0] = 0f;
                                projectile.localAI[0] = 12f;
                                player.channel = true;
                                projectile.velocity = Vector2.Normalize(Main.MouseWorld - projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * (player.HeldItem.shootSpeed / player.meleeSpeed);
                                int direction = Main.MouseWorld.X > player.position.X + player.width / 2f ? 1 : -1;
                                player.ChangeDir(direction);
                                projectile.direction = direction;
                                projectile.spriteDirection = -direction;
                                projectile.netUpdate = true;
                                Main.PlaySound(SoundID.Item1, player.position);
                                for (int i = 0; i < 10; i++)
                                {
                                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity.X = projectile.velocity.X * 0.3f;
                                    Main.dust[d].velocity.Y = projectile.velocity.Y * 0.3f;
                                    Main.dust[d].scale = Main.rand.NextFloat(1f, 2.2f);
                                }
                                for (int i = 0; i < 4; i++)
                                {
                                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31);
                                    Main.dust[d].velocity = Vector2.Lerp(-projectile.velocity, Main.dust[d].velocity, 0.7f);
                                    Main.dust[d].noGravity = true;
                                }
                            }
                        }
                        else
                        {
                            int target = AQNPC.FindTarget(projectile.position, 100f);
                            if (target != -1)
                            {
                                gotoPlayer = false;
                                //projectile.ai[0] = 2f;
                                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.npc[target].Center - projectile.Center) * (10f / player.meleeSpeed), 0.08f);
                            }
                        }
                    }
                    if (gotoPlayer)
                    {
                        if (length < 10f)
                        {
                            projectile.velocity += player.velocity * 0.9f;
                            if (length < 2)
                            {
                                projectile.Center = gotoPosition;
                            }
                            else
                            {
                                projectile.velocity = Vector2.Normalize(difference) * (length / 2f);
                            }
                        }
                        else
                        {
                            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * (30f / player.meleeSpeed), 0.12f);
                        }
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Blast(projectile.velocity);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool hitEffect = false;
            if (oldVelocity.X != projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                    hitEffect = true;
                projectile.position.X += projectile.velocity.X;
                projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (oldVelocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                    hitEffect = true;
                projectile.position.Y += projectile.velocity.Y;
                projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            projectile.ai[0] = 1f;
            if (hitEffect)
            {
                projectile.netUpdate = true;
                Blast(oldVelocity);
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var playerCenter = player.MountedCenter;
            var chainTexture = ModContent.GetTexture(this.GetPath("_Chain"));
            int height = chainTexture.Height - 2;
            var velocity = projectile.position + offset - playerCenter;
            int length = (int)(velocity.Length() / height);
            velocity.Normalize();
            velocity *= height;
            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            var origin = new Vector2(chainTexture.Width / 2f, chainTexture.Height / 2f);
            for (int j = 1; j < length; j++)
            {
                var position = playerCenter + velocity * j;
                var color = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f));
                if (j < 6)
                    color *= 1f / 6f * j;
                Main.spriteBatch.Draw(chainTexture, position - Main.screenPosition, null, color, rotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            }

            var effects = projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            bool draw = true;
            var texture = this.GetTextureobj();
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            if (projectile.ai[0] == 0f)
            {
                float power = projectile.velocity.Length();

                if (power > 4f)
                {
                    float p = power - 4f;
                    int afterImages = 1 + (int)p;
                    if (afterImages > ProjectileID.Sets.TrailCacheLength[projectile.type])
                        afterImages = ProjectileID.Sets.TrailCacheLength[projectile.type];
                    p *= 10f;
                    byte minLight = (byte)(int)p;
                    if (lightColor.R < minLight)
                        lightColor.R = minLight;
                    if (lightColor.G < minLight)
                        lightColor.G = minLight;
                    if (lightColor.B < minLight)
                        lightColor.B = minLight;
                    var afterImageColor = new Color(lightColor.R / 255f * 2, lightColor.G / 255f * 1.5f, lightColor.B / 255f * 0.5f, 0.05f);
                    for (int i = 0; i < afterImages; i++)
                    {
                        float progress = 1f - 1f / afterImages * i;
                        Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, afterImageColor * (progress + 0.1f), rotation, origin, Math.Min(projectile.scale * progress + 0.1f, projectile.scale), effects, 0f);
                    }
                    Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, effects, 0f);
                    draw = false;
                    Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, afterImageColor, projectile.rotation, origin, projectile.scale * 1.1f, effects, 0f);
                }
            }
            if (draw)
                Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, lightColor, projectile.rotation, origin, 1f, effects, 0f);
            return false;
        }
    }
}
