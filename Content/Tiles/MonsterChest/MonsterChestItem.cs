using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.MonsterChest;

public class MonsterChestItem : ContentItem {
    public override string Texture => AequusTextures.Item(ItemID.GoldenChest);

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<MonsterChest>(), 1);
    }
}