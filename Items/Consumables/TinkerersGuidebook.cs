using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class TinkerersGuidebook : ModItem
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
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                AequusWorld.tinkererRerolls = 0;
                return true;
            }
            if (AequusWorld.tinkererRerolls < 3)
            {
                AequusWorld.tinkererRerolls += 3;
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