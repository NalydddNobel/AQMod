using AequusRemake.Content.Critters.SeaFirefly;
using AequusRemake.Core.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace AequusRemake.Systems;

public class RecipeSystem : ModSystem {
    /// <summary><see cref="RecipeGroup"/> for Sea Firefly variants.</summary>
    public static RecipeGroup AnySeaFirefly { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for Gold Bar variants.</summary>
    public static RecipeGroup AnyGoldBar { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for Silver Bar variants.</summary>
    public static RecipeGroup AnySilverBar { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for Copper Bar variants.</summary>
    public static RecipeGroup AnyCopperBar { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for all IDs in <see cref="Main.anglerQuestItemNetIDs"/>.</summary>
    public static RecipeGroup AnyQuestFish { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for the Shellphone and its alternative modes.</summary>
    public static RecipeGroup Shellphone { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for paints.</summary>
    public static RecipeGroup AnyPaints { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for coatings.</summary>
    public static RecipeGroup AnyCoatings { get; private set; }
    /// <summary><see cref="RecipeGroup"/> for trash items.</summary>
    public static RecipeGroup AnyTrash { get; private set; }

    public override void AddRecipeGroups() {
        AnySeaFirefly = NewGroup("AnySeaFirefly", GetItems((i) => i.makeNPC == ModContent.NPCType<SeaFirefly>()));
        AnyGoldBar = NewGroup("AnyGoldBar", ItemID.GoldBar, ItemID.PlatinumBar);
        AnySilverBar = NewGroup("AnySilverBar", ItemID.SilverBar, ItemID.TungstenBar);
        AnyCopperBar = NewGroup("AnyCopperBar", ItemID.CopperBar, ItemID.TinBar);
        AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.NewClone());
        Shellphone = NewGroup("Shellphone", ItemID.ShellphoneDummy, ItemID.Shellphone, ItemID.ShellphoneHell, ItemID.ShellphoneOcean, ItemID.ShellphoneSpawn);
        AnyTrash = NewGroup("AnyTrash", ItemID.OldShoe, ItemID.FishingSeaweed, ItemID.TinCan);
        AnyPaints = NewGroup("AnyPaint", GetItems((i) => i.paint > PaintID.None));
        AnyCoatings = NewGroup("AnyCoatings", GetItems((i) => i.paintCoating > PaintCoatingID.None));
    }

    private static RecipeGroup NewGroup(string name, params int[] items) {
        RecipeGroup group = new RecipeGroup(() => Language.GetOrRegister("Mods.AequusRemake.Items.RecipeGroups." + name + ".DisplayName").Value, items);
        RecipeGroup.RegisterGroup(name, group);
        return group;
    }

    private static int[] GetItems(Predicate<Item> predicate, bool vanillaOnly = false) {
        return GetItemsInner(predicate, 0, vanillaOnly ? ItemID.Count : ItemLoader.ItemCount).ToArray();
    }

    private static IEnumerable<int> GetItemsInner(Predicate<Item> predicate, int minIdRange, int maxIdRange) {
        for (int i = minIdRange; i < maxIdRange; i++) {
            if (predicate(ContentSamples.ItemsByType[i])) {
                yield return i;
            }
        }
    }
}