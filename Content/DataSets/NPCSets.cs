using Aequus.Common.NPCs;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.DataSets;

public partial class NPCSets : DataSet {
    [JsonProperty]
    public static HashSet<NPCEntry> IsCorrupt { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCEntry> IsCrimson { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCEntry> IsHallow { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCEntry> FromPillarEvent { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<NPCEntry, bool> NameTagOverride { get; private set; } = new();

    [JsonProperty]
    public static HashSet<NPCEntry> StunnableByTypeId { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCAIEntry> StunnableByAI { get; private set; } = new();

    /// <summary>
    /// NPCs in this set cannot get speed increases or decreases. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> StatSpeedBlacklist { get; private set; } = new();

    /// <summary>
    /// Used for Royal Gel's Crown of Blood combination.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> FriendablePreHardmodeSlime { get; private set; } = new();

    /// <summary>
    /// Used for Volatile Gelatin's Crown of Blood combination.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> FriendableHardmodeSlime { get; private set; } = new();

    /// <summary>
    /// Enemies in this set cannot become friendly. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> Unfriendable { get; private set; } = new();

    /// <summary>
    /// NPCs in this set deal 'heat' contact damage. This damage can be resisted using the Frost Potion.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> DealsHeatDamage { get; private set; } = new();

    /// <summary>
    /// NPCs in this set cannot become Elites. This usually contains bosses.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> PrefixBlacklist { get; private set; } = new();

    [JsonProperty]
    public static HashSet<NPCEntry> PushableByTypeId { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCAIEntry> PushableByAI { get; private set; } = new();

    #region Bestiary
    public static List<IBestiaryInfoElement> CorruptionElements { get; private set; } = new();
    public static List<IBestiaryInfoElement> CrimsonElements { get; private set; } = new();
    public static List<IBestiaryInfoElement> HallowElements { get; private set; } = new();
    public static List<IBestiaryInfoElement> PillarElements { get; private set; } = new();

    private void LoadBestiaryElementTypes() {
        CorruptionElements.AddRange(new[] {
            BestiaryBuilder.Corruption,
            BestiaryBuilder.UndergroundCorruption,
            BestiaryBuilder.CorruptionDesert,
            BestiaryBuilder.CorruptionUndergroundDesert,
            BestiaryBuilder.CorruptionIce,
        });
        CrimsonElements.AddRange(new[] {
            BestiaryBuilder.Crimson,
            BestiaryBuilder.UndergroundCrimson,
            BestiaryBuilder.CrimsonDesert,
            BestiaryBuilder.CrimsonUndergroundDesert,
            BestiaryBuilder.CrimsonIce,
        });
        HallowElements.AddRange(new[] {
            BestiaryBuilder.Hallow,
            BestiaryBuilder.UndergroundHallow,
            BestiaryBuilder.HallowDesert,
            BestiaryBuilder.HallowUndergroundDesert,
            BestiaryBuilder.HallowIce,
        });
        PillarElements.AddRange(new[] {
            BestiaryBuilder.SolarPillar,
            BestiaryBuilder.VortexPillar,
            BestiaryBuilder.NebulaPillar,
            BestiaryBuilder.StardustPillar,
        });
    }
    private bool CheckBestiary(List<IBestiaryInfoElement> info, List<IBestiaryInfoElement> compareInfo) {
        foreach (var i in info) {
            if (i is SpawnConditionBestiaryInfoElement spawn) {
                foreach (var j in compareInfo) {
                    if (j is not SpawnConditionBestiaryInfoElement compareSpawn) {
                        continue;
                    }

                    if (spawn.GetDisplayNameKey() == compareSpawn.GetDisplayNameKey()) {
                        return true;
                    }
                }
                continue;
            }

            var type = i.GetType();
            foreach (var compare in compareInfo) {
                if (type == compare.GetType()) {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region Loading
    public override void Load() {
        LoadBestiaryElementTypes();
    }

    public override void PostSetupContent() {
        for (int i = 0; i < NPCLoader.NPCCount; i++) {
            var npc = ContentSamples.NpcsByNetId[i];
            if (NPCHelper.IsImmune(i, BuffID.Slow)) {
                StatSpeedBlacklist.Add((NPCEntry)i);
            }
        }
    }

    public override void AddRecipes() {
        for (int i = NPCID.NegativeIDCount + 1; i < NPCLoader.NPCCount; i++) {
            var bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(i);
            if (bestiaryEntry == null || bestiaryEntry.Info == null) {
                continue;
            }

            var info = bestiaryEntry.Info;
            if (CheckBestiary(info, CorruptionElements)) {
                IsCorrupt.Add((NPCEntry)i);
            }
            if (CheckBestiary(info, CrimsonElements)) {
                IsCrimson.Add((NPCEntry)i);
            }
            if (CheckBestiary(info, HallowElements)) {
                IsHallow.Add((NPCEntry)i);
            }
            if (CheckBestiary(info, PillarElements)) {
                FromPillarEvent.Add((NPCEntry)i);
            }
        }
    }
    #endregion

    #region Methods
    public bool IsUnholy(int npcId) {
        return IsCorrupt.Contains(npcId) || IsCrimson.Contains(npcId);
    }
    public bool IsHoly(int npcId) {
        return IsHallow.Contains(npcId);
    }
    #endregion
}