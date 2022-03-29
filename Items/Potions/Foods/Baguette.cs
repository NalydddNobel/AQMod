using AQMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions.Foods
{
    public class Baguette : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.UseSound = SoundID.Item2;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(silver: 80);
            item.buffType = ModContent.BuffType<BaguetteBuff>();
            item.buffTime = 216000;
        }

        public override bool CanBurnInLava()
        {
            return true;
        }
    }
}