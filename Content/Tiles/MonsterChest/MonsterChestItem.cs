namespace Aequus.Content.Tiles.MonsterChest;

public class MonsterChestItem : ModItem {
    public override string Texture => AequusTextures.Item(ItemID.GoldenChest);

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<MonsterChest>(), 1);
    }
}