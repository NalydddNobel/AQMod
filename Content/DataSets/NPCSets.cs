using Aequus.Common.NPCs;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DataSets;

[DataID(typeof(NPCID))]
public partial class NPCSets : DataSet {
    public static readonly HashSet<int> IsCorrupt = new();
    public static readonly HashSet<int> IsCrimson = new();
    public static readonly HashSet<int> IsHallow = new();
    public static readonly HashSet<int> FromPillarEvent = new();

    [JsonProperty]
    public static DataIDKeyDictionary<bool> NameTagOverride;

    [JsonProperty]
    public static DataIDValueSet StunnableByTypeId;
    [DataID(typeof(NPCAIStyleID))]
    [JsonProperty]
    public static DataIDValueSet StunnableByAI;

    /// <summary>
    /// NPCs in this set cannot get speed increases or decreases. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet StatSpeedBlacklist;

    /// <summary>
    /// Used for Royal Gel's Crown of Blood combination.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet FriendablePreHardmodeSlime;

    /// <summary>
    /// Used for Volatile Gelatin's Crown of Blood combination.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet FriendableHardmodeSlime;

    /// <summary>
    /// Enemies in this set cannot become friendly. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet Unfriendable;

    /// <summary>
    /// NPCs in this set deal 'heat' contact damage. This damage can be resisted using the Frost Potion.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet DealsHeatDamage;

    /// <summary>
    /// NPCs in this set cannot become Elites. This usually contains bosses.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet PrefixBlacklist;

    [JsonProperty]
    public static DataIDValueSet PushableByTypeId;
    [DataID(typeof(NPCAIStyleID))]
    [JsonProperty]
    public static DataIDValueSet PushableByAI;

    #region Bestiary
    public static readonly List<IBestiaryInfoElement> CorruptionElements = new();
    public static readonly List<IBestiaryInfoElement> CrimsonElements = new();
    public static readonly List<IBestiaryInfoElement> HallowElements = new();
    public static readonly List<IBestiaryInfoElement> PillarElements = new();

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
                StatSpeedBlacklist.Add(i);
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
                IsCorrupt.Add(i);
            }
            if (CheckBestiary(info, CrimsonElements)) {
                IsCrimson.Add(i);
            }
            if (CheckBestiary(info, HallowElements)) {
                IsHallow.Add(i);
            }
            if (CheckBestiary(info, PillarElements)) {
                FromPillarEvent.Add(i);
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