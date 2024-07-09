using System;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace AequusRemake.Core.Entities.Bestiary;

/// <summary>Helper class for initializing bestiary entries.</summary>
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
        ALanguage.RegisterKey(key);
        return new EntryEditor(key, database, bestiaryEntry, modNPC);
    }

    public static EntryEditor CreateEntry(this ModNPC modNPC, string key, BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        ALanguage.RegisterKey(key);
        return new EntryEditor(key, database, bestiaryEntry, modNPC);
    }

    public static void ReSort(ModNPC modNPC, int npcIdToSortAfter, int sortOffset = 0) {
        try {
            Dictionary<int, int> sorting = ContentSamples.NpcBestiarySortingId;
            if (!sorting.TryGetValue(modNPC.Type, out int oldEntryID)) {
                throw new Exception("ModNPC does not have a Bestiary Entry sorting id.");
            }
            if (!sorting.TryGetValue(npcIdToSortAfter, out int sortingID)) {
                throw new Exception("NPC to sort after does not have a Bestiary Entry sorting id.");
            }
            sortingID += sortOffset;

            if (oldEntryID == sortingID) {
                return;
            }

            if (oldEntryID < sortingID) {
                for (int i = oldEntryID + 1; i <= sortingID; i++) {
                    for (int k = 1; k < NPCLoader.NPCCount; k++) {
                        if (sorting.TryGetValue(k, out int sort) && sort == i) {
                            sorting[k] = i - 1;
                        }
                    }
                }
            }
            else {
                for (int i = oldEntryID - 1; i >= sortingID; i--) {
                    for (int k = 1; k < NPCLoader.NPCCount; k++) {
                        if (sorting.TryGetValue(k, out int sort) && sort == i) {
                            sorting[k] = i + 1;
                        }
                    }
                }
            }
            sorting[modNPC.Type] = sortingID;
        }
        catch (Exception ex) {
            Log.Error($"Failed to move entry for {modNPC.Type} ({modNPC.Name}) to {npcIdToSortAfter} ({(NPCID.Search.TryGetName(npcIdToSortAfter, out string name) ? name : "Unknown")}).\n{ex}");
        }
    }

    #region Spawn Condition Shortcuts (Since typing these out normally sucks)
    public static SpawnConditionBestiaryInfoElement DayTime => BestiaryTimeTag.DayTime;
    public static SpawnConditionBestiaryInfoElement NightTime => BestiaryTimeTag.NightTime;

    public static SpawnConditionBestiaryInfoElement SurfaceBiome => BestiaryBiomeTag.Surface;
    public static SpawnConditionBestiaryInfoElement CavernsBiome => BestiaryBiomeTag.Caverns;
    public static SpawnConditionBestiaryInfoElement DesertBiome => BestiaryBiomeTag.Desert;
    public static SpawnConditionBestiaryInfoElement SnowBiome => BestiaryBiomeTag.Snow;
    public static SpawnConditionBestiaryInfoElement UndergroundSnowBiome => BestiaryBiomeTag.UndergroundSnow;
    public static SpawnConditionBestiaryInfoElement SkyBiome => BestiaryBiomeTag.Sky;
    public static SpawnConditionBestiaryInfoElement OceanBiome => BestiaryBiomeTag.Ocean;
    public static SpawnConditionBestiaryInfoElement Dungeon => BestiaryBiomeTag.TheDungeon;
    public static SpawnConditionBestiaryInfoElement Underground => BestiaryBiomeTag.Underground;
    public static SpawnConditionBestiaryInfoElement Underworld => BestiaryBiomeTag.TheUnderworld;
    public static SpawnConditionBestiaryInfoElement Corruption => BestiaryBiomeTag.TheCorruption;
    public static SpawnConditionBestiaryInfoElement UndergroundCorruption => BestiaryBiomeTag.UndergroundCorruption;
    public static SpawnConditionBestiaryInfoElement CorruptionDesert => BestiaryBiomeTag.CorruptDesert;
    public static SpawnConditionBestiaryInfoElement CorruptionUndergroundDesert => BestiaryBiomeTag.CorruptUndergroundDesert;
    public static SpawnConditionBestiaryInfoElement CorruptionIce => BestiaryBiomeTag.CorruptIce;
    public static SpawnConditionBestiaryInfoElement Crimson => BestiaryBiomeTag.TheCrimson;
    public static SpawnConditionBestiaryInfoElement UndergroundCrimson => BestiaryBiomeTag.UndergroundCrimson;
    public static SpawnConditionBestiaryInfoElement CrimsonDesert => BestiaryBiomeTag.CrimsonDesert;
    public static SpawnConditionBestiaryInfoElement CrimsonUndergroundDesert => BestiaryBiomeTag.CrimsonUndergroundDesert;
    public static SpawnConditionBestiaryInfoElement CrimsonIce => BestiaryBiomeTag.CrimsonIce;
    public static SpawnConditionBestiaryInfoElement Hallow => BestiaryBiomeTag.TheHallow;
    public static SpawnConditionBestiaryInfoElement UndergroundHallow => BestiaryBiomeTag.UndergroundHallow;
    public static SpawnConditionBestiaryInfoElement HallowDesert => BestiaryBiomeTag.HallowDesert;
    public static SpawnConditionBestiaryInfoElement HallowUndergroundDesert => BestiaryBiomeTag.HallowUndergroundDesert;
    public static SpawnConditionBestiaryInfoElement HallowIce => BestiaryBiomeTag.HallowIce;

    public static SpawnConditionBestiaryInfoElement SolarPillar => BestiaryBiomeTag.SolarPillar;
    public static SpawnConditionBestiaryInfoElement VortexPillar => BestiaryBiomeTag.VortexPillar;
    public static SpawnConditionBestiaryInfoElement NebulaPillar => BestiaryBiomeTag.NebulaPillar;
    public static SpawnConditionBestiaryInfoElement StardustPillar => BestiaryBiomeTag.StardustPillar;
    public static SpawnConditionBestiaryInfoElement WindyDayEvent => BestiaryEventTag.WindyDay;
    public static SpawnConditionBestiaryInfoElement BloodMoon => BestiaryEventTag.BloodMoon;
    #endregion
}