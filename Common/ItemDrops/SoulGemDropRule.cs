using Aequus.Content.Necromancy;
using Aequus.Items.Consumables.SoulGems;
using Aequus.Items.Weapons;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops
{
    public class SoulGemDropRule : IItemDropRule
    {
        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public SoulGemDropRule()
        {
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.npc != null && info.player != null)
            {
                return info.player.FindItemInInvOrVoidBag((item) => item.ModItem is SoulGemWeaponBase soulGemWeapon && soulGemWeapon.tier >= NecromancyDatabase.GetSoulGemTier(info.npc.netID), out bool inVoidBag) != null && !inVoidBag;
            }
            return false;
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            int tier = NecromancyDatabase.GetSoulGemTier(info.npc.netID);
            if (tier < 0 || tier > SoulGemWeaponBase.MaxTier)
            {
                return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.DoesntFillConditions, };
            }
            if (info.rng.NextBool(35 - 7 * (tier - 1)))
            {
                var l = new List<SoulGemBase>(SoulGemBase.RegisteredSoulGems);
                while (l.Count > 0)
                {
                    int index = info.rng.Next(l.Count);
                    if (l[index].Tier == tier)
                    {
                        CommonCode.DropItem(info, l[index].Type, 1);
                        return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.Success, };
                    }
                    l.RemoveAt(index);
                }
            }

            return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.FailedRandomRoll, };
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            var ratesInfo2 = ratesInfo.With(1f);
            float num = 1f;

            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo2);
        }
    }
}