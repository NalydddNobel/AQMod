﻿using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.Recipes;
using Aequus.Content.Items.Materials.PossessedShard;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.CriticalStrike;

public class SteakEyes : ModItem {
    /// <summary>
    /// Default Value: 0.15
    /// <para>This is only added on the first stack of the accessory.</para>
    /// </summary>
    public static float NotStackableCritDamage = 0.15f;
    /// <summary>
    /// Default Value: 0.1
    /// </summary>
    public static float StackableCritDamage = 0.1f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.Percent(NotStackableCritDamage + StackableCritDamage));

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetEntry(this, this.GetLocalization("Tooltip").WithFormatArgs(TextHelper.Create.Percent(NotStackableCritDamage + StackableCritDamage * 2f)));
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(24, 24);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1, silver: 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.Aequus();
        aequus.highSteaksDamage = Math.Max(aequus.highSteaksDamage, NotStackableCritDamage) + StackableCritDamage;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<HighSteaks>()
            .AddIngredient<PossessedShard>(5)
            .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }
}