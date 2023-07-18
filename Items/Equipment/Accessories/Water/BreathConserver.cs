using Aequus.Common.Items.EquipmentBooster;
using Aequus.Items.Equipment.Accessories.Water;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Water {
    [LegacyName("ArmFloaties")]
    public class BreathConserver : ModItem {
        public static int BuffDuration = 600;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffDuration / 60);

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(BuffDuration * 2 / 60)));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().breathConserver++;
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        public int breathConserver;

        private void PostUpdateEquips_BreathConserver() {
            if (!Player.wet && breathConserver > 0) {
                Player.AddBuff(BuffID.Gills, BreathConserver.BuffDuration * breathConserver, quiet: true);
            }
        }
    }
}