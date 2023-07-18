using Aequus.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.Necro {
    public class RitualisticSkull : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

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
            CreateRecipe()
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient<Hexoplasm>(5)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.PapyrusScarab);
        }
    }
}