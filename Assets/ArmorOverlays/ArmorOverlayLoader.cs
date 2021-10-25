using AQMod.Common.PlayerLayers.ArmorOverlays;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.ArmorOverlays
{
    public class ArmorOverlayLoader
    {
        private readonly Dictionary<string, ArmorOverlay>[] _overlays;

        public ArmorOverlayLoader()
        {
            _overlays = new Dictionary<string, ArmorOverlay>[(byte)ArmorOverlayType.Count];
            for (int i = 0; i < _overlays.Length; i++)
            {
                _overlays[i] = new Dictionary<string, ArmorOverlay>();
            }
        }

        public void AddOverlay(ArmorOverlay overlay, ArmorOverlayType type, string name)
        {
            _overlays[(byte)type].Add(name, overlay);
        }

        public void AddOverlay<T>(ArmorOverlay overlay, ArmorOverlayType type) where T : ModItem
        {
            _overlays[(byte)type].Add(typeof(T).Name, overlay);
        }

        public void AddHeadOverlay<T>(ArmorHeadOverlay overlay) where T : ModItem
        {
            _overlays[(byte)ArmorOverlayType.Head].Add(typeof(T).Name, overlay);
        }

        public void AddBodyOverlay<T>(ArmorBodyOverlay overlay) where T : ModItem
        {
            _overlays[(byte)ArmorOverlayType.Body].Add(typeof(T).Name, overlay);
        }

        /// <summary>
        /// Turns the overlay type into an equip type, returns an invalid equip type of -1 if there is not a match
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EquipType ArmorOverlayToEquipType(ArmorOverlayType type)
        {
            switch (type)
            {
                case ArmorOverlayType.Head:
                return EquipType.Head;
                case ArmorOverlayType.Body:
                return EquipType.Body;
                case ArmorOverlayType.Legs:
                return EquipType.Legs;
            }
            return (EquipType)(-1);
        }

        public void InvokeArmorOverlay(ArmorOverlayType type, PlayerDrawInfo info)
        {
            int slot = -1;
            switch (type)
            {
                case ArmorOverlayType.Head:
                slot = info.drawPlayer.head;
                if (slot < Main.numArmorHead)
                    return;
                break;
                case ArmorOverlayType.Body:
                slot = info.drawPlayer.body;
                if (slot < Main.numArmorBody)
                    return;
                break;
                case ArmorOverlayType.Legs:
                slot = info.drawPlayer.legs;
                if (slot < Main.numArmorLegs)
                    return;
                break;
                default:
                return;
            }
            var equipType = ArmorOverlayToEquipType(type);
            var texture = EquipLoader.GetEquipTexture(equipType, slot);
            if (texture.mod.Name == "AQMod")
            {
                //if (Main.GameUpdateCount % 30 == 0)
                //    Main.NewText(texture.Name);
                var overlay = GetOverlay(type, texture.Name);
                if (overlay != null)
                    overlay.Draw(info);
            }
        }

        /// <summary>
        /// Returns null if it doesn't have the overlay
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ArmorOverlay GetOverlay(ArmorOverlayType type, string name)
        {
            var d = _overlays[(byte)type];
            if (d.ContainsKey(name))
                return d[name];
            return null;
        }
    }
}