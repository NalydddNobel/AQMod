using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life.Water {
    public class MermanFins : ModItem, ItemHooks.IUpdateItemDye {
        public static int BuffDuration = 1200;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffDuration / 60);

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(BuffDuration * 2 / 60)));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 10);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 2);
            Item.hasVanityEffects = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().breathConserver += 2;
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