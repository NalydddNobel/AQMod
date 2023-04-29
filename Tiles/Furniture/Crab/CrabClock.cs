using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Crab {
    public class CrabClock : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<BaseWallClock>());
            Item.value = Item.buyPrice(gold: 1);
        }
    }

    public class CrabClockTile : BaseWallClock {
    }
}