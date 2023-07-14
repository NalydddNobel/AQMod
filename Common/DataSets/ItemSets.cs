using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
                if (modItem.GetType().GetAttribute<UnusedContentAttribute>() != null) {
                    modItem.Item.ResearchUnlockCount = 0;
                    IsRemovedQuickCheck.Add(modItem.Type);
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
                }
            }
        }
    }

    public record struct DedicatedContentInfo {
        private readonly Func<Color> Color;
        public readonly string DedicateeName;

        public DedicatedContentInfo(string name, Func<Color> getColor) {
            DedicateeName = name;
            Color = getColor;
        }
        public DedicatedContentInfo(string name, Color color) : this(name, () => color) {
        }

        public Color GetTextColor() {
            return Color();
        }
    }
}