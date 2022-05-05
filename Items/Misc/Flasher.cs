using Aequus.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Flasher : ModItem
    {
        public static float flashIntensity;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Testing Item");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(silver: 15);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 1;
            Item.useAnimation = 1;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                FlashScene.Flash.Set(Main.MouseWorld, 1f);
                EffectsSystem.Shake.Set(20f);
            }
            return true;
        }
    }
}