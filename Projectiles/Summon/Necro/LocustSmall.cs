using Aequus.Buffs.Debuffs;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class LocustSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = Aequus.NecromancyDamage;
            Projectile.aiStyle = -1;
            Projectile.alpha = 200;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            int target = Projectile.FindTargetWithLineOfSight(400f);
            if (target != -1)
            {
                if (Projectile.alpha == 0 || target != (int)Projectile.ai[1])
                {
                    Projectile.tileCollide = false;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 10f, 0.025f);
                }
            }
            else
            {
                if (Projectile.velocity.Length() < 5f)
                {
                    Projectile.velocity *= 1.05f;
                }
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            Projectile.LoopingFrame(4);
            Projectile.rotation = Projectile.velocity.X * 0.055f;
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Projectile.ai[1])
            {
                return Projectile.alpha <= 0 ? null : false;
            }
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            LocustDebuff.AddStack(target, 60);
        }
    }

    public class LocustLarge : LocustSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.netUpdate = true;
            LocustDebuff.AddStack(target, 120, 2);
        }
    }
}