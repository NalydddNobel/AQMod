using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class VictorsReward : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 1);
            Item.maxStack = 9999;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.Aequus().maxLifeRespawnReward = false;
                return true;
            }
            if (!player.Aequus().maxLifeRespawnReward)
            {
                player.Aequus().maxLifeRespawnReward = true;
                return true;
            }

            return false;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }
    }
}