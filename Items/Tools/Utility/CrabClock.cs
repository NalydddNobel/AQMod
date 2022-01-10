using AQMod.Content.World.Events;
using AQMod.Localization;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Utility
{
    public class CrabClock : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = Item.sellPrice(silver: 20);
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTime = 100;
            item.useAnimation = 100;
            item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.channel)
            {
                player.itemTime = 10;
                player.itemAnimation = 10;
                CrabSeason.crabSeasonTimerRate += 99;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text;
            if (item.favorited)
            {
                text = CrabSeason.crabSeasonTimer < 0
                    ? string.Format(AQText.ModText("CrabClock.Ends").Value, AQUtils.TimeText2(CrabSeason.CrabSeasonTimerMin + CrabSeason.crabSeasonTimer))
                    : string.Format(AQText.ModText("CrabClock.Starts").Value, AQUtils.TimeText2(CrabSeason.crabSeasonTimer));
            }
            else
            {
                text = AQText.ModText("CrabClock.Favorite").Value;
            }
            tooltips.Add(new TooltipLine(mod, "CrabSeasonTimer", text));
        }
    }
}