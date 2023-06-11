﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public class ItemSets : DataSet {
        public static readonly Dictionary<int, DedicatedContentInfo> DedicatedContent = new();
        public static readonly HashSet<int> IsPaintbrush = new();

        public override void OnLoad(Mod mod) {
            IsPaintbrush.Add(ItemID.Paintbrush);
            IsPaintbrush.Add(ItemID.SpectrePaintbrush);
            IsPaintbrush.Add(ItemID.PaintRoller);
            IsPaintbrush.Add(ItemID.SpectrePaintRoller);
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