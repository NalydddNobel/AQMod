using Aequus.Items;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Common.ModPlayers {
    public interface IAccessoryData {
        void Clear() {
        }
    }

    public class Accessory<T> where T : class, IAccessoryData {
        private Item _item;
        private T _ref;

        public void SetAccessory(Item item, T accessory) {
            if (_item != null && item.rare > _item.rare) {
                return;
            }
            _item = item;
            _ref = accessory;
        }

        public void Clear() {
            _ref?.Clear();
            _ref = null;
            _item = null;
        }

        public IEntitySource GetSource(Player player) {
            return player.GetSource_Accessory(_item);
        }

        public int EquipmentStacks() {
            return _item.EquipmentStacks();
        }

        public bool TryGetItem(out Item item, out T accessory) {
            item = _item;
            accessory = _ref;
            return _item != null;
        }
    }
}