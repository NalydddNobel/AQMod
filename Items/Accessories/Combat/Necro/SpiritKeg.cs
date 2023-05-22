using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Necro {
    public class SpiritKeg : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.ghostLifespan += 3600;
            aequus.ghostSlotsMax++;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<SpiritBottle>()
                .AddIngredient<BloodiedBucket>()
                .AddIngredient(ItemID.Keg)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.PapyrusScarab);
        }
    }
}