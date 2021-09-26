using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class SpectreSoul : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + AQTextureAssets.None;

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.extraUpdates = 6;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            if (projectile.ai[0] > 3)
            {
                projectile.ai[0] = 0f;
                int target = -1;
                float closestDist = 2150f;
                var center = projectile.Center;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    var distance = Vector2.Distance(center, Main.npc[i].Center);
                    if (Main.npc[i].CanBeChasedBy() && distance < closestDist && AQNPC.AreTheSameNPC((int)projectile.ai[1], Main.npc[i].type))
                    {
                        target = i;
                        closestDist = distance;
                    }
                }
                if (target > -1)
                    projectile.velocity = Vector2.Lerp(projectile.velocity, new Vector2(2f, 0f).RotatedBy((Main.npc[target].Center - center).ToRotation()), 0.05f);
            }
        }

        public override void PostAI()
        {
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.MonoDust>(), 0f, 0f, 0, new Color(240, 90, 100, 0));
            Main.dust[d].velocity = projectile.velocity * 0.01f;
        }
    }
}