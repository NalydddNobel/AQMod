using Aequus.Common.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.MagicMirrors.PhasePhone {
    public class PhasePhone : PhasePhoneHome {
        public override int ShellphoneClone => ItemID.ShellphoneDummy;
        public override int ShellphoneConvert => ModContent.ItemType<PhasePhoneHome>();

        public override void HoldItem(Player player) {
            Item.Transform(ShellphoneConvert);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddRecipeGroup(AequusRecipes.Shellphone)
                .AddIngredient<PhaseMirror.PhaseMirror>()
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterAfter(ItemID.Shellphone);
        }
    }
}