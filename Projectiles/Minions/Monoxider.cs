using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Minions
{
    public class Monoxider : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 16;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 24;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 1;
        }

        public static Vector2 GetOffset(Player player, int carry)
        {
            float xOff = ((carry + 1) % 3 - 1) * (24 / 2);
            float yOff = -(carry / 3 * (20 / 2));
            return new Vector2(player.width * 0.5f + xOff * player.direction, -4 + yOff);
        }

        private void IncreaseStack(AQVisualsPlayer drawingPlayer)
        {
            drawingPlayer.hatMinionCarry++;
            if (drawingPlayer.hatMinionCarry == 0)
            {
                projectile.velocity = new Vector2(0f, 0f);
                projectile.frameCounter = 0;
                projectile.frame = 0;
                projectile.ai[0] = 1f;
                return;
            }
            int currentCarry = 1;
            projectile.velocity = new Vector2(0f, 0f);
            projectile.frameCounter = 0;
            projectile.frame = 0;
            for (int i = Main.maxProjectiles - 1; i >= 0; i--)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == projectile.type && (p.ai[0] > 0f || p.whoAmI == projectile.whoAmI))
                {
                    p.ai[0] = currentCarry;
                    currentCarry++;
                }
            }
        }

        private void Turn()
        {
            projectile.spriteDirection = projectile.velocity.X < 0f ? -1 : 1;
        }

        /// <summary>
        /// The seeing distance
        /// </summary>
        private const int C = 2000;

        public override bool MinionContactDamage() => projectile.ai[0] == 0f;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.monoxiderBird = false;
            if (aQPlayer.monoxiderBird)
                projectile.timeLeft = 2;
            int target = -1;
            float dist = C;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    var difference = npc.Center - center;
                    float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, projectile.position, projectile.width, projectile.height))
                        c *= 2; // enemies behind walls need to be 2x closer in order to be targeted
                    if (c < dist)
                    {
                        target = i;
                        dist = c;
                    }
                }
            }
            if (projectile.ai[0] > 0f)
            {
                if (target != -1)
                {
                    projectile.hide = false;
                    projectile.ai[0] = 0f;
                    projectile.direction = player.direction;
                    projectile.spriteDirection = player.direction;
                    projectile.frame = 1;
                }
                else
                {
                    projectile.Center = player.position + GetOffset(player, (int)projectile.ai[0] - 1);
                    projectile.spriteDirection = player.direction;
                    return;
                }
            }
            else if (projectile.ai[0] < 0f)
            {
                if (projectile.localAI[0] < 4)
                {
                    projectile.localAI[0] = ProjectileID.Sets.TrailCacheLength[projectile.type];
                }
                else if (projectile.localAI[0] > 2)
                {
                    projectile.localAI[0]--;
                }
                Vector2 gotoPos = player.position + GetOffset(player, drawingPlayer.hatMinionCarry);
                Vector2 difference = gotoPos - projectile.Center;
                float distance = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                if (distance > 15f)
                {
                    if (projectile.ai[1] < 32f)
                        projectile.ai[1] += 1.5f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * (projectile.ai[1] + player.velocity.Length() * 0.5f), 0.65f);
                    Turn();
                }
                else
                {
                    projectile.hide = true;
                    projectile.Center = player.position + GetOffset(player, (int)projectile.ai[0] - 1);
                    projectile.spriteDirection = player.direction;
                    IncreaseStack(drawingPlayer);
                }
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31);
                Main.dust[d].velocity = (projectile.velocity + Main.dust[d].velocity * 0.5f) * 0.05f;
                return;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6 - projectile.velocity.Length() / 4f)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 1;
            }
            if (target == -1)
            {
                Vector2 gotoPos = player.position + GetOffset(player, drawingPlayer.hatMinionCarry);
                Vector2 difference = gotoPos - projectile.Center;
                float distance = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                if (distance > 100f)
                {
                    if (projectile.ai[1] < 20f)
                        projectile.ai[1] += 1f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * projectile.ai[1], 0.1f);
                    Turn();
                }
                else if (distance > 5f)
                {
                    if (projectile.ai[1] > 6f)
                    {
                        projectile.ai[1] -= 0.5f;
                    }
                    else
                    {
                        projectile.ai[1] = 6f;
                    }
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * projectile.ai[1], 0.1f) + player.velocity * 0.1f;
                    Turn();
                }
                else
                {
                    projectile.hide = true;
                    IncreaseStack(drawingPlayer);
                }
            }
            else
            {
                Vector2 gotoPos = Main.npc[target].Center;
                Vector2 difference = gotoPos - projectile.Center;
                float distance = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                if (distance > 100f)
                {
                    if (projectile.ai[1] < 20f)
                        projectile.ai[1] += 1f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * projectile.ai[1], 0.1f) + Main.npc[target].velocity * 0.1f;
                    Turn();
                }
                else if (distance > 5f)
                {
                    if (projectile.ai[1] > 6f)
                    {
                        projectile.ai[1] -= 0.5f;
                    }
                    else
                    {
                        projectile.ai[1] = 6f;
                    }
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * projectile.ai[1], 0.1f) + Main.npc[target].velocity * 0.1f;
                    Turn();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.ai[0] = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            Rectangle frame = new Rectangle(0, projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 center = new Vector2(projectile.width / 2, projectile.height / 2);
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color((int)(lightColor.G * 1.5f), (int)(lightColor.G * 0.5f), (int)(lightColor.B * 0.5f), 128);
            if (projectile.ai[0] == -1 && projectile.localAI[0] >= 2)
            {
                for (int i = 0; i < projectile.localAI[0]; i++)
                {
                    float progress = 1f / projectile.localAI[0] * i;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + center - Main.screenPosition, frame, Color.Lerp(color, new Color(10, 0, 0, 0), progress), projectile.rotation, frame.Size() / 2f, 1f, effects, 0f);
                }
            }
            else
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    float progress = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type] * i;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + center - Main.screenPosition, frame, Color.Lerp(color, new Color(10, 0, 0, 0), progress), projectile.rotation, frame.Size() / 2f, 1f, effects, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, projectile.position + center - Main.screenPosition, frame, lightColor, projectile.rotation, frame.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}