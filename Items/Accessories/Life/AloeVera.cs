using Aequus.Common.Items.EquipmentBooster;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life {
    public class AloeVera : ModItem {
        public static int LifeRegen = 4;
        public static float AddMultiplier = 0.5f;
        public static float DebuffDamageResistMultiplier = 0.75f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeRegen / 2, (int)(AddMultiplier * 100f), (int)((1f - DebuffDamageResistMultiplier) * 100f));

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(this.GetLocalization("BoostTooltip", () => "").WithFormatArgs(LifeRegen / 2, (int)(AddMultiplier * 2f * 100f), (int)((1f - MathF.Pow(DebuffDamageResistMultiplier, 2f)) * 100f)), null));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.lifeRegen += LifeRegen / 2; // Gets multiplied by 2 later
            player.Aequus().regenerationMultiplier += AddMultiplier;
            player.Aequus().regenerationBadMultiplier *= DebuffDamageResistMultiplier;
        }
    }
}