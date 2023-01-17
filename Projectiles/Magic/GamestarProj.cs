using Aequus.Buffs.Debuffs;
using Aequus.Content;
using Aequus.Graphics.RenderTargets;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class GamestarProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetStaticDefaults()
        {
            this.SetTrail(10);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.alpha = 200;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] > 0f)
            {
                int target = (int)Projectile.ai[0] - 1;
                if ((int)Projectile.ai[1] <= 0)
                {
                    Projectile.ai[1] = Main.rand.Next(1, 5) * 10;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.ai[1]--;
            }
            Projectile.velocity *= 0.9975f;
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            ScreenCulling.SetPadding(20);
            if (!ScreenCulling.OnScreenWorld(Utils.CenteredRectangle(Projectile.Center, new Vector2(100f))))
            {
                return;
            }
            GamestarRenderer.Particles.Add(new GamestarParticle(Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f), Color.White, 8));

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Main.rand.NextFloat(1f) < 0.33f)
                    continue;
                GamestarRenderer.Particles.Add(new GamestarParticle(Projectile.oldPos[i] + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height)), Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f), Color.White, Main.rand.Next(1, 7)));
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate == 0)
            {
                return true;
            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public void SpawnParticles(Entity target)
        {
            ScreenCulling.SetPadding(200);
            if (!ScreenCulling.OnScreenWorld(Utils.CenteredRectangle(Projectile.Center, new Vector2(100f))))
            {
                return;
            }

            int amt = Math.Max((target.width + target.height) / 30, 5);
            for (int i = 0; i < amt; i++)
            {
                GamestarRenderer.Particles.Add(new GamestarParticle(target.Center + new Vector2(Main.rand.NextFloat(-target.width, target.width), Main.rand.NextFloat(-target.height, target.height)),
                    Main.rand.NextVector2Unit(), Color.White, 20));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BitCrushedDebuff>(), 240);
            if (Projectile.ai[0] + 1f <= target.whoAmI)
            {
                Projectile.ai[0] = target.whoAmI + 1;
                Projectile.timeLeft += 30;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                SpawnParticles(target);
            }
            for (int i = (int)Projectile.ai[0]; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile))
                {
                    Projectile.velocity = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                    return;
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile))
                {
                    Projectile.velocity = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                    Projectile.ai[0] = 0f;
                    break;
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SpawnParticles(target);
            }
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SpawnParticles(target);
            }
        }
    }
}