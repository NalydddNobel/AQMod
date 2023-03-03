using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Common.Utilities;
using Aequus.Content.Boss.Crabson;
using Aequus.Content.Boss.DustDevil;
using Aequus.Content.Boss.OmegaStarite;
using Aequus.Content.Boss.RedSpriteMiniboss;
using Aequus.Content.Boss.SpaceSquidMiniboss;
using Aequus.Content.Boss.UltraStariteMiniboss;
using Aequus.Content.Town.CarpenterNPC;
using Aequus.Content.WorldGeneration;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Misc.Carpentry.Rewards;
using Aequus.Networking;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Aequus.Tiles;
using Aequus.Tiles.CrabCrevice;
using Aequus.Tiles.CraftingStation;
using Aequus.Tiles.Furniture;
using Aequus.UI;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus
{
    public class AequusWorld : ModSystem
    {
        public const int SmallWidth = 4200;
        public const int SmallHeight = 1200;
        public const int MedWidth = 6400;
        public const int MedHeight = 1800;
        public const int LargeWidth = 8400;
        public const int LargeHeight = 2400;

        /// <summary>
        /// Used by the <see cref="Carpenter"/> for detecting biomes easier, since Shutterstocker shots may not include enough tiles to register a biome.
        /// </summary>
        public static int TileCountsMultiplier;

        private static FieldInfo SceneMetrics__tileCounts;
        private static MethodInfo WorldGen_UpdateWorld_OvergroundTile;
        private static MethodInfo WorldGen_UpdateWorld_UndergroundTile;

        /// <summary>
        /// Whether or not the <see cref="WhiteFlag"/> item was used.
        /// </summary>
        [SaveData("WhiteFlag")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool usedWhiteFlag;

        /// <summary>
        /// Whether or not the Glimmer event was completed.
        /// </summary>
        [SaveData("Glimmer")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventCosmic;
        /// <summary>
        /// Whether or not the Demon Siege event was completed.
        /// </summary>
        [SaveData("DemonSiege")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventDemon;
        /// <summary>
        /// Whether or not the Gale Streams event was completed.
        /// </summary>
        [SaveData("GaleStreams")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventAtmosphere;

        /// <summary>
        /// Whether or not the <see cref="SpaceSquid"/> miniboss was defeated.
        /// </summary>
        [SaveData("SpaceSquid")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedSpaceSquid;
        /// <summary>
        /// Whether or not the <see cref="RedSprite"/> miniboss was defeated.
        /// </summary>
        [SaveData("RedSprite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedRedSprite;

        /// <summary>
        /// Whether or not the <see cref="Crabson"/> boss was defeated.
        /// </summary>
        [SaveData("Crabson")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedCrabson;

        /// <summary>
        /// Unused.
        /// </summary>
        [SaveData("Upriser")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedUpriser;

        /// <summary>
        /// Whether or not the <see cref="HyperStarite"/> enemy was defeated.
        /// </summary>
        [SaveData("HyperStarite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedHyperStarite;
        /// <summary>
        /// Whether or not the <see cref="UltraStarite"/> enemy was defeated.
        /// </summary>
        [SaveData("UltraStarite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedUltraStarite;
        /// <summary>
        /// Whether or not the <see cref="OmegaStarite"/> boss was defeated.
        /// </summary>
        [SaveData("OmegaStarite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedOmegaStarite;

        /// <summary>
        /// Whether or not the <see cref="DustDevil"/> boss was defeated.
        /// </summary>
        [SaveData("DustDevil")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedDustDevil;

        /// <summary>
        /// A tracker of the total amount of Shadow Orbs broken in the world, this is currently unused.
        /// </summary>
        [SaveData("ShadowOrbs")]
        public static int shadowOrbsBrokenTotal;

        /// <summary>
        /// How many times the tinkerer should re-roll items for a potential higher value prefix. 
        /// This value flips between 0 and 3 depending on if the <see cref="TinkerersGuidebook"/> has been consumed.
        /// </summary>
        [SaveData("TinkererRerolls")]
        public static int tinkererRerolls;

        [SaveData("XmasWorld")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool xmasWorld;

        [SaveData("XmasHats")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool xmasHats;

        [SaveData("ChestCobalt")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool chestCobaltTier;
        [SaveData("ChestMythril")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool chestMythrilTier;
        [SaveData("ChestAdamantite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool chestAdamantiteTier;
        [SaveData("HardmodeChests")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool hardmodeChests;
        [SaveData("EoCOres")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool eyeOfCthulhuOres;

        public static StructureLookups Structures { get; internal set; }

        public override void Load()
        {
            WorldGen_UpdateWorld_OvergroundTile = typeof(WorldGen).GetMethod("UpdateWorld_OvergroundTile", BindingFlags.NonPublic | BindingFlags.Static);
            WorldGen_UpdateWorld_UndergroundTile = typeof(WorldGen).GetMethod("UpdateWorld_UndergroundTile", BindingFlags.NonPublic | BindingFlags.Static);
            SceneMetrics__tileCounts = typeof(SceneMetrics).GetField("_tileCounts", Helper.LetMeIn);

            LoadHooks();
        }

        #region Hooks
        private static void LoadHooks()
        {
            On.Terraria.Main.ShouldNormalEventsBeAbleToStart += Main_ShouldNormalEventsBeAbleToStart;
            On.Terraria.Main.UpdateTime_StartNight += Main_UpdateTime_StartNight;
            On.Terraria.SceneMetrics.ExportTileCountsToMain += SceneMetrics_ExportTileCountsToMain;
        }

        private static bool Main_ShouldNormalEventsBeAbleToStart(On.Terraria.Main.orig_ShouldNormalEventsBeAbleToStart orig)
        {
            return !usedWhiteFlag && orig();
        }

        private static void Main_UpdateTime_StartNight(On.Terraria.Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            orig(ref stopEvents);
            if (!stopEvents)
            {
                GlimmerSystem.OnTransitionToNight();
            }
        }
        private static void SceneMetrics_ExportTileCountsToMain(On.Terraria.SceneMetrics.orig_ExportTileCountsToMain orig, SceneMetrics self)
        {
            if (TileCountsMultiplier > 1)
            {
                var arr = (int[])SceneMetrics__tileCounts.GetValue(self);
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] *= 5;
                }
            }
            orig(self);
        }
        #endregion

        public override void Unload()
        {
            WorldGen_UpdateWorld_OvergroundTile = null;
            WorldGen_UpdateWorld_UndergroundTile = null;
            SceneMetrics__tileCounts = null;
        }

        public void ResetWorldData()
        {
            eyeOfCthulhuOres = false;
            downedHyperStarite = false;
            downedUltraStarite = false;
            hardmodeChests = false;
            chestCobaltTier = false;
            chestMythrilTier = false;
            chestAdamantiteTier = false;
            xmasWorld = false;
            xmasHats = false;
            usedWhiteFlag = false;
            TileCountsMultiplier = 0;
            shadowOrbsBrokenTotal = 0;
            downedEventCosmic = false;
            downedEventDemon = false;
            downedEventAtmosphere = false;
            downedSpaceSquid = false;
            downedRedSprite = false;
            downedCrabson = false;
            downedOmegaStarite = false;
            downedDustDevil = false;
            downedUpriser = false;
            Structures = new StructureLookups();
        }

        public override void OnWorldLoad()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Reset();
            }
            ResetWorldData();
        }

        public override void OnWorldUnload()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Reset();
            }
            ResetWorldData();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
            Structures.Save(tag);
            var tileData = AequusTileData.Save();
            if (tileData.Length != 0)
            {
                tag["AequusTileData"] = tileData;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
            Structures.Load(tag);
            if (tag.TryGet<byte[]>("AequusTileData", out var tileData))
            {
                AequusTileData.Load(tileData);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            NetTypeAttribute.SendData(writer, this);
            writer.Write(shadowOrbsBrokenTotal);
            writer.Write(tinkererRerolls);
        }

        public override void NetReceive(BinaryReader reader)
        {
            NetTypeAttribute.ReadData(reader, this);
            shadowOrbsBrokenTotal = reader.ReadInt32();
            tinkererRerolls = reader.ReadInt32();
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            GoreNestTile.BiomeCount = tileCounts[ModContent.TileType<GoreNestTile>()];
            SedimentaryRockTile.BiomeCount = tileCounts[ModContent.TileType<SedimentaryRockTile>()];
            AshTombstones.numAshTombstones = tileCounts[ModContent.TileType<AshTombstones>()] / 4;
            foreach (var mossBiome in GlowingMossBiome.MossBiomes)
            {
                mossBiome.tileCount = tileCounts[mossBiome.MossTileID] + tileCounts[mossBiome.MossBrickTileID];
            }
            TileCountsMultiplier = 1;
        }

        public static void MarkAsDefeated(ref bool defeated, int npcID)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
        }

        public static void RandomUpdateTile_Surface(int x, int y, bool checkNPCSpawns = false, int wallDist = 3)
        {
            WorldGen_UpdateWorld_OvergroundTile.Invoke(null, new object[] { x, y, checkNPCSpawns, wallDist, });
        }

        public static void RandomUpdateTile_Underground(int x, int y, bool checkNPCSpawns = false, int wallDist = 3)
        {
            WorldGen_UpdateWorld_UndergroundTile.Invoke(null, new object[] { x, y, checkNPCSpawns, wallDist, });
        }

        public static void RandomUpdateTile(int x, int y, bool checkNPCSpawns = false, int wallDist = 3)
        {
            if (y < Main.worldSurface)
            {
                RandomUpdateTile_Surface(x, y, checkNPCSpawns, wallDist);
            }
            else
            {
                RandomUpdateTile_Underground(x, y, checkNPCSpawns, wallDist);
            }
        }

        public static bool Outer(int x, int iths)
        {
            int ithX = Main.maxTilesX / iths;
            if (x <= ithX || x >= Main.maxTilesX - ithX)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A bitsbyte instance where 0 is copper, 1 is iron, 2 is silver, 3 is gold. When they are false, they are the alternate world ore.</returns>
        public static BitsByte OreTiers()
        {
            if (Main.drunkWorld)
            {
                return byte.MaxValue;
            }
            return new BitsByte(
                WorldGen.SavedOreTiers.Copper == TileID.Copper,
                WorldGen.SavedOreTiers.Iron == TileID.Iron,
                WorldGen.SavedOreTiers.Silver == TileID.Silver,
                WorldGen.SavedOreTiers.Gold == TileID.Gold);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the world has cobalt, false if the world has palladium, null if the world isn't in hardmode or has neither</returns>
        public static bool? HasCobalt()
        {
            if (!Main.hardMode || (WorldGen.SavedOreTiers.Cobalt != TileID.Cobalt && WorldGen.SavedOreTiers.Cobalt != TileID.Palladium))
            {
                return null;
            }
            return WorldGen.SavedOreTiers.Cobalt == TileID.Cobalt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the world has mythril, false if the world has orichalcum, null if the world isn't in hardmode or has neither</returns>
        public static bool? HasMythril()
        {
            if (!Main.hardMode || (WorldGen.SavedOreTiers.Mythril != TileID.Mythril && WorldGen.SavedOreTiers.Mythril != TileID.Orichalcum))
            {
                return null;
            }
            return WorldGen.SavedOreTiers.Mythril == TileID.Mythril;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the world has adamantite, false if the world has titanium, null if the world isn't in hardmode or has neither</returns>
        public static bool? HasAdamantite()
        {
            if (!Main.hardMode || (WorldGen.SavedOreTiers.Adamantite != TileID.Adamantite && WorldGen.SavedOreTiers.Adamantite != TileID.Titanium))
            {
                return null;
            }
            return WorldGen.SavedOreTiers.Adamantite == TileID.Adamantite;
        }
    }
}