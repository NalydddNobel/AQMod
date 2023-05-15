using Aequus.Common.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.RecordBreaker {
    [AutoloadGlowMask]
    public class RecordBreaker : ModItem {
        public override void SetDefaults() {
            Item.SetWeaponValues(25, 4.5f, 6);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.hammer = 59;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(ItemID.GoldHammer, Type);
            AequusRecipes.AddShimmerCraft(ItemID.PlatinumHammer, Type);
        }
    }
}