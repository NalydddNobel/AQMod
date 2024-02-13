using Aequus.Core.DataSets;
using Aequus.Core.Initialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Content.DataSets;

public class BuffMetadata : MetadataSet {
    [JsonProperty]
    public static HashSet<Entry<BuffID>> PotionPrefixBlacklist { get; private set; } = new();
    [JsonProperty]
    public static HashSet<Entry<BuffID>> NotTypicalDebuff { get; private set; } = new();
    [JsonProperty]
    public static HashSet<Entry<BuffID>> DontChangeDuration { get; private set; } = new();
    [JsonProperty]
    public static HashSet<Entry<BuffID>> CooldownDebuff { get; private set; } = new();
    /// <summary>Set for compatibility with Thorium.</summary>
    [JsonProperty]
    public static HashSet<Entry<BuffID>> PlayerStatusDebuff { get; private set; } = new();
    /// <summary>Set for compatibility with Thorium.</summary>
    [JsonProperty]
    public static HashSet<Entry<BuffID>> PlayerDoTDebuff { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<Entry<BuffID>, List<Entry<BuffID>>> BuffConflicts { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<Entry<BuffID>, bool> IsFireDebuff { get; private set; } = new();

    [JsonProperty]
    internal static List<Entry<BuffID>> DemonSiegeImmune { get; private set; } = new();

    public override void PostSetupContent() {
        for (int i = 0; i < BuffLoader.BuffCount; i++) {
            if (Main.debuff[i]) {
                PotionPrefixBlacklist.Add(i);
                if (BuffSets.NurseCannotRemoveDebuff[i]) {
                    NotTypicalDebuff.Add(i);
                }
                else if ((i < BuffID.NeutralHunger || i > BuffID.Starving) && !BuffSets.IsATagBuff[i] && !BuffSets.TimeLeftDoesNotDecrease[i]) {
                    if (BuffID.Search.TryGetName(i, out string name)) {
                        if (name.Contains('/')) {
                            name = name.Split('/')[^1];
                        }
                        if (name.Contains("Cooldown")) {
                            CooldownDebuff.Add(i);
                            DontChangeDuration.Add(i);
                            NotTypicalDebuff.Add(i);
                        }
                        else if (name.Contains("Fire") || name.Contains("fire") || name.Contains("Burn") || name.Contains("Flame") || name.Contains("flame") || name.Contains("Inferno") || name.Contains("Blaze")) {
                            IsFireDebuff.TryAdd(i, true);
                        }
                    }
                }
            }
            if (BuffSets.IsFedState[i] || BuffSets.IsWellFed[i]) {
                PotionPrefixBlacklist.Add(i);
            }
        }
    }

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