using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Bobbers
{
    public class StarcatcherBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            DrawOriginOffsetY = -8;
        }

        public override bool PreDrawExtras()
        {
            var player = Main.player[Projectile.owner];
            if (!Projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            float x = Main.player[Projectile.owner].direction == -1 ? -58f : 44f;
            Helper.DrawFishingLine(player, Projectile.position, Projectile.width / 2, Projectile.height, Projectile.velocity, Projectile.localAI[0], Main.player[Projectile.owner].Center + new Vector2(x, -28f),
                getLighting: (v, c) => Projectile.whoAmI % 2 == 0 ? new Color(100, 200, 255, 200) : new Color(255, 255, 25, 200));
            return false;
        }
    }
}