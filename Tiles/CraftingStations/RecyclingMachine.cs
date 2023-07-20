using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CraftingStations {
    public class RecyclingMachine : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<RecyclingMachineTile>());
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
        }
    }
}