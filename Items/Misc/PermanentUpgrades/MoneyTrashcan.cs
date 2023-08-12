using Aequus.Common.Items;
using Aequus.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Misc.PermanentUpgrades {
    public class MoneyTrashcan : ModItem {
        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.expert = true;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool AltFunctionUse(Player player) {
            return true;
        }

        public override bool ConsumeItem(Player player) {
            return player.altFunctionUse != 2;
        }

        public override bool? UseItem(Player player) {
            var aequus = player.Aequus();
            if (player.altFunctionUse == 2) {
                aequus.usedPermaTrashMoney = false;
                return true;
            }
            if (!aequus.usedPermaTrashMoney) {
                aequus.usedPermaTrashMoney = true;
                return true;
            }

            return false;
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        [SaveData("CrabsonTrashcan")]
        public bool usedPermaTrashMoney;
        public float trashMoney;

        public Item lastTrashItem;

        public double GetTrashPrice(int itemType, int itemStack) {
            double multiplier = Math.Clamp(trashMoney, 0.0, 1.0) / 5.0;
            long value = ContentSamples.ItemsByType[itemType].value;
            var price = value * itemStack * multiplier;
            return price;
        }
        public double GetTrashPrice(Item item) {
            return GetTrashPrice(item.netID, item.stack);
        }

        public void ResetEffects_TrashMoney() {
            trashMoney = usedPermaTrashMoney ? 0.25f : 0f;
        }

        public void Load_TrashMoney() {
            ItemSlot.OnItemTransferred += ItemSlot_OnItemTransferred;
        }

        public void Unload_TrashMoney() {
            ItemSlot.OnItemTransferred -= ItemSlot_OnItemTransferred;
        }

        private static bool ItemSlot_OverrideLeftClick_MoneyTrashcan(Item[] inv, int context, int slot) {

            if (context == ItemSlot.Context.TrashItem) {
                if (inv[slot].IsAir || inv[slot].IsACoin || Main.mouseItem != null && !Main.mouseItem.IsAir) {
                    return false;
                }
                double amount = Main.LocalPlayer.Aequus().GetTrashPrice(inv[slot]);
                if (!Main.LocalPlayer.BuyItem((int)amount)) {
                    return true;
                }
            }

            return false;
        }

        private static void ItemSlot_OnItemTransferred(ItemSlot.ItemTransferInfo info) {
            if (info.ToContext == ItemSlot.Context.TrashItem && !ContentSamples.ItemsByType[info.ItemType].IsACoin) {
                double amount = Main.LocalPlayer.Aequus().GetTrashPrice(info.ItemType, info.TransferAmount);
                Helper.DropMoney(new EntitySource_Misc("Aequus: Money Trashcan"), Main.LocalPlayer.Hitbox, (long)amount, quiet: false);
            }
        }
    }
}