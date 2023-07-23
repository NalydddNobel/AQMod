using Aequus.Common;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.SoulGem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class SouljointCuffs : ModItem {
        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().ghostChains++;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.Shackle)
                .AddIngredient<DemonicEnergy>()
                .AddIngredient<SoulGemFilled>(5)
                .AddTile(TileID.DemonAltar)
                .TryRegisterAfter(ItemID.MagicCuffs);
#endif
        }
    }
}