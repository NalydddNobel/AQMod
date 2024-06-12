using Aequus.Common.Bestiary;
using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.DataSets;

public class NPCDataSet : DataSet {
    /// <summary>NPCs in this set will not inherit elements from their Bestiary Tags.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> NoBestiaryElementInheritence { get; private set; } = [];

    /// <summary>NPCs in this set will not pass down their elements to items in their loot pool.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> NoDropElementInheritence { get; private set; } = [
        NPCID.Zombie,
        NPCID.BaldZombie,
        NPCID.PincushionZombie,
        NPCID.SlimedZombie,
        NPCID.SwampZombie,
        NPCID.TwiggyZombie,
        NPCID.FemaleZombie,
        NPCID.ZombieDoctor,
        NPCID.ZombiePixie,
        NPCID.ZombieRaincoat,
        NPCID.ZombieSuperman,
        NPCID.ZombieSweater,
        NPCID.ZombieXmas,
        NPCID.TorchZombie,
        NPCID.ZombieEskimo,
        NPCID.ArmedZombie,
        NPCID.ArmedZombieCenx,
        NPCID.ArmedZombieEskimo,
        NPCID.ArmedZombiePincussion,
        NPCID.ArmedZombieSlimed,
        NPCID.ArmedZombieSwamp,
        NPCID.ArmedZombieTwiggy,
        NPCID.ArmedTorchZombie,
        NPCID.Skeleton,
        NPCID.SkeletonAlien,
        NPCID.SkeletonArcher,
        NPCID.SkeletonAstonaut,
        NPCID.SkeletonTopHat,
        NPCID.ArmoredSkeleton,
        NPCID.HeadacheSkeleton,
        NPCID.BoneThrowingSkeleton,
        NPCID.BoneThrowingSkeleton2,
        NPCID.BoneThrowingSkeleton3,
        NPCID.BoneThrowingSkeleton4,
        NPCID.HeavySkeleton,
        NPCID.MisassembledSkeleton,
        NPCID.PantlessSkeleton,
        NPCID.SporeSkeleton,
    ];

    /// <summary>NPC Ids in this set cannot have their items stolen by the <see cref="Content.Items.Weapons.Melee.AncientCutlass.AncientCutlass"/>.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> CannotPickpocketItemsFrom { get; private set; } = [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail];

    /// <summary>
    /// NPC Ids in this set cannot damage the Occultist, or have their souls trapped into Soul Gems.
    /// Automatically populated with all NPC Ids which have the Underworld or Gore Nest as bestiary tags.
    /// </summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> CannotGrantSoulGems { get; private set; } = [];

    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> FromGlimmer { get; private set; } = [];
    /// <summary>Automatically populated with all NPC Ids which have the Eclipse as bestiary tags.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> FromEclipse { get; private set; } = [];
    /// <summary>Automatically populated with all NPC Ids which have the Blood Moon as bestiary tags.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> FromBloodMoon { get; private set; } = [];

    /// <summary>Entries in this set can completely override the Name Tag conditional check with their own value.</summary>
    [JsonProperty]
    public static Dictionary<IDEntry<NPCID>, bool> NameTagOverride { get; private set; } = new();

    /// <summary>Entries in this set are able to be stunned by the Stun Gun (<see cref="Content.Items.Weapons.Classless.StunGun.StunGunDebuff"/>).</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> StunnableByTypeId { get; private set; } = new();
    /// <summary>Enemies with an AI Type in this set are able to be stunned by the Stun Gun (<see cref="Content.Items.Weapons.Classless.StunGun.StunGunDebuff"/>).</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCAIStyleID>> StunnableByAI { get; private set; } = new();

    /// <summary>Used for Royal Gel's Crown of Blood combination.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> FriendablePreHardmodeSlime { get; private set; } = new();

    /// <summary>Used for Volatile Gelatin's Crown of Blood combination.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> FriendableHardmodeSlime { get; private set; } = new();

    /// <summary>Enemies in this set cannot become friendly through necromancy.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> Unfriendable { get; private set; } = new();

    /// <summary>NPCs in this set deal 'heat' contact damage. This damage can be resisted using the Frost Potion.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> DealsHeatDamage { get; private set; } = new();

    /// <summary>NPCs in this set cannot be given Elite affixes.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> PrefixBlacklist { get; private set; } = new();

    /// <summary>Entries in this set are able to be pushed by the Pumpinator.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCID>> PushableByTypeId { get; private set; } = new();
    /// <summary>Enemies with an AI Type in this set are able to be pushed by the Pumpinator.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<NPCAIStyleID>> PushableByAI { get; private set; } = new();

    #region Loading
    public override void SetupContent() {
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
            if (BestiaryTags.BloodMoon.ContainsNPCIdInner(i)) {
                FromBloodMoon.Add(i);
            }
            if (BestiaryTags.Eclipse.ContainsNPCIdInner(i)) {
                FromEclipse.Add(i);
            }
            if (BestiaryTags.Underworld.ContainsNPCIdInner(i)) {
                CannotGrantSoulGems.Add(i);
            }
        }
    }
    #endregion
}