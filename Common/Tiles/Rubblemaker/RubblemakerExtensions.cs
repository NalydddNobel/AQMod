﻿using Terraria.GameContent;

namespace Aequus.Common.Tiles.Rubblemaker;
public static class RubblemakerExtensions {
    public static void AddToSmallRubblemaker(this ModTile modTile, int itemId, params int[] styles) {
        FlexibleTileWand.RubblePlacementSmall.AddVariations(itemId, modTile.Type, styles);
    }
    public static void AddToSmallRubblemaker<T>(this ModTile modTile, params int[] styles) where T : ModItem {
        AddToSmallRubblemaker(modTile, ModContent.ItemType<T>(), styles);
    }
    public static void AddToMediumRubblemaker(this ModTile modTile, int itemId, params int[] styles) {
        FlexibleTileWand.RubblePlacementMedium.AddVariations(itemId, modTile.Type, styles);
    }
    public static void AddToMediumRubblemaker<T>(this ModTile modTile, params int[] styles) where T : ModItem {
        AddToMediumRubblemaker(modTile, ModContent.ItemType<T>(), styles);
    }
    public static void AddToLargeRubblemaker(this ModTile modTile, int itemId, params int[] styles) {
        FlexibleTileWand.RubblePlacementLarge.AddVariations(itemId, modTile.Type, styles);
    }
    public static void AddToLargeRubblemaker<T>(this ModTile modTile, params int[] styles) where T : ModItem {
        AddToLargeRubblemaker(modTile, ModContent.ItemType<T>(), styles);
    }
}