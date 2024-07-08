using Terraria.Localization;

namespace AequusRemake.Core.Localization;

public class NPCCondition() : IDialogueCondition {
    private bool TryGetNPCID(string name, out int id) {
        // First search the name for a matching name-ID pair.
        if (NPCID.Search.TryGetId(name, out id)) {
            return true;
        }

        // Search again but with "Aequus/" appended behind it.
        if (NPCID.Search.TryGetId($"Aequus/{name}", out id)) {
            return true;
        }

        return false;
    }

    bool IDialogueCondition.IsMet(LocalizedText text, string name) {
        if (TryGetNPCID(name, out int id)) {
            return NPC.AnyNPCs(id);
        }
        return false;
    }
}
