using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class MiniDemonScythe : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DemonScythe);
            projectile.friendly = true;
            projectile.magic = false;
            projectile.melee = true;
            projectile.extraUpdates = 1;
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = 240;
            aiType = ProjectileID.DemonScythe;
        }

        public override void AI()
        {
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(4) && target.Distance(Main.player[projectile.owner].Center) < (ProjectileID.Sets.YoyosMaximumRange[projectile.type] / 2f))
                Buffs.Debuffs.CorruptionHellfire.Inflict(target, 360);
        }
    }
}