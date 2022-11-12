using Aequus.NPCs;
using System;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class NameTagCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public string[] validNames;

        public NameTagCondition(params string[] names)
        {
            validNames = names;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return info.npc != null && info.npc.TryGetGlobalNPC<NPCNameTag>(out var nameTag) && nameTag.HasNameTag && validNames.Contains(nameTag.NameTag.ToLower());
        }

        public bool CanShowItemDropInUI()
        {
            return false;
        }

        public string GetConditionDescription()
        {
            string value = "";
            for (int i = 0; i < validNames.Length; i++)
            {
                if (i != 0)
                    value += ", ";
                value += $"'{validNames[i]}'";
            }
            return Language.GetTextValueWith("Mods.Aequus.DropCondition.NameTag", new { NameTag = value, });
        }


    }
}