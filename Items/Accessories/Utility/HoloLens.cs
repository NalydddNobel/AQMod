using Aequus.Common.Recipes;
using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class HoloLens : ModItem, ItemHooks.IUpdateVoidBag
    {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
            Item.rare = ItemRarityID.Orange;
        }


        public override void UpdateInventory(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                ChestLensInterface.Enabled = true;
        }

        void ItemHooks.IUpdateVoidBag.UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            if (Main.myPlayer == player.whoAmI)
                ChestLensInterface.Enabled = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.myPlayer == player.whoAmI)
                ChestLensInterface.Enabled = true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<PhaseMirror>());
        }
    }
}