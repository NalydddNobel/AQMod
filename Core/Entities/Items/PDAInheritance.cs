using AequusRemake.Content.Configuration;
using AequusRemake.Systems.VanillaChanges;
using System.Collections.Generic;
using System.Linq;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Core.Entities.Items;

public class PDAEffects : GlobalItem, IPostSetupRecipes {
    public static HashSet<int> PDAUpgrades { get; private set; } = new();

    public override void UpdateInfoAccessory(Item item, Player player) {
        if (PDAUpgrades.Contains(item.type)) {
            if (item.ModItem?.Mod == AequusRemake.Instance) {
                player.RefreshInfoAccsFromItemType(ContentSamples.ItemsByType[ItemID.PDA]);
            }

            if (GameplayConfig.Instance.PDAGetsAequusRemakeItems) {
                InfoAccessoryChanges.PDA.UpdateAequusRemakeInfoAccs(player);
            }
        }
    }

    public void PostSetupRecipes(Mod mod) {
        Log.Debug("Scanning PDA recipe trees...");
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
