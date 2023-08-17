using Aequus.Items.Weapons.Ranged.Misc.BlockGlove;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Common.DataSets {
    public class ItemSets : DataSet {
        protected override ContentFileInfo ContentFileInfo => new(ItemID.Search);

        public static Dictionary<int, LocalizedText> RarityNames = new();
        public static List<int> LegendaryFish = new();
        /// <summary>
        /// Items marked as important will have special properties:
        /// <list type="bullet">
        /// <item>If this item is inside a chest, the chest will not gain Aequus loot, or Hardmode Chest Loot.</item>
        /// </list>
        /// </summary>
        public static readonly HashSet<int> ImportantItem = new();
        public static readonly List<int> OrderedPillarFragments_ByClass = new();
        public static readonly List<int> OrderedPillarFragments_ByColor = new();
        public static readonly Dictionary<int, DedicatedContentInfo> DedicatedContent = new();
        public static readonly HashSet<int> IsPaintbrush = new();
        public static readonly HashSet<int> IsRemovedQuickCheck = new();
        public static readonly List<int> FishingTrashForDevilsTounge = new();
        public static readonly HashSet<int> IsPotion = new();
        public static readonly List<Item> Helmets = new();
        public static readonly List<Item> Chestplates = new();
        public static readonly List<Item> Leggings = new();

        internal static Dictionary<int, int> _itemToBannerLookup;

        public override void OnLoad(Mod mod) {
            IsPaintbrush.Add(ItemID.Paintbrush);
            IsPaintbrush.Add(ItemID.SpectrePaintbrush);
            IsPaintbrush.Add(ItemID.PaintRoller);
            IsPaintbrush.Add(ItemID.SpectrePaintRoller);
            FishingTrashForDevilsTounge.Add(ItemID.FishingSeaweed);
            FishingTrashForDevilsTounge.Add(ItemID.TinCan);
            FishingTrashForDevilsTounge.Add(ItemID.OldShoe);
            FishingTrashForDevilsTounge.Add(ItemID.JojaCola);
        }

        public override void PostSetupContent() {
            #region Rarity Names
            for (int i = ItemRarityID.Master; i <= ItemRarityID.Purple; i++) {
                if (TextHelper.TryGet("Mods.Aequus.ItemRarity." + i, out var text)) {
                    RarityNames.Add(i, text);
                }
            }
            foreach (var modRarity in ModContent.GetContent<ModRarity>()) {
                if (TextHelper.TryGet($"Mods.Aequus.ItemRarity.{modRarity.Mod.Name}.{modRarity.Name}", out var text)) {
                    RarityNames.Add(modRarity.Type, text);
                }
            }
            #endregion
            foreach (var modItem in Aequus.Instance.GetContent<ModItem>()) {
                if (modItem.GetType().GetAttribute<UnusedContentAttribute>() != null || modItem.GetType().GetAttribute<WorkInProgressAttribute>() != null) {
                    modItem.Item.ResearchUnlockCount = 0;
                }
                var modRequired = modItem.GetType().GetAttribute<ModRequiredAttribute>();
                if (modRequired != null && !ModLoader.HasMod(modRequired.ModNeeded)) {
                    modItem.Item.ResearchUnlockCount = 0;
                }
            }
            foreach (var item in ContentSamples.ItemsByType.Values) {
                if (!item.vanity) {
                    if (item.headSlot > -1) {
                        Helmets.Add(item);
                    }
                    if (item.bodySlot > -1) {
                        Chestplates.Add(item);
                    }
                    if (item.legSlot > -1) {
                        Leggings.Add(item);
                    }
                    if (item.rare >= ItemRarityID.White && item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.buffNoTimeDisplay[item.buffType] && !Main.meleeBuff[item.buffType] && !BuffSets.PotionPrefixBlacklist.Contains(item.buffType)) {
                        IsPotion.Add(item.type);
                    }
                }
            }
        }
    }

    public record struct DedicatedContentInfo {
        public static DedicatedContentInfo DefaultValue = new("nalyddd", () => Color.Lerp(Color.BlueViolet, Color.Purple, Helper.Wave((float)Main.timeForVisualEffects, 0f, 1f)));

        private readonly Func<Color> GetColor;
        public readonly string DedicateeName;

        public bool IsInvalid => string.IsNullOrEmpty(DedicateeName);

        public DedicatedContentInfo(string name, Func<Color> getColor) {
            DedicateeName = name;
            GetColor = getColor;
        }
        public DedicatedContentInfo(string name, Color color) : this(name, () => color) {
        }

        public Color GetTextColor() {
            return GetColor();
        }

        public Color GetFaelingColor() {
            return GetColor();
        }

        public static DedicatedContentInfo FromName(string name) {
            TryFromName(name, out var info);
            return info;
        }

        public static bool TryFromName(string name, out DedicatedContentInfo info) {
            info = DefaultValue;
            foreach (var d in ItemSets.DedicatedContent) {
                if (d.Value.DedicateeName == name) {
                    info = d.Value;
                    return true;
                }
            }
            return false;
        }

        public static DedicatedContentInfo Random(UnifiedRandom random = null) {
            random ??= Main.rand;
            for (int i = 0; i< 100000; i++) {
                if (ItemSets.DedicatedContent.TryGetValue(random.Next(ItemLoader.ItemCount), out var value)) {
                    return value;
                }
            }
            return DefaultValue;
        }
    }
}