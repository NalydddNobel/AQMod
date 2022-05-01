using Aequus.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class FlawlessDrop : IItemDropRule, IItemDropRuleCondition, IProvideItemConditionDescription
    {
        private readonly int ItemType;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public FlawlessDrop(int item)
        {
            ItemType = item;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            ratesInfo.conditions = new List<IItemDropRuleCondition>() { this, };
            drops.Add(new DropRateInfo(ItemType, 1, 1, ratesInfo.parentDroprateChance, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            var result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.DoesntFillConditions;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (info.npc.playerInteraction[Main.myPlayer] && !info.npc.GetGlobalNPC<FlawlessNPC>().damagedPlayers[Main.myPlayer])
                {
                    CommonCode.DropItemFromNPC(info.npc, ItemType, 1);
                    result.State = ItemDropAttemptResultState.Success;
                }
            }
            else
            {
                int item = Item.NewItem(new EntitySource_Parent(info.npc), info.npc.getRect(), ItemType, 1, noBroadcast: true);
                Main.timeItemSlotCannotBeReusedFor[item] = 54000;
                var flags = info.npc.GetGlobalNPC<FlawlessNPC>().damagedPlayers;
                for (int i = 0; i < 255; i++)
                {
                    var plr = Main.player[i];
                    if (plr.active && info.npc.playerInteraction[i] && !flags[i])
                    {
                        NetMessage.SendData(MessageID.InstancedItem, i, -1, null, item);
                        result.State = ItemDropAttemptResultState.Success;
                    }
                }
                Main.item[item].active = false;
            }
            return result;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.Flawless");
        }
    }
}
