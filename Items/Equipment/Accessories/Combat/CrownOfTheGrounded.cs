using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.PlayerLayers.Equipment;
using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Equipment.Accessories.Combat {
    public class CrownOfTheGrounded : ModItem, ItemHooks.IUpdateItemDye {
        /// <summary>
        /// Default Value: 10
        /// </summary>
        public static int AddedCrit = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AddedCrit);

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(AddedCrit * 2)));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = Item.buyPrice(gold: 7, silver: 50);
            Item.hasVanityEffects = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory
                && !Main.LocalPlayer.Aequus().grounded) {
                return Color.Gray;
            }
            return null;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accGroundCrownCrit += AddedCrit;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                var crown = player.Aequus().GetEquipDrawer<HoverCrownEquip>();
                crown.SetEquip(this, dyeItem);
                crown.CrownColor = Color.LightGreen;
            }
        }
    }
}