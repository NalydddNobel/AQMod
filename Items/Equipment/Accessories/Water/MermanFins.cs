using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Items.Equipment.Accessories.Water;
using Terraria.Localization;

namespace Aequus.Items.Equipment.Accessories.Water {
    public class MermanFins : ModItem, ItemHooks.IUpdateItemDye {
        public static readonly int BuffDuration = 1200;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffDuration / 60);

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(BuffDuration * 2 / 60)));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 10);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
            Item.hasVanityEffects = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().mermanFins++;
        }

        public override Color? GetAlpha(Color lightColor) {
            Item.color = Main.LocalPlayer.skinColor;
            return null;
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().equippedEars = Type;
                player.Aequus().cEars = dyeItem.dye;
            }
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        public int mermanFins;

        private void PostUpdateEquips_MermanFins() {
            if (!Player.wet && mermanFins > 0) {
                Player.AddBuff(BuffID.Gills, MermanFins.BuffDuration * mermanFins, quiet: true);
            }
        }
    }
}