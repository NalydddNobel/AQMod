using AequusRemake.Core.Structures.Conditions;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.Utilities;

namespace AequusRemake.Core.Localization;

public class DialogueSelector {
    public static readonly DialogueSelector Instance = new DialogueSelector();

    public readonly Dictionary<string, IDialogueCondition> ConditionKeys = new() {
        ["Night"] = new DialogueCondition(Condition.TimeNight),
        ["Graveyard"] = new DialogueCondition(Condition.InGraveyard),
        ["Thunderstorm"] = new DialogueCondition(Condition.Thunderstorm),
        ["BloodMoon"] = new DialogueCondition(Condition.BloodMoon),
        ["Glimmer"] = new DialogueCondition(ACondition.InGlimmer),
        ["Eclipse"] = new DialogueCondition(Condition.Eclipse),
        ["WindyDay"] = new DialogueCondition(Condition.HappyWindyDay),
        ["Rain"] = new DialogueCondition(Condition.InRain),
        ["Party"] = new DialogueCondition(Condition.BirthdayParty),

        ["DownedEyeOfCthulhu"] = new DialogueCondition(Condition.DownedEyeOfCthulhu),
        ["DownedEvilBoss"] = new DialogueCondition(Condition.DownedEowOrBoc),
        ["DownedSkeletron"] = new DialogueCondition(Condition.DownedSkeletron),
        ["DownedEarlygameBoss"] = new DialogueCondition(Condition.DownedEarlygameBoss),
        ["Hardmode"] = new DialogueCondition(Condition.Hardmode),
        ["DownedGolem"] = new DialogueCondition(Condition.DownedGolem),
        ["DownedMartians"] = new DialogueCondition(Condition.DownedMartians),

        //["EarlyHardmode"] = new DialogueCondition(ACondition.New(...)),
        //["PylonNearby"] = new DialogueCondition(ACondition.New(...)),
        //["LunarPillars"] = new DialogueCondition(ACondition.New(...)),

        ["Rare"] = new RareCondition(),
        ["NPC"] = new NPCCondition(),
        ["Mod"] = new ModIsEnabledCondition(),
    };

    public LocalizedText GetRandomValidText(string selectionKey, UnifiedRandom random = null) {
        IEnumerable<LocalizedText> allocEnumText = GetValid(selectionKey);
        LocalizedText[] allocArrayText = allocEnumText.ToArray();

        // Default to Main.rand if null.
        random ??= Main.rand;

        return random.Next(allocArrayText);
    }

    public IEnumerable<LocalizedText> GetValid(string selectionKey) {
        LocalizedText[] allocArr = LanguageManager.Instance.FindAll((compareKey, text) => compareKey.StartsWith(selectionKey));

        for (int i = 0; i < allocArr.Length; i++) {
            string textKey = allocArr[i].Key;

            string trimmedKey = textKey[selectionKey.Length..];

            string[] keyConditions = trimmedKey.Split('.');

            if (CheckConditions(allocArr[i], keyConditions)) {
                yield return allocArr[i];
            }
        }

        bool CheckConditions(LocalizedText value, string[] conditions) {
            for (int i = 0; i < conditions.Length; i++) {
                string conditionValue, conditionName;
                conditionValue = conditionName = conditions[i];

                // Split key by the underscore.
                // For example, NPC_Merchant would be split into a condition key of "NPC" with value "Merchant"
                int underscoreIndex = conditionName.IndexOf('_');
                if (underscoreIndex != -1 && underscoreIndex < conditionValue.Length - 2) {
                    conditionValue = conditionName[(underscoreIndex + 1)..];
                    conditionName = conditionName[..underscoreIndex];
                }

                if (ConditionKeys.TryGetValue(conditionName, out IDialogueCondition condition)) {
                    if (!condition.IsMet(value, conditionValue)) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
