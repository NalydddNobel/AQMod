using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods.GaleStreams
{
    public class CinnamonRoll : ModItem
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
            item.buffType = BuffID.WellFed;
            item.buffTime = 72000;
        }

        public override bool UseItem(Player player)
        {
            if (player.name == "Nalyd T.")
            {
                player.AddBuff(BuffID.Poisoned, 240);
            }
            return base.UseItem(player);
        }
    }
}