using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod {
    internal class MagicStorage : ModSupport<MagicStorage>
    {
        private static object BlockItemsHook;
        private static List<object> BlacklistedItemData;
        private static Type ItemDataType;
        private static IList blockStorageItemList;

        public override void SafeLoad(Mod mod)
        {
            try
            {
                var craftingGUI = mod.Code.GetType("MagicStorage.CraftingGUI");
                if (craftingGUI != null)
                {
                    var setSelectedRecipe = craftingGUI.GetMethod("SetSelectedRecipe", BindingFlags.NonPublic | BindingFlags.Static);
                    if (setSelectedRecipe == null)
                    {
                        return;
                    }
                    var blockStorageItemsField = craftingGUI.GetField("blockStorageItems", BindingFlags.NonPublic | BindingFlags.Static);
                    if (blockStorageItemsField == null)
                    {
                        return;
                    }
                    var setSelectedRecipeHook = GetType().GetMethod(nameof(SetSelectedRecipe), BindingFlags.Public | BindingFlags.Static);
                    new Hook(setSelectedRecipe, setSelectedRecipeHook);
                    blockStorageItemList = (IList)blockStorageItemsField.GetValue(null);
                }
                ItemDataType = mod.Code.GetType("MagicStorage.ItemData");
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void SetSelectedRecipe(Action<Recipe> orig, Recipe recipe)
        {
            orig(recipe);
            try
            {
                if (blockStorageItemList != null && ItemDataType != null && BlacklistedItemData != null)
                {
                    foreach (var itemData in BlacklistedItemData)
                    {
                        blockStorageItemList.Add(itemData);
                    }
                }
            }
            catch (Exception ex)
            {
                Aequus.Instance.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void AddBlacklistedItemData(int item, int prefix)
        {
            if (Instance == null || ItemDataType == null)
                return;

            if (BlacklistedItemData == null)
                BlacklistedItemData = new List<object>();

            try
            {
                var itemData = Activator.CreateInstance(ItemDataType, new object[] { item, prefix });
                if (itemData != null)
                {
                    BlacklistedItemData.Add(itemData);
                }
            }
            catch (Exception ex)
            {
                Aequus.Instance.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void SafeUnload()
        {
            BlockItemsHook = null;
            BlacklistedItemData?.Clear();
            BlacklistedItemData = null;
            ItemDataType = null;
            blockStorageItemList = null;
        }
    }
}