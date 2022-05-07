using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Summon
{
    public class InsurgentBolt : InsurgentSkull
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainbowCrystalExplosion;

        public override void SetStaticDefaults()
        {
            this.SetTrail(20);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.extraUpdates = 1;
            Projectile.scale = 0.1f;
            Projectile.alpha = 250;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(90, 255, 255 - (int)AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0, 120), 200);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 1)
            {
                Projectile.velocity *= 0.6f;
                Projectile.alpha += 15;
                if (Projectile.alpha > 255)
                {
                    Projectile.scale -= 0.8f;
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    Projectile.frame = 1;
                    Projectile.rotation = 0f;
                }
                Projectile.tileCollide = false;
                return;
            }

            Projectile.tileCollide = true;
            int target = Projectile.FindTargetWithLineOfSight(400f);
            if (target != -1 && target != (int)Projectile.ai[1])
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 10f, 0.05f);
            }
            else
            {
                if (Projectile.velocity.Length() < 10f)
                {
                    Projectile.velocity *= 1.05f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 6;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.scale < 0.5f)
            {
                Projectile.scale += 0.025f;
                if (Projectile.scale > 0.5f)
                {
                    Projectile.scale = 0.5f;
                }
            }
            if (Projectile.alpha == 0)
                Projectile.damage = 1;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
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