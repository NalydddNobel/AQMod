using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public sealed class DoubleShotProj : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Bullet;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Bullet);
            projectile.light = 0f;
            projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            projectile.hide = false;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            if ((int)projectile.ai[0] == 0)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                int p = Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 1f);
                Main.projectile[p].rotation = Main.projectile[p].velocity.ToRotation() + MathHelper.PiOver2;
                Main.projectile[p].extraUpdates = projectile.extraUpdates;
                projectile.ai[0] = 1f;
            }
            Lighting.AddLight(projectile.position, new Vector3(0.2f, 0.1f, 0f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            Collision.HitTiles(projectile.position, oldVelocity, projectile.width, projectile.height);
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.extraUpdates);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.extraUpdates = reader.ReadInt32();
        }
    }
}