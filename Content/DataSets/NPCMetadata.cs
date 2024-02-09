using Aequus.Common.NPCs.Bestiary;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.DataSets;

public class NPCMetadata : MetadataSet {
    /// <summary>
    /// NPC Ids in this set cannot damage the Occultist, or have their souls trapped into Soul Gems.
    /// Automatically populated with all NPC Ids which have the Underworld or Gore Nest as bestiary tags.
    /// </summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> Soulless { get; private set; } = new();

    /// <summary>Automatically populated with all NPC Ids which have the Corruption as bestiary tags.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> IsCorrupt { get; private set; } = new();
    /// <summary>Automatically populated with all NPC Ids which have the Crimson as bestiary tags.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> IsCrimson { get; private set; } = new();
    /// <summary>Automatically populated with all NPC Ids which have the Hallow as bestiary tags.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> IsHallow { get; private set; } = new();
    /// <summary>Automatically populated with all NPC Ids which have a Pillar as a bestiary tag.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> FromPillarEvent { get; private set; } = new();

    /// <summary>Entries in this set can completely override the Name Tag conditional check with their own value.</summary>
    [JsonProperty]
    public static Dictionary<Entry<NPCID>, bool> NameTagOverride { get; private set; } = new();

    /// <summary>Entries in this set are able to be stunned by the Stun Gun (<see cref="Weapons.Classless.StunGun.StunGunDebuff"/>).</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> StunnableByTypeId { get; private set; } = new();
    /// <summary>Enemies with an AI Type in this set are able to be stunned by the Stun Gun (<see cref="Weapons.Classless.StunGun.StunGunDebuff"/>).</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCAIStyleID>> StunnableByAI { get; private set; } = new();

    /// <summary>Used for Royal Gel's Crown of Blood combination.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> FriendablePreHardmodeSlime { get; private set; } = new();

    /// <summary>Used for Volatile Gelatin's Crown of Blood combination.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> FriendableHardmodeSlime { get; private set; } = new();

    /// <summary>Enemies in this set cannot become friendly through necromancy.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> Unfriendable { get; private set; } = new();

    /// <summary>NPCs in this set deal 'heat' contact damage. This damage can be resisted using the Frost Potion.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> DealsHeatDamage { get; private set; } = new();

    /// <summary>NPCs in this set cannot be given Elite affixes.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> PrefixBlacklist { get; private set; } = new();

    /// <summary>Entries in this set are able to be pushed by the Pumpinator.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCID>> PushableByTypeId { get; private set; } = new();
    /// <summary>Enemies with an AI Type in this set are able to be pushed by the Pumpinator.</summary>
    [JsonProperty]
    public static HashSet<Entry<NPCAIStyleID>> PushableByAI { get; private set; } = new();

    #region Bestiary
    [JsonIgnore]
    public static List<IFilterInfoProvider> CorruptionTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IFilterInfoProvider> CrimsonTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IFilterInfoProvider> HallowTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IFilterInfoProvider> PillarTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IFilterInfoProvider> UnderworldTags { get; private set; } = new();

    /// <summary>
    /// Compares bestiary tags by comparing <see cref="IFilterInfoProvider.GetDisplayNameKey"/> results.
    /// Biome/Event/Time/Ect Tags utilize <see cref="IFilterInfoProvider"/>.
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="searchTags"></param>
    /// <returns></returns>
    private static bool ContainsBestiaryTags(List<IBestiaryInfoElement> tags, List<IFilterInfoProvider> searchTags) {
        return tags?.Where(t => t is IFilterInfoProvider).Select(t => (IFilterInfoProvider)t)
            .Any(t => searchTags.Any(s => t.GetDisplayNameKey() == s.GetDisplayNameKey())) ?? false;
    }
    #endregion

    #region Loading
    public override void SetStaticDefaults() {
        CorruptionTags.AddRange(new IFilterInfoProvider[] {
            BestiaryBuilder.Corruption,
            BestiaryBuilder.UndergroundCorruption,
            BestiaryBuilder.CorruptionDesert,
            BestiaryBuilder.CorruptionUndergroundDesert,
            BestiaryBuilder.CorruptionIce,
        });
        CrimsonTags.AddRange(new IFilterInfoProvider[] {
            BestiaryBuilder.Crimson,
            BestiaryBuilder.UndergroundCrimson,
            BestiaryBuilder.CrimsonDesert,
            BestiaryBuilder.CrimsonUndergroundDesert,
            BestiaryBuilder.CrimsonIce,
        });
        HallowTags.AddRange(new IFilterInfoProvider[] {
            BestiaryBuilder.Hallow,
            BestiaryBuilder.UndergroundHallow,
            BestiaryBuilder.HallowDesert,
            BestiaryBuilder.HallowUndergroundDesert,
            BestiaryBuilder.HallowIce,
        });
        PillarTags.AddRange(new IFilterInfoProvider[] {
            BestiaryBuilder.SolarPillar,
            BestiaryBuilder.VortexPillar,
            BestiaryBuilder.NebulaPillar,
            BestiaryBuilder.StardustPillar,
        });
        UnderworldTags.AddRange(new IFilterInfoProvider[] {
            BestiaryBuilder.Underworld,
        });

        // Make all of these NPCs immune to the vanilla "Slow" debuff.
        // Debuffs which modify movement speed should inherit this immunity.
        NPCSets.SpecificDebuffImmunity[NPCID.HallowBoss][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.CultistBoss][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.BloodEelBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.BloodEelTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.BoneSerpentBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.BoneSerpentTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.CultistDragonBody1][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.CultistDragonBody2][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.CultistDragonBody3][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.CultistDragonBody4][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.CultistDragonTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.DevourerBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.DevourerTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.DiggerBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.DiggerTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.DuneSplicerBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.DuneSplicerTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.EaterofWorldsBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.EaterofWorldsTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.GiantWormBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.GiantWormTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.LeechBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.LeechTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.SeekerBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.SeekerTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.SolarCrawltipedeBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.SolarCrawltipedeTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.StardustWormBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.StardustWormTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.TombCrawlerBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.TombCrawlerTail][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.WyvernBody][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.WyvernBody2][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.WyvernBody3][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.WyvernLegs][BuffID.Slow] = true;
        NPCSets.SpecificDebuffImmunity[NPCID.WyvernTail][BuffID.Slow] = true;
    }

    public override void AddRecipes() {
        for (int i = NPCID.NegativeIDCount + 1; i < NPCLoader.NPCCount; i++) {
            BestiaryEntry bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(i);
            if (bestiaryEntry == null || bestiaryEntry.Info == null) {
                continue;
            }

            List<IBestiaryInfoElement> info = bestiaryEntry.Info;
            if (ContainsBestiaryTags(info, CorruptionTags)) {
                IsCorrupt.Add(i);
            }
            if (ContainsBestiaryTags(info, CrimsonTags)) {
                IsCrimson.Add(i);
            }
            if (ContainsBestiaryTags(info, HallowTags)) {
                IsHallow.Add(i);
            }
            if (ContainsBestiaryTags(info, PillarTags)) {
                FromPillarEvent.Add(i);
            }
            if (ContainsBestiaryTags(info, UnderworldTags)) {
                Soulless.Add(i);
            }
        }
    }
    #endregion

    #region Methods
    public static bool IsUnholy(int npcId) {
        return IsCorrupt.Contains(npcId) || IsCrimson.Contains(npcId);
    }
    public static bool IsHoly(int npcId) {
        return IsHallow.Contains(npcId);
    }
    #endregion
}