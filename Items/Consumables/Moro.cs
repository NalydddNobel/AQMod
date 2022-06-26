using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class Moro : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemDefaults.RarityGaleStreams + 2;
            Item.UseSound = SoundID.Item4;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool? UseItem(Player player)
        {
            if (!player.Aequus().moroSummonerFruit)
            {
                player.Aequus().moroSummonerFruit = true;
                return true;
            }

            return false;
        }
    }
}