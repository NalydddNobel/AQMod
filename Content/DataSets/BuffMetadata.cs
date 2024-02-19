using Aequus.Core.DataSets;
using Aequus.Core.Initialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Content.DataSets;

public class BuffMetadata : MetadataSet {
    /// <summary>Buffs in this set should not have their duration altered, like the Luck Potion effect.</summary>
    [JsonProperty]
    public static HashSet<Entry<BuffID>> CannotChangeDuration { get; private set; } = new();
    /// <summary>Set for compatibility with Thorium.</summary>
    [JsonProperty]
    public static HashSet<Entry<BuffID>> PlayerStatusDebuff { get; private set; } = new();
    /// <summary>Set for compatibility with Thorium.</summary>
    [JsonProperty]
    public static HashSet<Entry<BuffID>> PlayerDoTDebuff { get; private set; } = new();

    [JsonIgnore]
    public static Dictionary<Entry<BuffID>, List<Entry<BuffID>>> BuffConflicts { get; private set; } = new();

    private static void AddBuffConflictsInner(int buffID, int conflictor) {
        (CollectionsMarshal.GetValueRefOrAddDefault(BuffConflicts, buffID, out _) ??= new()).Add(conflictor);
    }

    public static void AddBuffConflicts(int buffID, int buffID2) {
        LoadingSteps.EnqueuePostSetupContent(() => {
            AddBuffConflictsInner(buffID, buffID2);
            AddBuffConflictsInner(buffID2, buffID);
        });
    }
}