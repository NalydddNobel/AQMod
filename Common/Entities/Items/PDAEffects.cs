using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Entities.Items;

public sealed class PDAEffects : GlobalItem, IPostSetupRecipes {
    public static readonly HashSet<int> PDAUpgrades = [];

    public override void UpdateInfoAccessory(Item item, Player player) {
        if (PDAUpgrades.Contains(item.type)) {
            if (item.ModItem?.Mod == Aequus.Instance) {
                player.RefreshInfoAccsFromItemType(ContentSamples.ItemsByType[ItemID.PDA]);
            }

            //if (GameplayConfig.Instance.PDAGetsAequusItems) {
            //    InfoAccessoryChanges.PDA.UpdateAequusInfoAccs(player);
            //}
        }
    }

    void IPostSetupRecipes.PostSetupRecipes() {
#if DEBUG
        Mod.Logger.Debug("Scanning PDA recipe trees...");
#endif
        MoveUpTree(ItemID.PDA);
    }

    private void MoveUpTree(int itemId) {
        for (int i = 0; i < Recipe.numRecipes; i++) {
            Recipe recipe = Main.recipe[i];
            if (recipe == null || recipe.createItem == null || PDAUpgrades.Contains(recipe.createItem.type)
                || (recipe.requiredItem?.Any(i => i != null && i.type == itemId)) != true) {
                continue;
            }

            foreach (int types in Helper.GetAllOverridesOfItemId(recipe.createItem.type)) {
                PDAUpgrades.Add(types);
                MoveUpTree(types);
            }
        }
    }
}