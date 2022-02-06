using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public sealed class TrapperMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
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
            projectile.manualDirectionChange = true;
            projectile.scale = 0.7f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.trapperImp = false;
            if (aQPlayer.trapperImp)
                projectile.timeLeft = 2;
            int target = -1;
            float distance = 1200f + 50f * (player.ownedProjectileCounts[projectile.type] - 1);

            if (player.HasMinionAttackTargetNPC)
            {
                int t = player.MinionAttackTargetNPC;
                float d = (Main.npc[t].Center - center).Length();
                if (d < distance)
                {
                    target = t;
                    distance = d;
                }
            }

            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(attacker: projectile, ignoreDontTakeDamage: false))
                    {
                        var difference = npc.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (!Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height))
                            c *= 12;
                        if (c < distance)
                        {
                            target = i;
                            distance = c;
                        }
                    }
                }
            }

            if ((player.Center - projectile.Center).Length() > 1000f)
            {
                projectile.Center = player.Center;
                projectile.velocity *= 0.1f;
            }
            else
            {
                AQProjectile.GetProjectileGroupStats_RotationalType(out int count, out int whoAmI, projectile);
                var gotoPosition = player.Center + new Vector2(0f,
                    -AQProjectile.GetSuggestedRadius(projectile, 128f + (float)Math.Sin(Main.time / 120f + whoAmI * 0.6f) * 15f))
                    .RotatedBy(AQProjectile.GetSuggestedRotation(projectile, count, whoAmI));
                projectile.velocity = Vector2.Lerp(projectile.velocity, (gotoPosition - center) / 4f, 0.05f);
                projectile.rotation = (projectile.Center - Main.player[projectile.owner].MountedCenter).ToRotation() + MathHelper.PiOver2;
            }

            if (target != -1)
            {
                projectile.ai[0]--;
                if (projectile.ai[0] <= 0f)
                {
                    projectile.ai[0] = 90f - Main.rand.Next(20);
                    projectile.localAI[0] = 30f;
                    Main.PlaySound(SoundID.Item38, projectile.Center);
                    if (Main.myPlayer == projectile.owner)
                    {
                        float projectileSpeed = 18f;
                        int projectileType = ModContent.ProjectileType<TrapperMinionBlast>();
                        var normal = Vector2.Normalize(Main.npc[target].Center - center);
                        projectile.velocity -= normal * (projectileSpeed / 3f);
                        Projectile.NewProjectile(center, normal * projectileSpeed, projectileType, projectile.damage, projectile.knockBack, projectile.owner, 100f);
                    }
                }
            }
            else
            {
                projectile.ai[0] = 0f;
            }

            if (projectile.localAI[0] > 0f)
            {
                projectile.frameCounter = 0;
                projectile.frame = 0;
                projectile.localAI[0]--;
            }
            else
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 5)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame >= Main.projFrames[projectile.type])
                        projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var playerCenter = Main.player[projectile.owner].MountedCenter;

            var drawPosition = projectile.Center;
            var texture = Main.projectileTexture[projectile.type];
            var frame = texture.Frame(verticalFrames: Main.projFrames[projectile.type], frameY: projectile.frame);
            var origin = frame.Size() / 2f;

            var chainTexture = ModContent.GetTexture(this.GetPath("_Chain"));
            var chainOrigin = new Vector2(chainTexture.Width / 2f, chainTexture.Height / 2f);
            int height = (int)(chainTexture.Height * projectile.scale);
            var chainEnd = drawPosition/* + new Vector2(origin.Y + chainOrigin.Y, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2)*/;
            var chainVelocity = chainEnd - playerCenter;
            int reps = (chainVelocity.Length() / height).RoundUp();
            chainVelocity.Normalize();
            chainVelocity *= height;
            float rotation = chainVelocity.ToRotation() + MathHelper.PiOver2;

            float wave = AQUtils.Wave(Main.GlobalTime * 10f, 0.9f, 1.1f);
            const int fade = 5;
            bool shouldFadeFromMouse = AQPlayer.TileImportantItem(Main.player[projectile.owner]);
            for (int j = 1; j < reps; j++)
            {
                var position = chainEnd - chainVelocity * j;
                var color = projectile.GetAlpha(lightColor);
                if (shouldFadeFromMouse)
                {
                    float lengthToMouse = (position - Main.MouseWorld).Length();
                    if (lengthToMouse < 160f)
                    {
                        float m = lengthToMouse / 160f;
                        color *= m * m; // squares in order to have a very steep curve
                    }
                }
                int J = reps - j;
                float lengthToPlayer = (position - playerCenter).Length();
                if (lengthToPlayer < 100f)
                {
                    float m = lengthToPlayer / 100f;
                    color *= m * m;
                    color.R = (byte)(int)Math.Min(color.R * 1.5f * wave, 255f);
                }
                else if (J > reps - fade)
                {
                    Main.spriteBatch.Draw(chainTexture, position - Main.screenPosition, null, color, rotation, chainOrigin, projectile.scale, SpriteEffects.None, 0f);
                    if (reps < fade * 2)
                    {
                        color *= 1f / fade * (fade - (fade * 2 - J)) * wave;
                    }
                    else
                    {
                        color *= 1f / fade * (fade - (reps - J)) * wave;
                    }
                    color.R = (byte)(int)Math.Min(color.R * 1.5f, 255f);
                    color.A = 0;
                }
                Main.spriteBatch.Draw(chainTexture, position - Main.screenPosition, null, color, rotation, chainOrigin, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}