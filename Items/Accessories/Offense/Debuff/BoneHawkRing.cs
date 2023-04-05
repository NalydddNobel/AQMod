using Aequus.Common.Recipes;
using Aequus.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Debuff
{
    [AutoloadEquip(EquipType.HandsOn)]
    [LegacyName("BoneRing")]
    public class BoneHawkRing : ModItem
    {
        /// <summary>
        /// Default Value: 10
        /// </summary>
        public static int InflictChance = 10;
        /// <summary>
        /// Default Value: 300 (5 seconds)
        /// </summary>
        public static int DebuffDuration = 300;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accBoneRing++;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<BattleAxe>());
        }
    }
}