using AQMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class NeutronJuice : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 30;
            item.consumable = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.buyPrice(silver: 20);
            item.buffType = ModContent.BuffType<NeutronYogurtBuff>();
            item.buffTime = 36000;
        }
    }
}