using Aequus.Items.Placeable.Furniture.Gravity;
using Aequus.Items.Placeable.Furniture.Oblivion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Oblivion
{
    public class OblivionChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DustType = DustID.Ash;
            ChestDrop = ModContent.ItemType<OblivionChest>();
            AddMapEntry(ColorHelper.FurnitureColor, CreateMapEntryName());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            try
            {
                var tile = Main.tile[i, j];
                int left = i;
                int top = j;
                if (tile.TileFrameX % 36 != 0)
                {
                    left--;
                }
                if (tile.TileFrameY != 0)
                {
                    top--;
                }
                int chest = Chest.FindChest(left, top);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + AequusHelpers.TileDrawOffset,
                    new Rectangle(tile.TileFrameX, 38 * (chest == -1 ? 0 : Main.chest[chest].frame) + tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            catch
            {
            }
        }
    }
}