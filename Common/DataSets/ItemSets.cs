using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public class ItemSets : DataSet {
        public static readonly Dictionary<int, DedicatedContentInfo> DedicatedContent = new();
        public static readonly HashSet<int> IsPaintbrush = new();
        public static readonly HashSet<int> IsRemovedQuickCheck = new();
        public static readonly List<int> FishingTrashForDevilsTounge = new();
        public static readonly List<Item> Helmets = new();
        public static readonly List<Item> Chestplates = new();
        public static readonly List<Item> Leggings = new();

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

        public override void PostSetupContent(Aequus aequus) {
            foreach (var modItem in aequus.GetContent<ModItem>()) {
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