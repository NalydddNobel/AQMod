using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon {
    public class PiranhaPlantFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AI()
        {
            Projectile.rotation = MathHelper.PiOver2 * (Main.GameUpdateCount % 48 / 6);
            if (Main.rand.NextBool(10))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, -Projectile.velocity.X, -Projectile.velocity.Y);
                d.noGravity = true;
                if (Main.rand.NextBool())
                {
                    d.fadeIn += d.scale + 0.2f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 240);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 240);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.noGravity = true;
                if (Main.rand.NextBool())
                {
                    d.fadeIn += d.scale + 0.2f;
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
    }
}