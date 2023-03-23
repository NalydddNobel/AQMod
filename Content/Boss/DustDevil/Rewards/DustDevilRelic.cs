using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.DustDevil.Rewards
{
    public class DustDevilRelic : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BossRelics>(), BossRelics.DustDevil);
            Item.width = 30;
            Item.height = 40;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.buyPrice(gold: 5);
        }
    }
}