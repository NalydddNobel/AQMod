using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.PlayerEquips
{
    public class EquipOverlayLoader
    {
        private readonly Dictionary<string, EquipOverlay>[] _overlays;

        public EquipOverlayLoader()
        {
            _overlays = new Dictionary<string, EquipOverlay>[(byte)EquipLayering.Count];
            for (int i = 0; i < _overlays.Length; i++)
            {
                _overlays[i] = new Dictionary<string, EquipOverlay>();
            }
        }

        public void AddOverlay(EquipOverlay overlay, EquipLayering type, string name)
        {
            _overlays[(byte)type].Add(name, overlay);
        }

        public void AddOverlay<T>(EquipOverlay overlay, EquipLayering type) where T : ModItem
        {
            _overlays[(byte)type].Add(typeof(T).Name, overlay);
        }

        public void AddHeadOverlay<T>(EquipHeadOverlay overlay) where T : ModItem
        {
            _overlays[(byte)EquipLayering.Head].Add(typeof(T).Name, overlay);
        }

        public void AddBodyOverlay<T>(EquipBodyOverlay overlay) where T : ModItem
        {
            _overlays[(byte)EquipLayering.Body].Add(typeof(T).Name, overlay);
        }

        /// <summary>
        /// Turns the overlay type into an equip type, returns an invalid equip type of -1 if there is not a match
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EquipType ArmorOverlayToEquipType(EquipLayering type)
        {
            switch (type)
            {
                case EquipLayering.Head:
                return EquipType.Head;
                case EquipLayering.Body:
                return EquipType.Body;
                case EquipLayering.Legs:
                return EquipType.Legs;
            }
            return (EquipType)(-1);
        }

        public void InvokeArmorOverlay(EquipLayering type, PlayerDrawInfo info)
        {
            int slot = -1;
            switch (type)
            {
                case EquipLayering.Head:
                slot = info.drawPlayer.head;
                if (slot < Main.numArmorHead)
                    return;
                break;
                case EquipLayering.Body:
                slot = info.drawPlayer.body;
                if (slot < Main.numArmorBody)
                    return;
                break;
                case EquipLayering.Legs:
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
        public EquipOverlay GetOverlay(EquipLayering type, string name)
        {
            var d = _overlays[(byte)type];
            if (d.ContainsKey(name))
                return d[name];
            return null;
        }
    }
}