using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Fishing
{
    public class CrabRod : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BobberWooden);
            drawOriginOffsetY = -8;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            var player = Main.player[projectile.owner];
            if (!projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            AQGraphics.Rendering.FishingLine(new Color(175, 146, 146, 255), player, projectile.position, projectile.width, projectile.height, projectile.velocity, projectile.localAI[0], new Vector2(45f, 39f));
            return false;
        }
    }
}