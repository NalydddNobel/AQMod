using Aequus.Core.Hooks;
using Aequus.Core.Entities.Tiles.Components;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.FloatingTrash;

public class FloatingTrashTile : FloatingTrashBase, ITouchEffects {
    public override string Texture => AequusTextures.Tile(TileID.Iron);

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        TerrariaHooks.OnRandomTileUpdate += OnRandomTileUpdate;
    }

    private static void OnRandomTileUpdate(int i, int j) {
        if (Main.dayTime && WorldGen.oceanDepths(i, j) && Main.tile[i, j - 1].LiquidAmount > 0 && TileHelper.ScanUp(new(i, j - 1), 100, out var result, TileHelper.HasNoLiquid) && !Framing.GetTileSafely(result.X, result.Y + 1).HasTile) {
            WorldGen.PlaceTile(result.X, result.Y + 1, ModContent.TileType<FloatingTrashTile>(), mute: true);
        }
    }

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
        Main.GetItemDrawFrame(randomItemTexture, out var itemTexture, out var itemFrame);

        var drawCoordinates = new Vector2(i * 16f + 8f + random.Next(-4, 4), j * 16f + Helper.Oscillate(Main.GlobalTimeWrappedHourly * random.NextFloat(0.5f, 1.12f), 5f, 11f) + TileHelper.GetWaterY(Main.tile[i, j].LiquidAmount)) - Main.screenPosition + TileHelper.DrawOffset;
        spriteBatch.Draw(itemTexture, drawCoordinates.Floor(), itemFrame, Lighting.GetColor(i, j), random.NextFloat(MathHelper.Pi), itemFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        return false;
    }
}