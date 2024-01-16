using Terraria.GameContent.Bestiary;

namespace Aequus.Common.NPCs;

/// <summary>
/// Helper class for setting up bestiary entries.
/// </summary>
internal static class BestiaryBuilder {
    public struct Entry {
        private readonly ModNPC modNPC;
        private readonly BestiaryDatabase database;
        private readonly BestiaryEntry entry;

        public Entry(string flavorText, BestiaryDatabase database, BestiaryEntry bestiaryEntry, ModNPC modNPC) {
            this.modNPC = modNPC;
            this.database = database;
            entry = bestiaryEntry;
            bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement(flavorText));
        }

        public Entry AddMainSpawn(SpawnConditionBestiaryInfoElement info) {
            AddSpawn(info);
            return UseAsBackground(info);
        }

        public Entry AddSpawn(SpawnConditionBestiaryInfoElement info) {
            entry.Info.Add(info);
            return this;
        }

        public Entry UseAsBackground(SpawnConditionBestiaryInfoElement info) {
            entry.Info.Add(new BestiaryPortraitBackgroundProviderPreferenceInfoElement(info));
            return this;
        }

        public Entry QuickUnlock() {
            return UseInfoProvider(new CommonEnemyUICollectionInfoProvider(modNPC.NPC.GetBestiaryCreditId(), true));
        }

        public Entry UseInfoProvider(CommonEnemyUICollectionInfoProvider infoProvider) {
            entry.UIInfoProvider = infoProvider;
            return this;
        }
    }

    #region Spawn Condition Elements
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

    public static Entry CreateEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        return new Entry("Mods.Aequus.NPCs." + modNPC.Name + ".Bestiary", database, bestiaryEntry, modNPC);
    }
    public static Entry CreateGoldenCritterEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        return new Entry("CommonBestiaryFlavor.GoldCritter", database, bestiaryEntry, modNPC);
    }
    public static Entry CreateGoldenBaitCritterEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        return new Entry("CommonBestiaryFlavor.GoldBaitCritter", database, bestiaryEntry, modNPC);
    }

    public static Entry CreateGaleStreamsEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry, bool miniBoss = false) {
        var entry = modNPC.CreateEntry(database, bestiaryEntry);
        if (miniBoss) {
            entry.QuickUnlock();
        }
        return entry;
    }

    public static int[] SetBiome<T>(this ModNPC modNPC) where T : ModBiome {
        modNPC.SpawnModBiomes = new int[] { ModContent.GetInstance<T>().Type };
        return modNPC.SpawnModBiomes;
    }

    public static void MoveBestiaryEntry(ModNPC modNPC, int sortingID) {
        int oldEntryID = ContentSamples.NpcBestiarySortingId[modNPC.Type];
        if (oldEntryID == sortingID) {
            return;
        }
        if (oldEntryID < sortingID) {
            for (int i = oldEntryID + 1; i <= sortingID; i++) {
                for (int k = 1; k < NPCLoader.NPCCount; k++) {
                    if (ContentSamples.NpcBestiarySortingId.TryGetValue(k, out int sort) && sort == i) {
                        ContentSamples.NpcBestiarySortingId[k] = i - 1;
                    }
                }
            }
        }
        else {
            for (int i = oldEntryID - 1; i >= sortingID; i--) {
                for (int k = 1; k < NPCLoader.NPCCount; k++) {
                    if (ContentSamples.NpcBestiarySortingId.TryGetValue(k, out int sort) && sort == i) {
                        ContentSamples.NpcBestiarySortingId[k] = i + 1;
                    }
                }
            }
        }
        ContentSamples.NpcBestiarySortingId[modNPC.Type] = sortingID;
    }
}