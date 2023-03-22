using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused.DebugItems
{
    internal class ShakeEffect : ModItem
    {
        public override string Texture => AequusTextures.FrozenTear.Path;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 0;
            Tooltip.SetDefault(
                """
                Activates a shake effect on your cursor
                Testing Item
                """);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.rare = ItemRarityID.Red;
            Item.width = 20;
            Item.height = 20;
            Item.color = Main.OurFavoriteColor;
        }

        public override bool? UseItem(Player player)
        {
            int x = Helper.MouseTileX;
            int y = Helper.MouseTileY;

            ScreenShake.SetShake(30f, 0.9f, Main.MouseWorld);
            return true;
        }
    }
}