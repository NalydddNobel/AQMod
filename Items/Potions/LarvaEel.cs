using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class LarvaEel : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 15);
            item.rare = ItemRarityID.Green;
            item.maxStack = 30;
            item.consumable = true;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 20;
            item.useTime = 20;
            item.potion = true;
            item.healLife = 100;
            item.buffType = BuffID.Honey;
            item.buffTime = 10800;
        }
    }
}