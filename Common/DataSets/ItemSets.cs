using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Aequus.Common.DataSets {
    public class ItemSets : DataSet {
        public static readonly Dictionary<int, DedicatedContentInfo> DedicatedContent = new();
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