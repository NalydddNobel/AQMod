using Aequus.Buffs;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions {
    public class NeutronYogurt : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            this.StaticDefaultsToDrink(Color.HotPink);
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
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.buffType = ModContent.BuffType<NeutronYogurtBuff>();
            Item.buffTime = 36000;
        }
    }
}