using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.AshGraves
{
    public class AshTombstoneProj : TombstoneProjBase
    {
        public static Asset<Texture2D> Glow { get; private set; }

        public override void Load()
        {
            Glow = ModContent.Request<Texture2D>(this.GetPath() + "_Glow");
        }

        public override void Unload()
        {
            Glow = null;
        }

        public override int TileType => ModContent.TileType<Tombstones>();
        public override int TileStyle => Tombstones.AshTombstoneStyle;

        public override string GetTombstoneText()
        {
            return base.GetTombstoneText() + "\n" + AshTombstoneText();
        }

        public override void PostDraw(Color lightColor)
        {
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Glow.Value, drawCoordinates, frame, new Color(200, 100, 100, 0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 3f, 0.2f, 0.5f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}