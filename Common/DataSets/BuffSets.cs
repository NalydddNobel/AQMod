using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets;

public class BuffSets : DataSet {
    protected override ContentFileInfo ContentFileInfo => new(BuffID.Search);

    public static readonly HashSet<int> ClearableDebuff = new();
    public static readonly HashSet<int> PotionPrefixBlacklist = new();
    public static readonly HashSet<int> NotTypicalDebuff = new();
    public static readonly HashSet<int> DontChangeDuration = new();
    public static readonly HashSet<int> ProbablyFireDebuff = new();
    public static readonly HashSet<int> ProbablyCooldownDebuff = new();
    /// <summary>
    /// Set for compatibility with Thorium.
    /// </summary>
    public static readonly HashSet<int> PlayerStatusDebuff = new();
    /// <summary>
    /// Set for compatibility with Thorium.
    /// </summary>
    public static readonly HashSet<int> PlayerDoTDebuff = new();

    public static List<int> DemonSiegeImmune = new();

    public static Dictionary<int, List<int>> BuffConflicts = new();

    public override void PostSetupContent() {
        for (int i = 0; i < BuffLoader.BuffCount; i++) {
            if (Main.debuff[i]) {
                PotionPrefixBlacklist.Add(i);
                if (BuffID.Sets.NurseCannotRemoveDebuff[i]) {
                    NotTypicalDebuff.Add(i);
                }
                else if ((i < BuffID.NeutralHunger || i > BuffID.Starving) && !BuffID.Sets.IsATagBuff[i] && !BuffID.Sets.TimeLeftDoesNotDecrease[i]) {
                    ClearableDebuff.Add(i);
                    if (BuffID.Search.TryGetName(i, out string name)) {
                        if (name.Contains('/')) {
                            name = name.Split('/')[^1];
                        }
                        if (name.Contains("Cooldown")) {
                            ProbablyCooldownDebuff.Add(i);
                            DontChangeDuration.Add(i);
                            NotTypicalDebuff.Add(i);
                        }
                        else if (name.Contains("Fire") || name.Contains("fire") || name.Contains("Burn") || name.Contains("Flame") || name.Contains("flame") || name.Contains("Inferno") || name.Contains("Blaze")) {
                            ProbablyFireDebuff.Add(i);
                        }
                    }
                }
            }
            if (BuffID.Sets.IsFedState[i] || BuffID.Sets.IsWellFed[i]) {
                PotionPrefixBlacklist.Add(i);
            }
        }
    }

    private static void AddBuffConflictsInner(int buffID, int conflictor) {
        if (!BuffConflicts.ContainsKey(buffID)) {
            BuffConflicts[buffID] = new List<int>() { conflictor };
            return;
        }
        if (BuffConflicts[buffID].Contains(conflictor))
            return;
        BuffConflicts[buffID].Add(conflictor);
    }
    public static void AddBuffConflicts(int buffID, int buffID2) {
        AddBuffConflictsInner(buffID, buffID2);
        AddBuffConflictsInner(buffID2, buffID);
    }
}