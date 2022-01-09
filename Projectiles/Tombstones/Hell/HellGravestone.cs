using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using AQMod.Tiles.Furniture;

namespace AQMod.Projectiles.Tombstones.Hell
{
    public class HellGravestone : TombstoneType
    {
        public override int TileType => ModContent.TileType<AQTombstones>();
        public override int TileStyle => AQTombstones.HellCrossGravestone;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var drawCoordinates = projectile.Center - Main.screenPosition;
            var texture = Main.projectileTexture[projectile.type];
            var frame = texture.Frame(verticalFrames: Main.projFrames[projectile.type], frameY: projectile.frame);
            var origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(ModContent.GetTexture(this.GetPath("_Glow")), drawCoordinates, frame, new Color(200, 100, 100, 0) * AQUtils.Wave(Main.GlobalTime * 10f, 0.3f, 0.7f), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}