using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.PlayerLayers.Equipment {
    public sealed class EquipDrawLoader : ILoadable {
        private static List<EquipDraw> _equips = new();

        public static int Count => _equips.Count;

        public void Load(Mod mod) {
        }

        public void Unload() {
            _equips.Clear();
        }

        public static EquipDraw Get(int type) {
            return _equips[type];
        }

        public static int Register(EquipDraw draw) {
            if (draw.Type >= 0) {
                throw new Exception($"The equip draw '{draw.GetType().Name}' has already been registered.");
            }

            int type = Count;
            draw.Type = type;
            _equips.Add(draw);
            return type;
        }
    }
}