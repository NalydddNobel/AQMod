using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class GoldPowder : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.velocity *= 0.95f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] == 180f)
            {
                projectile.Kill();
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 233, projectile.velocity.X, projectile.velocity.Y, 50);
                    Main.dust[d].noGravity = true;
                }
            }
            if (Main.myPlayer != projectile.owner)
            {
                return;
            }
            var myRect = projectile.getRect();
            for (int i = 0; i < 200; i++)
            {
                if (!Main.npc[i].active)
                {
                    continue;
                }
                if (new Rectangle((int)Main.npc[i].position.X, (int)Main.npc[i].position.Y, Main.npc[i].width, Main.npc[i].height).Intersects(myRect))
                {
                    if (AQNPC.ConvertNPCtoGold(i))
                    {
                        projectile.Kill();
                        break;
                    }
                }
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}