using Aequus.Core.DataSets;
using Aequus.Core.Initialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Content.DataSets;

public class BuffSets : DataSet {
    public BuffSets() : base() {
    }

    [JsonProperty]
    public static HashSet<BuffEntry> PotionPrefixBlacklist { get; private set; } = new();
    [JsonProperty]
    public static HashSet<BuffEntry> NotTypicalDebuff { get; private set; } = new();
    [JsonProperty]
    public static HashSet<BuffEntry> DontChangeDuration { get; private set; } = new();
    [JsonProperty]
    public static HashSet<BuffEntry> CooldownDebuff { get; private set; } = new();
    /// <summary>
    /// Set for compatibility with Thorium.
    /// </summary>
    [JsonProperty]
    public static HashSet<BuffEntry> PlayerStatusDebuff { get; private set; } = new();
    /// <summary>
    /// Set for compatibility with Thorium.
    /// </summary>
    [JsonProperty]
    public static HashSet<BuffEntry> PlayerDoTDebuff { get; private set; } = new();

    [JsonProperty]
    public static List<BuffEntry> ModifiesMoveSpeed { get; private set; } = new();

    [JsonProperty]
    public static List<BuffEntry> DemonSiegeImmune { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<BuffEntry, List<BuffEntry>> BuffConflicts { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<BuffEntry, bool> IsFireDebuff { get; private set; } = new();

    public override void PostSetupContent() {
        for (int i = 0; i < BuffLoader.BuffCount; i++) {
            if (Main.debuff[i]) {
                PotionPrefixBlacklist.Add((BuffEntry)i);
                if (BuffID.Sets.NurseCannotRemoveDebuff[i]) {
                    NotTypicalDebuff.Add((BuffEntry)i);
                }
                else if ((i < BuffID.NeutralHunger || i > BuffID.Starving) && !BuffID.Sets.IsATagBuff[i] && !BuffID.Sets.TimeLeftDoesNotDecrease[i]) {
                    if (BuffID.Search.TryGetName(i, out string name)) {
                        if (name.Contains('/')) {
                            name = name.Split('/')[^1];
                        }
                        if (name.Contains("Cooldown")) {
                            CooldownDebuff.Add((BuffEntry)i);
                            DontChangeDuration.Add((BuffEntry)i);
                            NotTypicalDebuff.Add((BuffEntry)i);
                        }
                        else if (name.Contains("Fire") || name.Contains("fire") || name.Contains("Burn") || name.Contains("Flame") || name.Contains("flame") || name.Contains("Inferno") || name.Contains("Blaze")) {
                            IsFireDebuff.TryAdd((BuffEntry)i, true);
                        }
                    }
                }
            }
            if (BuffID.Sets.IsFedState[i] || BuffID.Sets.IsWellFed[i]) {
                PotionPrefixBlacklist.Add((BuffEntry)i);
            }
        }
    }

    private static void AddBuffConflictsInner(int buffID, int conflictor) {
        (CollectionsMarshal.GetValueRefOrAddDefault(BuffConflicts, (BuffEntry)buffID, out _) ??= new()).Add((BuffEntry)conflictor);
    }

    public static void AddBuffConflicts(int buffID, int buffID2) {
        LoadingSteps.EnqueuePostSetupContent(() => {
            AddBuffConflictsInner(buffID, buffID2);
            AddBuffConflictsInner(buffID2, buffID);
        });
    }
}