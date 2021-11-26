using AQMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Consumables.Potion
{
    public class SpoilsPotion : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.buyPrice(platinum: 1);
            item.buffTime = 7200;
            item.buffType = ModContent.BuffType<Spoiled>();
        }
    }
}