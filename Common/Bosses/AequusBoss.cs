using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Common.Bosses;

public class AequusBoss : ModNPC {
    public int TreasureBagId { get; protected set; }
    public int MaskId { get; protected set; }
    public int TrophyId { get; protected set; }
    public int RelicId { get; protected set; }
    public int ExpertItemId { get; protected set; }

    protected int LoadItem(ModItem modItem) {
        Mod.AddContent(modItem);
        return modItem.Type;
    }

    protected void AddMasterRelic(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(RelicId));
    }

    protected void AddTrophy(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(TrophyId));
    }

    protected void AddExpertItem(NPCLoot npcLoot) {
        (ItemLoader.GetItem(TreasureBagId) as BossBag)._rulesToRegister.Add(ItemDropRule.Common(ExpertItemId));
    }

    /// <summary>
    /// Adds all of the listed rules to the boss's Classic/Journey mode loot pool. To the boss's Treasure Bag's loot pool.
    /// </summary>
    /// <param name="npcLoot"></param>
    /// <param name="dropRules"></param>
    protected void AddBossDropItems(NPCLoot npcLoot, params IItemDropRule[] dropRules) {
        var notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        foreach (var dropRule in dropRules) {
            notExpertRule.OnSuccess(dropRule);
        }
        npcLoot.Add(notExpertRule);
        (ItemLoader.GetItem(TreasureBagId) as BossBag)._rulesToRegister.AddRange(dropRules);
    }
}