using Aequus.Core.CrossMod;
using System;

namespace Aequus.Core.Utilities;

public static class ExtendShop {
    /// <summary>Finds the next null index in an item array. Items which are "Air" are empty slots which were added to not contain any items inside them.</summary>
    /// <param name="itemList">The shop's item list.</param>
    public static int FindNextIndex(Item[] itemList) {
        for (int i = 0; i < itemList.Length; i++) {
            if (itemList[i] == null) {
                return i;
            }
        }

        return itemList.Length;
    }

    /// <summary>Adds an item with a custom value. (<paramref name="customValue"/>)</summary>
    /// <param name="shop"></param>
    /// <param name="itemType">The Item Id to add.</param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions) {
        var item = new Item(itemType) {
            shopCustomPrice = customValue
        };
        return shop.Add(item, conditions);
    }
    /// <summary><inheritdoc cref="AddCustomValue(NPCShop, int, int, Condition[])"/></summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="shop"></param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue(ModContent.ItemType<T>(), customValue, conditions);
    }
    /// <summary><inheritdoc cref="AddCustomValue(NPCShop, int, int, Condition[])"/></summary>
    /// <param name="itemType">The Item Id to add.</param>
    /// <param name="shop"></param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, double customValue, params Condition[] conditions) {
        return shop.AddCustomValue(itemType, (int)Math.Round(customValue), conditions);
    }
    /// <summary><inheritdoc cref="AddCustomValue(NPCShop, int, int, Condition[])"/></summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="shop"></param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue<T>(this NPCShop shop, double customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue<T>((int)Math.Round(customValue), conditions);
    }

    /// <summary>Attempts to add an item from another mod, if it exists and that mod is loaded.</summary>
    /// <typeparam name="T">The Mod.</typeparam>
    /// <param name="shop"></param>
    /// <param name="itemName">The internal name of the item.</param>
    /// <param name="conditions">Conditions for this item.</param>
    internal static NPCShop AddCrossMod<T>(this NPCShop shop, string itemName, params Condition[] conditions) where T : SupportedMod<T> {
        if (!SupportedMod<T>.TryFind<ModItem>(itemName, out var modItem)) {
            return shop;
        }
        return shop.Add(modItem.Type, conditions);
    }
}
