using Aequus.Buffs.Debuffs;
using Aequus.Graphics.Prims;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class InsurgentSkull : NecromancerBolt
    {
        protected PrimRenderer smokePrim;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            this.SetTrail(15);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(90, 255, 255 - (int)AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0, 120), 200);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 2)
            {
                Projectile.velocity = Vector2.Zero;

                int npc = (int)Projectile.ai[1];
                if (npc > -1)
                {
                    if (!Main.npc[npc].active)
                    {
                        Projectile.ai[1] = -2f;
                    }
                    else
                    {
                        Projectile.Center = Main.npc[npc].Center;
                    }
                }
                if (Projectile.alpha < 40)
                {
                    Projectile.scale -= 0.2f;
                }
                else
                {
                    float add = Projectile.Opacity / 20f;
                    if (Projectile.alpha < 60)
                    {
                        add /= 2f;
                    }
                    Projectile.scale += 0.02f + add;
                }
                Projectile.alpha -= 5;
                if (Projectile.alpha < 128 && Main.netMode != NetmodeID.Server && Main.rand.NextBool())
                {
                    var normal = Main.rand.NextVector2Unit();
                    float distance = Projectile.width * Projectile.scale;
                    var d = Dust.NewDustPerfect(Projectile.Center + normal * distance, ModContent.DustType<MonoDust>(), -normal * distance / 16f, 
                        newColor: new Color(255 - Projectile.alpha, 255, 255 - Main.rand.Next((int)(120 * (1f - Projectile.Opacity))), 100));
                }
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    SpawnExtraSouls();
                    Projectile.Kill();
                }
                Projectile.frame = 1;
                Projectile.rotation = 0f;
                Projectile.tileCollide = false;
                return;
            }

            if ((int)Projectile.ai[0] == 1)
            {
                Projectile.velocity *= 0.6f;
                Projectile.alpha += 15;
                if (Projectile.alpha > 255)
                {
                    Projectile.scale -= 0.8f;
                    Projectile.alpha = 255;
                    if (Projectile.ai[1] != -1f)
                    {
                        Projectile.ai[0] = 2f;
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                    Projectile.frame = 1;
                    Projectile.rotation = 0f;
                }
                Projectile.tileCollide = false;
                return;
            }

            Projectile.ai[1] = -1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
            Projectile.frame = 0;
            Projectile.damage = 1;
        }
        public void SpawnExtraSouls()
        {
            var center = Projectile.Center;
            int count = 1;
            var source = Projectile.GetSource_Death("Aequus:InsurgentSpawn");
            int collisionChecks = 0;
            int distance = 20;
            if (Projectile.ai[1] > 0f)
            {
                distance = (int)(Main.npc[(int)Projectile.ai[1]].Size.Length() / 2f);
            }
        RecalcShootDir:
            var normal = Main.rand.NextVector2Unit();
            if (Collision.SolidCollision(Projectile.Center + normal * distance - new Vector2(5f), 10, 10))
            {
                collisionChecks++;
                if (collisionChecks > 50)
                {
                    goto ShootAtEnemies;
                }
                goto RecalcShootDir;
            }
            var p = Projectile.NewProjectileDirect(source, Projectile.Center + normal * distance, normal * 10f, ModContent.ProjectileType<InsurgentBolt>(), 0, Projectile.knockBack, Projectile.owner);
        ShootAtEnemies:
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].whoAmI != (int)Projectile.ai[1] && Main.npc[i].CanBeChasedBy(Projectile) && Projectile.Distance(Main.npc[i].Center) < 600f)
                {
                    count++;
                    normal = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.025f, 0.025f));
                    p = Projectile.NewProjectileDirect(source, Projectile.Center + normal * distance, normal * 10f, ModContent.ProjectileType<InsurgentBolt>(), 0, Projectile.knockBack, Projectile.owner);
                    if (count >= 4)
                    {
                        break;
                    }
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            NecromancyDebuff.ApplyDebuff<EnthrallingDebuff>(target, 3600, Projectile.owner, 100f);
            Projectile.damage = 0;
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.damage = 0;
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = -1f;
            Projectile.velocity = oldVelocity;
            Projectile.netUpdate = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

            primScale = 8f;
            primColor = drawColor;
            if ((int)Projectile.ai[0] != 2)
            {
                if (smokePrim == null)
                {
                    smokePrim = new PrimRenderer(Images.Trail[3].Value, PrimRenderer.DefaultPass, (p) => new Vector2(16f * Projectile.scale) * (1f - p), (p) => new Color(5, 60, 30, 235) * Projectile.Opacity * (1f - p), drawOffset: new Vector2(Projectile.width / 2f, Projectile.height / 2f));
                }
                for (int i = 0; i < 3; i++)
                    smokePrim.Draw(Projectile.oldPos);
                DrawTrail(maxLength: ProjectileID.Sets.TrailCacheLength[Type] / 3 * 2);
            }

            var bloom = Images.Bloom[0].Value;
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, drawColor * 0.5f, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.75f, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Projectile.alpha < 128)
            {
                var center = Projectile.Center;
                int amt = 2;
                if (Projectile.ai[1] != -1f)
                {
                    amt += 8;
                    SoundID.NPCDeath52?.PlaySound(Projectile.Center, 0.9f, -0.2f);
                }
                for (int i = 0; i < amt; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(), newColor: new Color(90, 255, 255 - Main.rand.Next(120), 100));
                    d.velocity *= 0.2f;
                    d.velocity += (d.position - center) / 8f;
                    d.scale += Main.rand.NextFloat(-0.5f, 0f);
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.6f, 1f);
                }
                for (int i = 0; i < amt * 3; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(90, 255, 255 - Main.rand.Next(120), 100));
                    d.velocity *= 0.2f;
                    d.velocity += (d.position - center) / 8f;
                    d.scale += Main.rand.NextFloat(-0.1f, 0.2f);
                }
            }
        }
    }
}