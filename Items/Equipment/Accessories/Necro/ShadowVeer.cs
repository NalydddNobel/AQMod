using Aequus.Common;
using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class ShadowVeer : ModItem {
        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().ghostShadowDash++;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 100)
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddTile(TileID.DemonAltar)
                .TryRegisterBefore(ItemID.MasterNinjaGear)
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .TryRegisterBefore(ItemID.MasterNinjaGear);
#endif
        }
    }
}