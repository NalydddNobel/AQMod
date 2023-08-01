using Aequus.Common;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.SoulGem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class NaturesCruelty : ModItem {
        /// <summary>
        /// Default Value: 0.25
        /// </summary>
        public static float GhostHealthDR = 0.25f;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.NaturesGift)
                .AddIngredient<DemonicEnergy>()
                .AddIngredient<SoulGemFilled>(5)
                .AddTile(TileID.DemonAltar)
                .TryRegisterAfter(ItemID.ManaFlower);
#endif
        }
    }
}