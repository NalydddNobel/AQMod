using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.Pyramid.Tiles {
    public class PyramidStatue : ModItem {
        public override string Texture => Aequus.PlaceholderItem;

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PyramidStatueTile>());
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class PyramidStatueTile : ModTile {
        public override string Texture => Aequus.PlaceholderFurniture;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Origin = new Point16(2, 6);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 18 };
            TileObjectData.addTile(Type);
            DustType = DustID.Stone;
            MinPick = 225;
            AddMapEntry(new Color(140, 103, 103), TextHelper.GetItemName<PyramidStatue>());
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            var tile = Main.tile[i, j];
            if (tile.TileFrameX == 54 && tile.TileFrameY == 108) {
                var moonTexture = TextureAssets.Moon[0].Value;
                var frame = moonTexture.Frame(verticalFrames: 8, frameY: Main.moonPhase);
                var drawPosition = this.GetDrawPosition(i, j) + Helper.TileDrawOffset + new Vector2(-16f, -66f);

                spriteBatch.Draw(
                    moonTexture,
                    drawPosition,
                    moonTexture.Frame(verticalFrames: 8, frameY: 0),
                    Color.Black * 0.5f,
                    0f,
                    frame.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );

                spriteBatch.Draw(
                    moonTexture,
                    drawPosition,
                    frame,
                    Color.White,
                    0f,
                    frame.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
            return true;
        }
    }
}