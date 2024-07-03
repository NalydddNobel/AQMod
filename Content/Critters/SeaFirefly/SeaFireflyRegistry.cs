using Aequu2.Content.Systems;
using Aequu2.Content.Tiles.Misc.SeaFireflyBlock;
using Aequu2.Core.Graphics.Textures;
using Aequu2.DataSets;
using Aequu2.DataSets.Structures.Enums;
using System;

namespace Aequu2.Content.Critters.SeaFirefly;
public class SeaFireflyRegistry {
    public static byte RainbowIndex { get; private set; }

    private static ISeaFireflyInstanceData[] _registeredPalettes = [];

    public static ISeaFireflyInstanceData Default => _registeredPalettes[0];

    public static int ColorCount => _registeredPalettes.Length;

    private static Mod Mod => ModContent.GetInstance<Aequu2>();

    public static EffectTextureMask SeaFireflyItemMaskEffect { get; private set; }

    public static ISeaFireflyInstanceData GetPalette(int type) {
        return _registeredPalettes[type];
    }

    public static byte RegisterPalette(ISeaFireflyInstanceData dye) {
        Array.Resize(ref _registeredPalettes, _registeredPalettes.Length + 1);

        _registeredPalettes[^1] = dye;
        return (byte)(_registeredPalettes.Length - 1);
    }

    static void AddBasicDyes(SeaFirefly parent) {
        foreach (var pair in PaintDataSet.RGB) {
            // Skip entry if it's brown or doesn't have a related dye.
            if (pair.Key == PaintColor.Brown || !PaintDataSet.Dyes.TryGetValue(pair.Key, out var item)) {
                continue;
            }

            string name = pair.Key.ToString();
            Color rgb = pair.Value with { A = 40 };
            int dyeItem = item.Id;
            AddBasicDyeVariant(parent, name, rgb, dyeItem);
        }
    }

    static byte AddBasicDyeVariant(SeaFirefly parent, string Name, Color DyeColor, int DyeItem) {
        return AddDyedVariant(parent, new DyeVariant(Name, DyeColor), DyeItem);
    }

    static byte AddDyedVariant(SeaFirefly parent, ISeaFireflyInstanceData Variant, int DyeItem) {
        byte color = AddVariant(ref Variant);

        ModItem dyedItem = new SeaFireflyItem(parent, Variant.Name, color);

        Mod.AddContent(dyedItem);

        AddSeaFireflyBlock(Variant);

        Aequu2.OnAddRecipes += AddRecipe;

        return color;

        void AddRecipe() {
            dyedItem.CreateRecipe()
                .AddRecipeGroup(RecipeSystem.AnySeaFirefly)
                .AddIngredient(DyeItem)
                .Register();
        }
    }

    static byte AddVariant(ref ISeaFireflyInstanceData Variant) {
        byte color = RegisterPalette(Variant);
        Variant.Type = color;
        return color;
    }

    static void AddVariantFull(ISeaFireflyInstanceData Variant) {
        byte result = AddVariant(ref Variant);
        AddSeaFireflyBlock(Variant);
    }

    static void AddSeaFireflyBlock(ISeaFireflyInstanceData Variant) {
        ModTile tile = new SeaFireflyBlock(Variant.Name, Variant.Type);

        Mod.AddContent(tile);
    }

    internal static void LoadAll(SeaFirefly parent) {
        AddVariantFull(new DefaultVariant(""));

        if (!Main.dedServ) {
            Main.QueueMainThreadAction(() => SeaFireflyItemMaskEffect = new EffectTextureMask(AequusTextures.SeaFireflyItem_Dyed.Asset));
        }

        // Add basic dye variants.
        AddBasicDyes(parent);

        // Add rainbow dye variant
        RainbowIndex = AddDyedVariant(parent, new RainbowVariant("Rainbow"), ItemID.RainbowDye);
    }
}
