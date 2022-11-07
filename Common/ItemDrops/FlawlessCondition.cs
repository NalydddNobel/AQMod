using Aequus.NPCs;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class FlawlessCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool isHidden;

        public virtual bool CanDrop(DropAttemptInfo info)
        {
            if (info.npc != null)
            {
                var flags = info.npc.GetGlobalNPC<FlawlessGlobalNPC>().damagedPlayers;
                for (int i = 0; i < flags.Length; i++)
                {
                    if (info.npc.playerInteraction[i] && !flags[i])
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI()
        {
            return !isHidden;
        }

        public virtual string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.Flawless");
        }
    }
}
