using Aequus.Common.Items.EquipmentBooster;
using Aequus.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life {
    [LegacyName("BloodCurcleav")]
    public class BloodCrystal : ModItem {
        /// <summary>
        /// Default Value: 20
        /// </summary>
        public static int DebuffLifeSteal = 30;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DebuffLifeSteal);

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(DebuffLifeSteal / 2)));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            if (aequus.debuffLifeSteal > 0) {
                aequus.debuffLifeSteal = Math.Max(aequus.debuffLifeSteal / 2, 1);
            }
            else {
                player.Aequus().debuffLifeSteal = DebuffLifeSteal;
            }
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<BloodyTearstone>(8)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<BloodyTearstone>(8)
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}