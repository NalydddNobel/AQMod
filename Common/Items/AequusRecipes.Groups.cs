using System;
using System.Collections.Generic;
using Terraria.Localization;

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
    /// <see cref="RecipeGroup"/> for paints.
    /// </summary>
    public static RecipeGroup AnyPaints { get; private set; }
    /// <summary>
    /// <see cref="RecipeGroup"/> for coatings.
    /// </summary>
    public static RecipeGroup AnyCoatings { get; private set; }
    /// <summary>
    /// <see cref="RecipeGroup"/> for trash items.
    /// </summary>
    public static RecipeGroup AnyTrash { get; private set; }

    public override void AddRecipeGroups() {
        AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.Copy());
        Shellphone = NewGroup("Shellphone", ItemID.ShellphoneDummy, ItemID.Shellphone, ItemID.ShellphoneHell, ItemID.ShellphoneOcean, ItemID.ShellphoneSpawn);
        AnyTrash = NewGroup("AnyTrash", ItemID.OldShoe, ItemID.FishingSeaweed, ItemID.TinCan);
        AnyPaints = NewGroup("AnyPaint", GetItems((i) => i.paint > PaintID.None));
        AnyCoatings = NewGroup("AnyCoatings", GetItems((i) => i.paintCoating > PaintCoatingID.None));
    }

    private static RecipeGroup NewGroup(String name, params Int32[] items) {
        RecipeGroup group = new RecipeGroup(() => Language.GetOrRegister("Mods.Aequus.Items.RecipeGroups." + name + ".DisplayName").Value, items);
        RecipeGroup.RegisterGroup(name, group);
        return group;
    }

    private static Int32[] GetItems(Predicate<Item> predicate, Boolean vanillaOnly = false) {
        List<Int32> list = new List<Int32>();
        Int32 count = vanillaOnly ? ItemID.Count : ItemLoader.ItemCount;
        for (Int32 i = 0; i < count; i++) {
            if (predicate(ContentSamples.ItemsByType[i])) {
                list.Add(i);
            }
        }
        return list.ToArray();
    }
}