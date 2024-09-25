﻿using Aequus.Content.ItemPrefixes.Potions;
using Aequus.CrossMod;

namespace Aequus.Items.Potions.Pollen;
public class MistralPollen : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PixieDust);
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes() {
        var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<EmpoweredPrefix>());
        foreach (var pair in EmpoweredPrefix.ItemToEmpoweredBuff) {
            if (prefix.CanRoll(ContentSamples.ItemsByType[pair.Key])) {
                var r = Recipe.Create(pair.Key, 1)
                    .ResultPrefix<EmpoweredPrefix>()
                    .AddIngredient(pair.Key)
                    .AddIngredient(Type)
                    .DisableDecraft()
                    .TryRegisterAfter(pair.Key);
                MagicStorage.AddBlacklistedItemData(pair.Key, prefix.Type);
            }
        }
    }
}