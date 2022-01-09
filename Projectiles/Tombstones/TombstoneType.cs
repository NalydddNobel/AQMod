using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AQMod.Projectiles.Tombstones
{
    public abstract class TombstoneType : ModProjectile
    {
        public abstract int TileType { get; }
        public virtual int TileStyle { get; }
        public virtual string GetTombstoneText()
        {
            return projectile.miscText;
        }

        public override void SetDefaults()
        {
			projectile.width = 24;
			projectile.height = 24;
			projectile.aiStyle = -1;
			projectile.penetrate = -1;
			projectile.knockBack = 12f;
		}

		public override void AI()
        {
			if (projectile.velocity.Y == 0f)
			{
				projectile.velocity.X *= 0.98f;
			}
			projectile.rotation += projectile.velocity.X * 0.1f;
			projectile.velocity.Y += 0.2f;
			if (projectile.owner != Main.myPlayer)
			{
				return;
			}
			int x = (int)((projectile.position.X + projectile.width / 2) / 16f);
			int y = (int)((projectile.position.Y + projectile.height - 4f) / 16f);
			if (Main.tile[x, y] == null || Main.tile[x, y].active())
			{
				return;
			}
			WorldGen.PlaceTile(x, y, TileType, mute: false, forced: false, projectile.owner, style: TileStyle);
			if (Main.tile[x, y].active())
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.TileChange, -1, -1, null, 1, x, y, TileType, TileStyle);
				}
				int sign = Sign.ReadSign(x, y);
				if (sign >= 0)
				{
					Sign.TextSign(sign, GetTombstoneText());
				}
				projectile.Kill();
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = oldVelocity.X * -0.75f;
			}
			if (projectile.velocity.Y != oldVelocity.Y && projectile.velocity.Y > 1.5)
			{
				projectile.velocity.Y = oldVelocity.Y * -0.7f;
			}
			return false;
        }
    }
}