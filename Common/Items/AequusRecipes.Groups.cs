using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public partial class AequusRecipes {
    /// <summary>
    /// <see cref="RecipeGroup"/> for all IDs in <see cref="Main.anglerQuestItemNetIDs"/>.
    /// </summary>
    public static RecipeGroup AnyQuestFish { get; private set; }
    /// <summary>
    /// <see cref="RecipeGroup"/> for the Shellphone and its alternative modes.
    /// </summary>
    public static RecipeGroup Shellphone { get; private set; }
    /// <summary>
    /// <see cref="RecipeGroup"/> for trash items.
    /// </summary>
    public static RecipeGroup AnyPaints { get; private set; }
    /// <summary>
    /// <see cref="RecipeGroup"/> for paints.
    /// </summary>
    public static RecipeGroup AnyTrash { get; private set; }

    private int[] GetItems(Predicate<Item> predicate, bool vanillaOnly = false) {
        List<int> list = new();
        int count = vanillaOnly ? ItemID.Count : ItemLoader.ItemCount;
        for (int i = 0; i < count; i++) {
            if (predicate(ContentSamples.ItemsByType[i])) {
                list.Add(i);
            }
        }
        return list.ToArray();
    }

    public override void AddRecipeGroups() {
        AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.CloneArray());
        Shellphone = NewGroup("Shellphone", ItemID.ShellphoneDummy, ItemID.Shellphone, ItemID.ShellphoneHell, ItemID.ShellphoneOcean, ItemID.ShellphoneSpawn);
        AnyTrash = NewGroup("AnyTrash", ItemID.OldShoe, ItemID.FishingSeaweed, ItemID.TinCan);
        AnyPaints = NewGroup("AnyPaint", GetItems((i) => i.paint > PaintID.None));
    }

    private static RecipeGroup NewGroup(string name, params int[] items) {
        RecipeGroup group = new(() => Language.GetOrRegister("Mods.Aequus.Items.RecipeGroups." + name + ".DisplayName").Value, items);
        RecipeGroup.RegisterGroup(name, group);
        return group;
    }
}