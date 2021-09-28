using AQMod.Assets;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class CelesteTorusProjectile : ModProjectile
    {
        public override string Texture => "AQMod/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.netImportant = true;
            projectile.ignoreWater = true;
            projectile.manualDirectionChange = true;
        }

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            AQPlayer aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.dead || !aQPlayer.blueSpheres)
                projectile.active = false;
            else
                projectile.Center = player.Center;
            projectile.damage = aQPlayer.celesteTorusDamage;
            projectile.knockBack = aQPlayer.celesteTorusKnockback;
            projectile.scale = aQPlayer.celesteTorusScale;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
            {
                var pos = aQPlayer.GetCelesteTorusPositionOffset(i);
                var collisionCenter = player.Center + new Vector2(pos.X, pos.Y);
                if (Utils.CenteredRectangle(collisionCenter, new Vector2(projectile.width, projectile.width) * aQPlayer.celesteTorusScale).Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = target.position.X < Main.player[projectile.owner].position.X ? -1 : 1;
        }
    }
}