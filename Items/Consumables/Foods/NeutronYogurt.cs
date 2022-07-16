using Aequus.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class NeutronYogurt : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            this.StaticDefaultsToDrink(Color.Yellow);
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.buffType = ModContent.BuffType<NeutronYogurtBuff>();
            Item.buffTime = 36000;
        }
    }
}