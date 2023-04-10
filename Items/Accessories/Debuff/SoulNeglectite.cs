using Aequus.Items.Materials.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Debuff {
    [LegacyName("SoulCrystal")]
    public class SoulNeglectite : ModItem {
        /// <summary>
        /// Default Value: 4
        /// </summary>
        public static int DebuffDamageRate = 4;
        /// <summary>
        /// Default Value: 10
        /// </summary>
        public static int DebuffDamage = 10;
        /// <summary>
        /// Default Value: <see cref="DebuffDamage"/> * <see cref="DebuffDamageRate"/> (40)
        /// <para>Yea, this is weird, trust me. But dealing x4 the damage keeps the damage numbers spawning at a similar rate for some reason...?</para>
        /// </summary>
        public static int RealDebuffDamage => DebuffDamage * DebuffDamageRate;

        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().debuffDamage += RealDebuffDamage;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<SoulGemFilled>(5)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}