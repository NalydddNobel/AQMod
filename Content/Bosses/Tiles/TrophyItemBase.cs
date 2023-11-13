using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Bosses.Tiles {
    public abstract class TrophyItemBase : ModItem {
        public abstract int TileStyle { get; }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<BossTrophiesTile>(), TileStyle);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}