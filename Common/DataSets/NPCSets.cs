using Aequus.Common.Entities.Bestiary;
using Aequus.Common.NPCs;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace Aequus.Common.DataSets;

public class NPCSets : DataSet {
    protected override ContentFileInfo ContentFileInfo => new(NPCID.Search);

    public static readonly List<int> Eclipse = [NPCID.Vampire, NPCID.VampireBat, NPCID.MothronEgg, NPCID.MothronSpawn];
    public static readonly List<int> Glimmer = [];
    public static readonly List<int> BloodMoon = [];

    public static readonly Dictionary<int, bool> PickpocketOverride = [];
    public static readonly Dictionary<int, bool> NameTagOverride = new();
    /// <summary>
    /// NPCs in this set cannot get speed increases or decreases. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    public static readonly HashSet<int> StatSpeedBlacklist = new();
    public static readonly HashSet<int> IsCorrupt = new();
    public static readonly HashSet<int> IsCrimson = new();
    public static readonly HashSet<int> IsHallow = new();
    public static readonly HashSet<int> FromPillarEvent = new();
    /// <summary>
    /// Used for Royal Gel's Crown of Blood combination.
    /// </summary>
    public static readonly HashSet<int> FriendablePreHardmodeSlime = new();
    /// <summary>
    /// Used for Volatile Gelatin's Crown of Blood combination.
    /// </summary>
    public static readonly HashSet<int> FriendableHardmodeSlime = new();
    /// <summary>
    /// Enemies in this set cannot become friendly. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    public static readonly HashSet<int> Unfriendable = new();
    /// <summary>
    /// NPCs in this set deal 'heat' contact damage. This damage can be resisted using the Frost Potion.
    /// </summary>
    public static readonly HashSet<int> DealsHeatDamage = new();
    /// <summary>
    /// NPCs in this set cannot become Elites. This usually contains bosses.
    /// </summary>
    public static readonly HashSet<int> ElitePrefixBlacklist = new();

    #region Bestiary
    public static List<IBestiaryInfoElement> CorruptionElements = new();
    public static List<IBestiaryInfoElement> CrimsonElements = new();
    public static List<IBestiaryInfoElement> HallowElements = new();
    public static List<IBestiaryInfoElement> PillarElements = new();

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
                    if (j is not SpawnConditionBestiaryInfoElement compareSpawn) continue;
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
    public override void OnLoad(Mod mod) {
        LoadBestiaryElementTypes();
        NameTagOverride[NPCID.EaterofWorldsBody] = false;
        NameTagOverride[NPCID.EaterofWorldsTail] = false;
    }

    public override void PostSetupContent() {
        // Make all of these NPCs immune to the vanilla "Slow" debuff.
        // Debuffs which modify movement speed should inherit this immunity.
        NPCID.Sets.SpecificDebuffImmunity[NPCID.HallowBoss][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistBoss][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BloodEelBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BloodEelTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BoneSerpentBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BoneSerpentTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody1][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody2][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody3][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody4][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DevourerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DevourerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DiggerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DiggerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DuneSplicerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DuneSplicerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.GiantWormBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.GiantWormTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.LeechBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.LeechTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SeekerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SeekerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SolarCrawltipedeBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SolarCrawltipedeTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.StardustWormBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.StardustWormTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.TombCrawlerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.TombCrawlerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernBody2][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernBody3][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernLegs][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernTail][BuffID.Slow] = true;

        for (int i = 0; i < NPCLoader.NPCCount; i++) {
            var npc = ContentSamples.NpcsByNetId[i];
            if (npc.boss || Helper.IsImmune(i, BuffID.Weak, BuffID.Slow)) {
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
            if (BestiaryTagCollection.BloodMoon.ContainsNPCIdInner(i)) {
                BloodMoon.Add(i);
            }
            if (BestiaryTagCollection.Eclipse.ContainsNPCIdInner(i)) {
                Eclipse.Add(i);
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