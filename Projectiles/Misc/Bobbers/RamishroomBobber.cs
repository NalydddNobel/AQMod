using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Bobbers
{
    public class RamishroomBobber : ModProjectile
    {
        private int ModifyShootHack;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            DrawOriginOffsetY = -8;
        }

        public override bool PreAI()
        {
            ModifyShootHack = Main.player[Projectile.owner].HeldItem.shoot;
            Main.player[Projectile.owner].HeldItem.shoot = Type;
            return true;
        }

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.15f, 0f));
        }

        public override void PostAI()
        {
            Main.player[Projectile.owner].HeldItem.shoot = ModifyShootHack;
        }

        public override bool PreDrawExtras()
        {
            var player = Main.player[Projectile.owner];
            if (!Projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            AequusHelpers.DrawFishingLine(player, Projectile.position, Projectile.width / 2, Projectile.height, Projectile.velocity, 
                Projectile.localAI[0], Main.player[Projectile.owner].Center + new Vector2(42f * Main.player[Projectile.owner].direction, -28f), new Color(100, 255, 50, 200));
            return false;
        }
    }
}