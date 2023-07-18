using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Herbs.Moonflower {
    public class MoonflowerSeeds : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<MoonflowerTile>());
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 200);
        }
    }
}