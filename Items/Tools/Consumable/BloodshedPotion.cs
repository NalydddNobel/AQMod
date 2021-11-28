using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Consumable
{
    public class BloodshedPotion : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.value = AQItem.Prices.PotionValue;
            item.maxStack = 999;
        }

        public override void UseStyle(Player player)
        {
            if (player.itemTime == 0)
                player.itemTime = (int)(item.useTime / PlayerHooks.TotalUseTimeMultiplier(player, item));
            else if (player.itemTime == (int)(item.useTime / PlayerHooks.TotalUseTimeMultiplier(player, item)) / 2)
            {
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 60);
                    Main.dust[d].velocity = player.velocity * 0.2f;
                }
                if (player.lastDeathPostion == new Vector2(0f, 0f))
                    return;
                player.PrepareForTeleport();
                player.Teleport(player.lastDeathPostion - new Vector2(0f, 48f), -1);
                item.stack--;
                if (item.stack <= 0)
                    item.TurnToAir();
                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 60);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = player.velocity * 0.2f;
                }
            }
        }
    }
}