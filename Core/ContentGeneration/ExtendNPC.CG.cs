using Aequus.Core.CodeGeneration;
using System;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Utilities;

public static partial class ExtendNPC {
    /// <summary>Adds <paramref name="entry"/> to <paramref name="npcLoot"/>, and its boss bag.</summary>
    /// <param name="npcLoot"></param>
    /// <param name="entry"></param>
    /// <param name="throwError">Whether or not to throw an error if a Treasure Bag does not exist. Defaults to true.</param>
    public static void AddBossDrop(this NPCLoot npcLoot, IItemDropRule entry, bool throwError = true) {
        var normalModeRule = new LeadingConditionRule(new Conditions.NotExpert());
        normalModeRule.OnSuccess(entry);

        npcLoot.Add(normalModeRule);
        AddToBossBag(npcLoot, entry, throwError: throwError);
    }
    /// <summary>Adds <paramref name="entry"/> to <paramref name="npcLoot"/>'s boss bag. Throws errors if a boss bag doesn't exist.</summary>
    public static void AddToBossBag(this NPCLoot npcLoot, IItemDropRule entry, bool throwError = true) {
        int npcNetId = Publicization<NPCLoot, int>.GetField(npcLoot, "npcNetId");

        ModNPC npc = ModContent.GetModNPC(npcNetId);
        ModItem treasureBag = npc.Mod.Find<ModItem>($"{npc.Name}Bag");
        if (treasureBag != null) {
            ItemDropDatabase database = Publicization<NPCLoot, ItemDropDatabase>.GetField(npcLoot, "itemDropDatabase");
            database.RegisterToItem(treasureBag.Type, entry);
        }
        else if (throwError) {
            throw new Exception($"NPC {npc.Name} does not have a treasure bag registered.");
        }
    }
}
