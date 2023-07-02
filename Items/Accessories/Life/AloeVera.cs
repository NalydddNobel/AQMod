using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life {
    public class AloeVera : ModItem {
        public static int LifeRegen = 4;
        public static float AddMultiplier = 1f;
        public static float DebuffDamageResist = 0.75f;

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this);
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.lifeRegen += LifeRegen / 2; // Gets multiplied by 2 later
            player.Aequus().regenerationMultiplier += AddMultiplier;
            player.Aequus().regenerationBadMultiplier *= DebuffDamageResist;
        }
    }
}