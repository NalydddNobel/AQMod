using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Pets
{
    public class AnglerFish : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.scale = 0.86f;
        }

        public Color LightColor()
        {
            return new Color(255, 250, 60, 255);
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            AQPlayer aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.dead)
                aQPlayer.anglerFish = false;
            if (aQPlayer.anglerFish)
                projectile.timeLeft = 2;
            Vector2 gotoPos = player.Center + new Vector2(-player.width * 3f * player.direction, -player.height * 1.5f);
            var center = projectile.Center;
            float distance = (center - gotoPos).Length();
            if (distance < projectile.width * 2)
            {
                projectile.velocity *= 0.99f;
            }
            else if (distance < 2000f)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(gotoPos - center) * 12f, 0.025f);
            }
            else
            {
                projectile.Center = player.Center;
                projectile.velocity *= 0.5f;
            }
            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.frameCounter++;
            if (projectile.frame > 1 && projectile.frame < 4)
            {
                projectile.frameCounter++;
            }
            if (projectile.frameCounter > 7)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
            Vector3 lightColor = Main.player[projectile.owner].cLight > 0
                ? DyeHelper.ModifyLight(GameShaders.Armor.GetSecondaryShader(Main.player[projectile.owner].cLight, Main.player[projectile.owner]), LightColor().ToVector3())
                : LightColor().ToVector3();
            Lighting.AddLight(projectile.Center, lightColor);
            projectile.spriteDirection = projectile.velocity.X < 0f ? 1 : -1;
        }
    }
}