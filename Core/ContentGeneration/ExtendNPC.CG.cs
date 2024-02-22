using Aequus.Core.CodeGeneration;
using Aequus.Core.ContentGeneration;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Utilities;

public static partial class ExtendNPC {
    /// <summary>Adds <paramref name="entry"/> to <paramref name="npcLoot"/>, and its boss bag.</summary>
    /// <param name="npcLoot"></param>
    /// <param name="entry"></param>
    public static void AddBossDrop(this NPCLoot npcLoot, IItemDropRule entry) {
        var normalModeRule = new LeadingConditionRule(new Conditions.NotExpert());
        normalModeRule.OnSuccess(entry);

        npcLoot.Add(normalModeRule);
        AddToBossBag(npcLoot, entry);
    }
    /// <summary>Adds <paramref name="entry"/> to <paramref name="npcLoot"/>'s boss bag. Throws errors if a boss bag doesn't exist.</summary>
    public static void AddToBossBag(this NPCLoot npcLoot, IItemDropRule entry) {
        int npcNetId = Publicization<NPCLoot, int>.Get(npcLoot, "npcNetId");
        int treasureBag = AutoNPCDefaults._npcToBossBag[ModContent.GetModNPC(npcNetId)].Type;
        ItemDropDatabase database = Publicization<NPCLoot, ItemDropDatabase>.Get(npcLoot, "itemDropDatabase");
        database.RegisterToItem(treasureBag, entry);
    }
}
