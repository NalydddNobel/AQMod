using AQMod.Common.WorldGeneration;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Pets
{
    public sealed class DwarfStaritePet : ModProjectile
    {
        public override string Texture => AQUtils.GetPath<DwarfStarite>();

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 215);
        }

        private void AI_RandVelocity()
        {
            int tileHeight = 0;
            int tileX = ((int)projectile.position.X + projectile.width) / 16;
            int tileY = ((int)projectile.position.Y + projectile.height) / 16;
            for (int i = 0; i < 10; i++)
            {
                if (AQWorldGen.ActiveAndSolid(tileX, tileY + i))
                {
                    tileHeight = i + 1;
                    break;
                }
            }
            if (tileHeight == 10)
            {
                projectile.ai[0] = 0.5f;
            }
            else
            {
                if ((int)projectile.ai[1] <= 0)
                {
                    projectile.ai[0] = Main.rand.NextFloat(-1f, 1f);
                    projectile.ai[1] = Main.rand.Next(20, 80);
                }
                else
                {
                    if (projectile.ai[1] > 80f)
                    {
                        projectile.ai[1] = 80f;
                    }
                    projectile.ai[1]--;
                }
            }
            if ((int)projectile.localAI[1] <= 0)
            {
                projectile.localAI[0] = Main.rand.NextFloat(-2f, 2f);
                projectile.localAI[1] = Main.rand.Next(120, 600);
            }
            else
            {
                projectile.localAI[1]--;
            }
            projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, projectile.localAI[0], 0.05f);
            projectile.velocity.Y = MathHelper.Lerp(projectile.velocity.Y, projectile.ai[0], 0.05f);
        }
        private void AI_SpinChance()
        {
            if (projectile.frameCounter == 0 && Main.rand.NextBool(400))
                projectile.frameCounter = 1;
        }
        private void AI_UpdateDust()
        {
            if (projectile.frameCounter != 0)
            {
                if (Main.rand.NextBool(10))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
                    Main.dust[d].velocity = projectile.velocity * 0.01f;
                    Main.dust[d].noLight = true;
                }
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 58);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].noLight = true;
                }
                if (Main.rand.NextBool(20))
                {
                    int g = Gore.NewGore(projectile.position + new Vector2(Main.rand.Next(projectile.width - 4), Main.rand.Next(projectile.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.6f;
                }
            }
            else
            {
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
                    Main.dust[d].velocity = projectile.velocity * 0.1f;
                    Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
                    Main.dust[d].noLight = true;
                }
                if (Main.rand.NextBool(120))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 58);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-1f, 1f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-1f, 1f);
                    Main.dust[d].scale = Main.rand.NextFloat(0.4f, 0.8f);
                    Main.dust[d].noLight = true;
                }
                if (Main.rand.NextBool(120))
                {
                    int g = Gore.NewGore(projectile.position + new Vector2(Main.rand.Next(projectile.width - 4), Main.rand.Next(projectile.height - 4)), new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.3f;
                }
            }
        }
        private void AI_UpdateFrame()
        {
            if (projectile.frameCounter != 0)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 3)
                {
                    projectile.frameCounter = 1;
                    projectile.frame++;
                    if (projectile.frame >= Main.projFrames[projectile.type])
                    {
                        projectile.frame = 0;
                        projectile.frameCounter = 0;
                    }
                }
            }
            else
            {
                projectile.frame = 0;
            }
        }
        public override void AI()
        {
            AQProjectile.UpdateProjActive(projectile, ref Main.player[projectile.owner].GetModPlayer<AQPlayer>().dwarfStarite);
            float distance = projectile.Distance(Main.player[projectile.owner].Center);
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                distance += 300f;
            }
            if (distance < 100f)
            {
                if (projectile.velocity.Length() > 5f)
                {
                    float plrSpeed = Main.player[projectile.owner].velocity.Length();
                    if (plrSpeed > 4f && projectile.velocity.Length() > plrSpeed)
                    {
                        projectile.velocity *= 0.96f;
                    }
                    else
                    {
                        projectile.velocity *= 0.95f;
                    }
                }
                AI_RandVelocity();
                projectile.rotation += projectile.velocity.X * 0.004f;
            }
            else
            {
                if (projectile.rotation > MathHelper.TwoPi)
                {
                    projectile.rotation %= MathHelper.TwoPi;
                }
                if (projectile.rotation < -MathHelper.TwoPi)
                {
                    projectile.rotation %= MathHelper.TwoPi;
                }
                projectile.rotation *= 0.99f;
                projectile.ai[0] *= 0.9f;
                projectile.localAI[0] *= 0.9f;
                if (distance > 2000f)
                {
                    projectile.Center = Main.player[projectile.owner].Center;
                }
                var gotoVeloc = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * (distance / 1600f);
                float maxVelo = 20f;
                float plrSpeed = Main.player[projectile.owner].velocity.Length();
                if (plrSpeed > maxVelo)
                {
                    maxVelo = plrSpeed;
                }
                if (distance > 320f)
                {
                    float amount = 0.1f;
                    if (distance > 420f)
                    {
                        amount += MathHelper.Clamp((distance - 420f) / 200f, 0f, 0.5f);
                    }
                    if (Main.player[projectile.owner].velocity.Length() < 4f)
                    {
                        amount *= 2.5f;
                        amount = Math.Max(amount, 0.25f);
                        maxVelo /= 4f;
                    }
                    amount = Math.Min(amount, 0.75f);
                    //Main.NewText(distance);
                    //Main.NewText(amount);
                    //Main.NewText(maxVelo);
                    projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, gotoVeloc, amount)) * projectile.velocity.Length();
                }
                maxVelo = MathHelper.Max(maxVelo, 4f);
                float speed = projectile.velocity.Length();
                if (speed < maxVelo)
                {
                    projectile.velocity += gotoVeloc;
                }
                else if (speed > maxVelo + 1f)
                {
                    projectile.velocity *= 0.99f;
                }
            }
            AI_SpinChance();
            AI_UpdateDust();
            AI_UpdateFrame();
            Lighting.AddLight(projectile.Center, new Vector3(0.5f, 0.5f, 0.1f) * AQUtils.Wave(Main.GlobalTime * 10f, 0.9f, 1.1f));
        }
    }
}