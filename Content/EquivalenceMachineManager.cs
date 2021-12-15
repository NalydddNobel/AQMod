using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public sealed class EquivalenceMachineManager : GlobalItem
    {
        public static bool AntiGravity { get; private set; }
        public static bool AntiGravityReset { get; private set; }
        private static byte update;

        public const int ItemsPerFrame = Main.maxItems / 10;

        internal static void UpdateItems()
        {
            AntiGravity = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    var aQPlayer = Main.player[i].GetModPlayer<AQPlayer>();
                    if (aQPlayer.antiGravityItems)
                    {
                        AntiGravity = true;
                        int min = ItemsPerFrame * update;
                        int max = ItemsPerFrame * (update + 1);
                        if (max > 400)
                        {
                            min = 0;
                            max = ItemsPerFrame;
                            update = 0;
                        }
                        var plrCenter = Main.player[i].Center;
                        for (int j = min; j < max; j++)
                        {
                            if (Main.item[j].active && !AQItem.Sets.Clones.ItemNoGravity[Main.item[j].type])
                            {
                                int grabRange = 500;
                                ItemLoader.GrabRange(Main.item[j], Main.player[i], ref grabRange);
                                var diff = Main.item[j].Center - plrCenter;
                                
                                if (Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y) < grabRange)
                                {
                                    ItemID.Sets.ItemNoGravity[Main.item[j].type] = true;
                                }
                                else
                                {
                                    ItemID.Sets.ItemNoGravity[Main.item[j].type] = false;
                                }
                            }
                        }
                    }
                }
            }
            if (!AntiGravity)
            {
                if (!AntiGravityReset)
                {
                    for (int i = 0; i < ItemLoader.ItemCount; i++)
                    {
                        ItemID.Sets.ItemNoGravity[i] = AQItem.Sets.Clones.ItemNoGravity[i];
                    }
                    update = 0;
                    AntiGravityReset = true;
                }
            }
            else
            {
                AntiGravityReset = false;
                update++;
            }
        }
    }
}