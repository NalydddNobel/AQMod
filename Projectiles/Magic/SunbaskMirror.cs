using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AQMod.Projectiles.Magic
{
    public class SunbaskMirror : mirrorProjectileType
    {
        protected override float LaserLength => 1000f;
        protected override Color LaserColor => new Color(255, 206, 115, 0);

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.magic = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 120);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float scale = getLaserScaleFromRot(projectile.rotation);
            damage = (int)(damage * scale);
            knockback *= scale;
            if (!crit && AQPlayer.PlayerCrit((int)scale * 100, Main.rand))
            {
                crit = true;
            }
            hitDirection = target.position.X + target.width / 2f < Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2f ? -1 : 1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}