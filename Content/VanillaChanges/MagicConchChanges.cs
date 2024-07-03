using Aequu2.Content.Configuration;
using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Content.VanillaChanges;

public class MagicConchChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveMagicConch;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.MagicConch;
    }

    public override void SetDefaults(Item entity) {
        entity.StatsModifiedBy.Add(Mod);
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
