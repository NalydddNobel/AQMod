using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods
{
    public class PeeledCarrot : ModItem, ISpecialFood
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 15);
            item.rare = ItemRarityID.LightRed;
            item.maxStack = 999;
            item.consumable = true;
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 20;
            item.useTime = 20;
            item.buffType = BuffID.WellFed;
            item.buffTime = 72000;
        }

        int ISpecialFood.ChangeBuff(Player player)
        {
            return ModContent.BuffType<Buffs.Foods.PeeledCarrot>();
        }
    }
}