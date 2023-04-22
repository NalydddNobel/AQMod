using Aequus.Content.ItemPrefixes.Armor;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Misc {
    public abstract class ArmorPrefixItem<T> : ModItem, ItemHooks.IRightClickOverrideWhenHeld where T : MossArmorPrefixBase {

        public int ArmorPrefix => ModContent.PrefixType<T>();

        public virtual bool ApplyArmorPrefix(ref Item item, ref Item heldItem) {
            if (!item.IsArmor()) {
                return false;
            }

            int wantedPrefix = ArmorPrefix;
            Item newItem = new(item.netID, item.stack, wantedPrefix);
            if (newItem.prefix != wantedPrefix) {
                return false;
            }

            item = item.Clone();
            item.Center = Main.LocalPlayer.Center;
            item.Aequus().armorPrefixAnimation = 12;
            PopupText.NewText(PopupTextContext.ItemReforge, item, item.stack, noStack: true);
            SoundEngine.PlaySound(AequusSounds.evilConvert);
            if (--heldItem.stack <= 0) {
                heldItem.TurnToAir();
            }
            return true;
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }

        public bool RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus) {
            return ApplyArmorPrefix(ref inv[slot], ref heldItem);
        }
    }
}