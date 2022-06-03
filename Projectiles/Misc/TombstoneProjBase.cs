using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public abstract class TombstoneProjBase : ModProjectile
    {
        public abstract int TileType { get; }
        public virtual int TileStyle { get; }
        public virtual string GetTombstoneText()
        {
            return Projectile.miscText;
        }

        public static string AshTombstoneText()
        {
            return AequusText.GetText("Deaths.AshTombstone." + Main.rand.Next(13));
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.knockBack = 12f;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X *= 0.98f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Projectile.velocity.Y += 0.2f;
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }
            int x = (int)((Projectile.position.X + Projectile.width / 2) / 16f);
            int y = (int)((Projectile.position.Y + Projectile.height - 4f) / 16f);
            if (Main.tile[x, y] == null || Main.tile[x, y].HasTile)
            {
                return;
            }
            WorldGen.PlaceTile(x, y, TileType, mute: false, forced: false, Projectile.owner, style: TileStyle);
            if (Main.tile[x, y].HasTile)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, x, y, TileType, TileStyle);
                }
                int sign = Sign.ReadSign(x, y);
                if (sign >= 0)
                {
                    Sign.TextSign(sign, GetTombstoneText());
                }
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = oldVelocity.X * -0.75f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Projectile.velocity.Y > 1.5)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.7f;
            }
            return false;
        }
    }
}