using Aequus.Items.Consumables.SlotMachines;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ExporterQuests
{
    public struct DefaultThieveryItemInfo : IThieveryItemInfo
    {
        public void SpawnLoot(Player player, int thieveryItemIndex)
        {
            var source = player.GetSource_GiftOrReward("Robster");

            if (Main.rand.NextBool(4))
            {
                player.QuickSpawnItem(source, ModContent.ItemType<GoldenRoulette>(), 1);
            }
            else
            {
                player.QuickSpawnItem(source, ModContent.ItemType<Roulette>(), 1);
            }

            int amtRolled = Math.Max(ExporterQuestSystem.QuestsCompleted / 15, 1);
            for (int k = 0; k < amtRolled; k++)
            {
                int roulette = SpawnLoot_ChooseRoulette(player, thieveryItemIndex);
                if (roulette != 0)
                {
                    player.QuickSpawnItem(source, roulette, 1);
                }
            }

            int extraMoney = Item.silver * 3 * ExporterQuestSystem.QuestsCompleted;
            AequusHelpers.DropMoney(source, player.getRect(), Main.rand.Next(Item.silver * 50 + extraMoney / 2, Item.gold + extraMoney));
        }

        public static int SpawnLoot_ChooseRoulette(Player player, int i)
        {
            var choices = new List<int>();
            if (Main.rand.NextBool(3))
            {
                choices.Add(ModContent.ItemType<Roulette>());
                choices.Add(ModContent.ItemType<GoldenRoulette>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 2)
            {
                choices.Add(ModContent.ItemType<SnowRoulette>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 4)
            {
                choices.Add(ModContent.ItemType<DesertRoulette>());
                choices.Add(ModContent.ItemType<OceanSlotMachine>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 8)
            {
                choices.Add(ModContent.ItemType<JungleSlotMachine>());
                choices.Add(ModContent.ItemType<SkyRoulette>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 14 && AequusWorld.downedEventDemon)
            {
                choices.Add(ModContent.ItemType<ShadowRoulette>());
            }
            return choices.Count > 0 ? choices[Main.rand.Next(choices.Count)] : ItemID.None;
        }
    }
}