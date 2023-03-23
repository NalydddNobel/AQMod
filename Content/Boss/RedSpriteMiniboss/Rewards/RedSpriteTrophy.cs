using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.RedSpriteMiniboss.Rewards
{
    public class RedSpriteTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.RedSprite);
            Item.maxStack = 9999;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}