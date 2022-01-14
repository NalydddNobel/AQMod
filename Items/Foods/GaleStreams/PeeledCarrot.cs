using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods.GaleStreams
{
    public class PeeledCarrot : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 15);
            item.rare = AQItem.Rarities.GaleStreamsRare - 1;
            item.maxStack = 999;
            item.consumable = true;
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 20;
            item.useTime = 20;
            item.buffType = ModContent.BuffType<Buffs.Foods.PeeledCarrot>();
            item.buffTime = 72000;
        }
    }
}