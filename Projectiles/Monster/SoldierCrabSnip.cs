using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class SoldierCrabSnip : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.timeLeft < 12)
            {
                projectile.scale -= 0.06f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }
    }
}