using Aequus.Content.Configuration;
using Aequus.Content.DataSets;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.VanillaChanges;

public class MagicConchChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveMagicConch;
    }

    public override void SetStaticDefaults() {
        LootDefinition.CreateFor(Loot.PollutedOceanPrimary, ItemID.MagicConch);
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type != ItemID.OasisCrate && item.type != ItemID.OasisCrateHard) {
            return;
        }

        // Sonic Toilet wasn't here
        foreach (var rule in itemLoot.Get(includeGlobalDrops: false)) {
            if (rule is AlwaysAtleastOneSuccessDropRule alwaysAtleastOneSuccess) {
                for (int i = 0; i < alwaysAtleastOneSuccess.rules.Length; i++) {
                    if (alwaysAtleastOneSuccess.rules[i] is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDropRule) {
                        for (int j = 0; j < oneFromOptionsDropRule.dropIds.Length; j++) {
                            if (oneFromOptionsDropRule.dropIds[j] == ItemID.MagicConch) {
                                ExtendArray.RemoveAt(ref oneFromOptionsDropRule.dropIds, j);
                                j--;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void ReplaceConch(Chest chest) {
        if (!VanillaChangesConfig.Instance.MoveMagicConch) {
            return;
        }

        foreach (var item in chest.Where(i => i.type == ItemID.MagicConch)) {
            item.Transform(WorldGen.genRand.NextBool() ? ItemID.SandstorminaBottle : ItemID.FlyingCarpet);
        }
    }
}
