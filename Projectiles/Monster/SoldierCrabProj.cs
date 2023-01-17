using Aequus.Buffs.Debuffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class SoldierCrabProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (lightColor * 2f).UseA(128) * 0.7f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 12)
            {
                Projectile.scale -= 0.06f;
                Projectile.velocity *= 0.98f;
            }
            Projectile.velocity *= 0.995f;
            if (Main.rand.NextBool(5))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, -Projectile.velocity.X * 0.5f, -Projectile.velocity.Y * 0.5f, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 0.8f));
                d.velocity *= 0.2f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<PickBreak>(), 480);
            }
        }
    }
}