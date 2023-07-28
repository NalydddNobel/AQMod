using Aequus.Common.DataSets;
using Aequus.Common.Recipes;
using Aequus.Items.Materials.Gems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Building.OmniPaint {
    public class OmniPaint : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.RedPaint);
            Item.paint = 0;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 2);
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void UpdateInventory(Player player) {
            if (Main.myPlayer != player.whoAmI) {
                return;
            }

            var ui = ModContent.GetInstance<OmniPaintUI>();
            var heldItem = player.HeldItemFixed();
            if (!Main.playerInventory && heldItem != null && !heldItem.IsAir && ItemSets.IsPaintbrush.Contains(heldItem.type)) {
                ui.Enabled = true;
            }
            Item.paint = Math.Max(ui.SelectedPaint, (byte)1);
            Item.paintCoating = ui.SelectedCoating;
        }

        public override void AddRecipes() {
            CreateRecipe(999)
                .AddRecipeGroup(AequusRecipes.AnyPaints, 999)
                .AddIngredient<OmniGem>()
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}