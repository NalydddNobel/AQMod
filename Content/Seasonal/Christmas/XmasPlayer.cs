using AQMod.Items.Misc;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod.Content.Seasonal.Christmas
{
    public class XmasPlayer : ModPlayer
    {
        public bool givenXmasGift;

        public override void Initialize()
        {
            givenXmasGift = false;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["givenXmasGift"] = givenXmasGift,
            };
        }

        public override void Load(TagCompound tag)
        {
            givenXmasGift = tag.GetBool("givenXmasGift");
        }

        public override void UpdateBiomes()
        {
            if (XmasSeed.XmasWorld)
            {
                player.ZoneSnow = true;
            }
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var now = DateTime.Now;
                if (now.Month == 12 && now.Day == 25)
                {
                    if (!givenXmasGift && XmasSeed.XmasWorld)
                    {
                        givenXmasGift = true;
                        player.QuickSpawnClonedItem(GiftItem.CreateGiftItem(1));
                        Main.NewText(Language.GetTextValue("Mods.AQMod.MerryXmas." + new UnifiedRandom(Main.LocalPlayer.name.GetHashCode()).Next(2), player.name), new Color(230, 230, 255, 255));
                    }
                }
                else
                {
                    givenXmasGift = false;
                }
            }
        }
    }
}