using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.PlayerLayers.Equipment;
using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Accessories.Combat {
    public class CrownOfDarkness : ModItem, ItemHooks.IUpdateItemDye {
        public static float DamageIncrease = 0.1f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.Percent(DamageIncrease, TextHelper.DefaultPercentFormat));

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(TextHelper.Create.Percent(DamageIncrease * 2f, TextHelper.DefaultPercentFormat))));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = Item.buyPrice(gold: 7, silver: 50);
            Item.hasVanityEffects = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory
                && !Main.LocalPlayer.Aequus().InDarkness) {
                return Color.Gray;
            }
            return null;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accDarknessCrownDamage += DamageIncrease;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                var crown = player.Aequus().GetEquipDrawer<HoverCrownEquip>();
                crown.SetEquip(this, dyeItem);
                crown.CrownColor = Color.Blue;
            }
        }
    }
}