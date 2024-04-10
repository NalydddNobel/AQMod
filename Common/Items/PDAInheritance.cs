using Aequus.Content.Configuration;
using Aequus.Content.VanillaChanges;
using Aequus.Core.Initialization;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Items;

public class PDAEffects : GlobalItem, IPostSetupRecipes {
    public static HashSet<int> PDAUpgrades { get; private set; } = new();

    public override void UpdateInfoAccessory(Item item, Player player) {
        if (PDAUpgrades.Contains(item.type)) {
            if (item.ModItem?.Mod == Aequus.Instance) {
                player.RefreshInfoAccsFromItemType(ContentSamples.ItemsByType[ItemID.PDA]);
            }

            if (GameplayConfig.Instance.PDAGetsAequusItems) {
                InfoAccessoryChanges.PDA.UpdateAequusInfoAccs(player);
            }
        }
    }

    public void PostSetupRecipes(Aequus aequus) {
        Aequus.Log.Debug("Scanning PDA recipe trees...");
        MoveUpTree(ItemID.PDA);
    }

    private void MoveUpTree(int itemId) {
        for (int i = 0; i < Recipe.numRecipes; i++) {
            Recipe recipe = Main.recipe[i];
            if (recipe == null || recipe.createItem == null || PDAUpgrades.Contains(recipe.createItem.type)
                || (recipe.requiredItem?.Any(i => i != null && i.type == itemId)) != true) {
                continue;
            }

            foreach (int types in ExtendItem.GetAllOverridesOfItemId(recipe.createItem.type)) {
                PDAUpgrades.Add(types);
                MoveUpTree(types);
            }
        }
    }
}
