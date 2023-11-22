using Aequus;
using Aequus.Common.Tiles.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.FloatingTrash;

public class FloatingTrashTile : FloatingTrashBase, ITouchEffects {
    public override string Texture => AequusTextures.Tile(TileID.Iron);

    public void Touch(int i, int j, Player player, AequusPlayer aequusPlayer) {
        var tile = Main.tile[i, j];
        tile.HasTile = false;
        if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer == player.whoAmI) {
            NetMessage.SendTileSquare(-1, i, j);
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (!UpdateActive(i, j)) {
            return false;
        }

        var random = Helper.RandomTileCoordinates(i, j);
        var randomItemTexture = random.Next(ItemLoader.ItemCount);
        ItemHelper.GetItemDrawData(randomItemTexture, out var itemFrame);
        
        var drawCoordinates = new Vector2(i * 16f + 8f + random.Next(-4, 4), j * 16f + Helper.Oscillate(Main.GlobalTimeWrappedHourly * random.NextFloat(0.5f, 1.12f), 5f, 11f) + TileHelper.GetWaterY(Main.tile[i,j].LiquidAmount)) - Main.screenPosition + TileHelper.DrawOffset;
        spriteBatch.Draw(TextureAssets.Item[randomItemTexture].Value, drawCoordinates.Floor(), itemFrame, Lighting.GetColor(i, j), random.NextFloat(MathHelper.Pi), TextureAssets.Item[randomItemTexture].Size() / 2f, 1f, SpriteEffects.None, 0f);
        return false;
    }
}