using Aequus.Common;
using Aequus.Items.Materials.Hexoplasm;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class RitualisticSkull : ModItem {
        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accRitualSkull = true;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient<Hexoplasm>(5)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.PapyrusScarab);
#endif
        }
    }
}