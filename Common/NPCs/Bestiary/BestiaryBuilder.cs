using Terraria.GameContent.Bestiary;

namespace Aequus.Common.NPCs.Bestiary;

/// <summary>
/// Helper class for initializing bestiary entries.
/// </summary>
internal static class BestiaryBuilder {
    public readonly struct EntryEditor {
        private readonly ModNPC ModNPC;
        private readonly BestiaryDatabase Database;
        private readonly BestiaryEntry Entry;

        public EntryEditor(string languageKey, BestiaryDatabase database, BestiaryEntry bestiaryEntry, ModNPC modNPC) {
            ModNPC = modNPC;
            Database = database;
            Entry = bestiaryEntry;
            bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement(languageKey));
        }

        public EntryEditor AddMainSpawn(SpawnConditionBestiaryInfoElement info) {
            AddSpawn(info);
            return PreferBackground(info);
        }

        public EntryEditor AddSpawn(SpawnConditionBestiaryInfoElement info) {
            Entry.Info.Add(info);
            return this;
        }

        public EntryEditor PreferBackground(SpawnConditionBestiaryInfoElement info) {
            Entry.Info.Add(new BestiaryPortraitBackgroundProviderPreferenceInfoElement(info));
            return this;
        }

        public EntryEditor QuickUnlock() {
            return UseInfoProvider(new CommonEnemyUICollectionInfoProvider(ModNPC.NPC.GetBestiaryCreditId(), true));
        }

        public EntryEditor UseInfoProvider(CommonEnemyUICollectionInfoProvider infoProvider) {
            Entry.UIInfoProvider = infoProvider;
            return this;
        }
    }

    public static EntryEditor CreateEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        string key = modNPC.GetLocalizationKey("Bestiary");
        ExtendLanguage.RegisterKey(key);
        return new EntryEditor(key, database, bestiaryEntry, modNPC);
    }

    public static EntryEditor CreateEntry(this ModNPC modNPC, string key, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        ExtendLanguage.RegisterKey(key);
        return new EntryEditor(key, database, bestiaryEntry, modNPC);
    }

    public static void InsertEntry(ModNPC modNPC, int wantedBestiaryId) {
        int oldEntryID = ContentSamples.NpcBestiarySortingId[modNPC.Type];
        if (oldEntryID == wantedBestiaryId) {
            return;
        }
        if (oldEntryID < wantedBestiaryId) {
            for (int i = oldEntryID + 1; i <= wantedBestiaryId; i++) {
                for (int k = 1; k < NPCLoader.NPCCount; k++) {
                    if (ContentSamples.NpcBestiarySortingId.TryGetValue(k, out int sort) && sort == i) {
                        ContentSamples.NpcBestiarySortingId[k] = i - 1;
                    }
                }
            }
        }
        else {
            for (int i = oldEntryID - 1; i >= wantedBestiaryId; i--) {
                for (int k = 1; k < NPCLoader.NPCCount; k++) {
                    if (ContentSamples.NpcBestiarySortingId.TryGetValue(k, out int sort) && sort == i) {
                        ContentSamples.NpcBestiarySortingId[k] = i + 1;
                    }
                }
            }
        }
        ContentSamples.NpcBestiarySortingId[modNPC.Type] = wantedBestiaryId;
    }

    public const string GoldenCritterKey = "CommonBestiaryFlavor.GoldCritter";
    public const string GoldenBaitCritterKey = "CommonBestiaryFlavor.GoldBaitCritter";

    #region Spawn Condition Shortcuts (Since typing these out normally sucks)
    public static SpawnConditionBestiaryInfoElement DayTime => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime;
    public static SpawnConditionBestiaryInfoElement NightTime => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime;

    public static SpawnConditionBestiaryInfoElement SurfaceBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface;
    public static SpawnConditionBestiaryInfoElement CavernsBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns;
    public static SpawnConditionBestiaryInfoElement DesertBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert;
    public static SpawnConditionBestiaryInfoElement SnowBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow;
    public static SpawnConditionBestiaryInfoElement UndergroundSnowBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow;
    public static SpawnConditionBestiaryInfoElement SkyBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky;
    public static SpawnConditionBestiaryInfoElement OceanBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean;
    public static SpawnConditionBestiaryInfoElement Dungeon => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon;
    public static SpawnConditionBestiaryInfoElement Underground => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground;
    public static SpawnConditionBestiaryInfoElement Underworld => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld;
    public static SpawnConditionBestiaryInfoElement Corruption => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption;
    public static SpawnConditionBestiaryInfoElement UndergroundCorruption => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption;
    public static SpawnConditionBestiaryInfoElement CorruptionDesert => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptDesert;
    public static SpawnConditionBestiaryInfoElement CorruptionUndergroundDesert => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptUndergroundDesert;
    public static SpawnConditionBestiaryInfoElement CorruptionIce => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CorruptIce;
    public static SpawnConditionBestiaryInfoElement Crimson => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson;
    public static SpawnConditionBestiaryInfoElement UndergroundCrimson => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson;
    public static SpawnConditionBestiaryInfoElement CrimsonDesert => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonDesert;
    public static SpawnConditionBestiaryInfoElement CrimsonUndergroundDesert => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonUndergroundDesert;
    public static SpawnConditionBestiaryInfoElement CrimsonIce => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.CrimsonIce;
    public static SpawnConditionBestiaryInfoElement Hallow => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow;
    public static SpawnConditionBestiaryInfoElement UndergroundHallow => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundHallow;
    public static SpawnConditionBestiaryInfoElement HallowDesert => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.HallowDesert;
    public static SpawnConditionBestiaryInfoElement HallowUndergroundDesert => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.HallowUndergroundDesert;
    public static SpawnConditionBestiaryInfoElement HallowIce => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.HallowIce;

    public static SpawnConditionBestiaryInfoElement SolarPillar => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.SolarPillar;
    public static SpawnConditionBestiaryInfoElement VortexPillar => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar;
    public static SpawnConditionBestiaryInfoElement NebulaPillar => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.NebulaPillar;
    public static SpawnConditionBestiaryInfoElement StardustPillar => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.StardustPillar;
    public static SpawnConditionBestiaryInfoElement WindyDayEvent => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.WindyDay;
    public static SpawnConditionBestiaryInfoElement BloodMoon => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon;
    #endregion
}