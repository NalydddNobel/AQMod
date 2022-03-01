using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class PalePufferfish : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.consumable = true;
            item.value = Item.sellPrice(silver: 50);
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
            item.useTime = 17;
            item.useAnimation = 17;
            item.maxStack = 999;
        }

        public override bool UseItem(Player player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (player.buffTime[i] > 0 && AQBuff.Sets.CanBeRemovedByWhiteBloodCell.Contains(player.buffType[i]))
                {
                    player.DelBuff(i);
                    i--;
                }
            }
            player.GetModPlayer<VampirismPlayer>().Vampirism = 0;
            return true;
        }
    }
}