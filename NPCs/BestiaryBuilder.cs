using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    /// <summary>
    /// Helper class for setting up bestiary entries
    /// </summary>
    public static class BestiaryBuilder
    {
        public struct Entry
        {
            private readonly ModNPC modNPC;
            private readonly BestiaryDatabase database;
            private readonly BestiaryEntry entry;

            public Entry(string flavorText, BestiaryDatabase database, BestiaryEntry bestiaryEntry, ModNPC modNPC)
            {
                this.modNPC = modNPC;
                this.database = database;
                entry = bestiaryEntry;
                bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement(flavorText));
            }

            public Entry AddMainSpawn(SpawnConditionBestiaryInfoElement info)
            {
                AddSpawn(info);
                return UseAsBackground(info);
            }

            public Entry AddSpawn(SpawnConditionBestiaryInfoElement info)
            {
                entry.Info.Add(info);
                return this;
            }

            public Entry UseAsBackground(SpawnConditionBestiaryInfoElement info)
            {
                entry.Info.Add(new BestiaryPortraitBackgroundProviderPreferenceInfoElement(info));
                return this;
            }

            public Entry QuickUnlock()
            {
                return UseInfoProvider(new CommonEnemyUICollectionInfoProvider(modNPC.NPC.GetBestiaryCreditId(), true));
            }

            public Entry UseInfoProvider(CommonEnemyUICollectionInfoProvider infoProvider)
            {
                entry.UIInfoProvider = infoProvider;
                return this;
            }
        }

        public static SpawnConditionBestiaryInfoElement DayTime => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime;
        public static SpawnConditionBestiaryInfoElement NightTime => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime;

        public static SpawnConditionBestiaryInfoElement SurfaceBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface;
        public static SpawnConditionBestiaryInfoElement CavernsBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns;
        public static SpawnConditionBestiaryInfoElement DesertBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert;
        public static SpawnConditionBestiaryInfoElement SnowBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow;
        public static SpawnConditionBestiaryInfoElement SkyBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky;
        public static SpawnConditionBestiaryInfoElement OceanBiome => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean;
        public static SpawnConditionBestiaryInfoElement Dungeon => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon;
        public static SpawnConditionBestiaryInfoElement Underground => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground;
        public static SpawnConditionBestiaryInfoElement Underworld => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld;

        public static SpawnConditionBestiaryInfoElement WindyDayEvent => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.WindyDay;
        public static SpawnConditionBestiaryInfoElement BloodMoon => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon;

        public static Entry CreateEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            return new Entry("Mods.Aequus.Bestiary." + modNPC.Name, database, bestiaryEntry, modNPC);
        }

        public static Entry CreateGaleStreamsEntry(this ModNPC modNPC, BestiaryDatabase database, BestiaryEntry bestiaryEntry, bool miniBoss = false)
        {
            var entry = CreateEntry(modNPC, database, bestiaryEntry);
            if (miniBoss)
            {
                entry.QuickUnlock();
            }
            return entry;
        }

        public static int[] SetBiome<T>(this ModNPC modNPC) where T : ModBiome
        {
            modNPC.SpawnModBiomes = new int[] { ModContent.GetInstance<T>().Type };
            return modNPC.SpawnModBiomes;
        }
    }
}