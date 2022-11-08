using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class CarpenterWeaponProj : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.DirtBall;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] = Main.rand.Next(3);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 20f)
            {
                Projectile.velocity.X *= 0.995f;
                Projectile.velocity.Y += 0.25f;
            }
            Projectile.rotation += Projectile.direction * 0.15f;
            Projectile.spriteDirection = Projectile.direction;
        }

        public override void Kill(int timeLeft)
        {
            var point = Projectile.Center.ToTileCoordinates();
            if (!WorldGen.InWorld(point.X, point.Y, 20))
                return;
            var map = new TileMapCache(new Rectangle(point.X - 2, point.Y - 2, 5, 5));
            map.ClearArea();
            Main.tile[point].Active(value: true);
            Main.tile[point].TileType = (ushort)Projectile.ai[0];
            WorldGen.KillTile(point.X, point.Y);
            map.ResetArea();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > 0f)
            {
                int tileID = (int)Projectile.ai[0];

                Main.instance.LoadTiles(tileID);
                var t = TextureAssets.Tile[tileID].Value;
                var frame = new Rectangle(162 + 18 * (int)Projectile.localAI[0], 54, 16, 16);
                var origin = frame.Size() / 2f;

                var drawCoords = Projectile.Center - Main.screenPosition;
                Main.EntitySpriteDraw(t, drawCoords, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection.ToSpriteEffect(), 0);
                return false;
            }
            return true;
        }
    }
}