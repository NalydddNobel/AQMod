using Aequus.Items.Accessories.Misc.Info;
using Aequus.Items.Misc.Bait;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus {
    public partial class AequusPlayer : ModPlayer
    {
        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            if (Main.rand.Next(8) <= (Player.anglerQuestsFinished / 4) || Player.anglerQuestsFinished <= 1)
            {
                if (Main.rand.NextBool())
                {
                    return;
                }

                for (int i = 0; i < rewardItems.Count; i++)
                {
                    if (rewardItems[i].type == ItemID.ApprenticeBait || rewardItems[i].type == ItemID.JourneymanBait || rewardItems[i].type == ItemID.MasterBait)
                    {
                        rewardItems.RemoveAt(i);
                        break;
                    }
                }

                var item = new Item();
                switch (Main.rand.Next(3))
                {
                    case 0:
                        item.SetDefaults(ModContent.ItemType<Omnibait>());
                        break;
                    case 1:
                        item.SetDefaults(ModContent.ItemType<LegendberryBait>());
                        break;
                    case 2:
                        item.SetDefaults(ModContent.ItemType<CrateBait>());
                        break;
                }

                if (Main.rand.Next(25) <= Player.anglerQuestsFinished)
                {
                    item.stack++;
                }
                for (int i = 0; i < 5; i++)
                {
                    if (Main.rand.Next(50 + i * 50) <= Player.anglerQuestsFinished)
                    {
                        item.stack++;
                    }
                }

                rewardItems.Add(item);
            }
        }

        public void LegendaryFishRewards(NPC npc, Item item, int i)
        {
            long money = Main.rand.Next(Item.gold * 4, Item.gold * 6);
            Player.DropFromItem(item.type);
            var source = npc.GetSource_GiftOrReward();
            if (!Player.HasItemCheckAllBanks(ModContent.ItemType<AnglerBroadcaster>()))
            {
                Player.QuickSpawnItem(source, ModContent.ItemType<AnglerBroadcaster>());
            }
            Player.QuickSpawnItem(source, Main.rand.NextBool() ? ModContent.ItemType<Omnibait>() : ModContent.ItemType<LegendberryBait>(), Main.rand.Next(4) + 1);
            Helper.DropMoney(source, Player.getRect(), money, quiet: false);
        }
    }
}