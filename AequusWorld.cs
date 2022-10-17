using Aequus.Biomes.Glimmer;
using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Content.CrossMod.ModCalls;
using Aequus.Content.WorldGeneration;
using Aequus.Tiles;
using Aequus.Tiles.CrabCrevice;
using Aequus.Tiles.Misc;
using Aequus.UI;
using System;
using System.Collections.Generic;
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
        public static int TileCountsMultiplier;

        private static FieldInfo SceneMetrics__tileCounts;
        private static MethodInfo WorldGen_UpdateWorld_OvergroundTile;
        private static MethodInfo WorldGen_UpdateWorld_UndergroundTile;

        [SaveData("WhiteFlag")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool whiteFlag;

        [SaveData("Glimmer")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventCosmic;
        [SaveData("DemonSiege")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventDemon;
        [SaveData("GaleStreams")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventAtmosphere;

        [SaveData("SpaceSquid")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedSpaceSquid;
        [SaveData("RedSprite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedRedSprite;

        [SaveData("Crabson")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedCrabson;

        [SaveData("HyperStarite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedHyperStarite;
        [SaveData("UltraStarite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedUltraStarite;
        [SaveData("OmegaStarite")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedOmegaStarite;

        [SaveData("DustDevil")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedDustDevil;

        [SaveData("ShadowOrbs")]
        public static int shadowOrbsBrokenTotal;

        [SaveData("TinkererRerolls")]
        public static int tinkererRerolls;

        public static StructureLookups Structures { get; internal set; }

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void Load()
        {
            WorldGen_UpdateWorld_OvergroundTile = typeof(WorldGen).GetMethod("UpdateWorld_OvergroundTile", BindingFlags.NonPublic | BindingFlags.Static);
            WorldGen_UpdateWorld_UndergroundTile = typeof(WorldGen).GetMethod("UpdateWorld_UndergroundTile", BindingFlags.NonPublic | BindingFlags.Static);
            SceneMetrics__tileCounts = typeof(SceneMetrics).GetField("_tileCounts", AequusHelpers.LetMeIn);

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
            return !whiteFlag && orig();
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
            whiteFlag = false;
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
            Structures = new StructureLookups();
        }

        public override void OnWorldLoad()
        {
            Aequus.SkiesDarkness = 1f;
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Reset();
            }
            ResetWorldData();
        }

        public override void OnWorldUnload()
        {
            Aequus.SkiesDarkness = 1f;
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
            TileCountsMultiplier = 1;
        }

        public static void MarkAsDefeated(ref bool defeated, int npcID)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
        }

        public static void RandomUpdateTile(int x, int y, bool checkNPCSpawns = false, int wallDist = 3)
        {
            if (y < Main.worldSurface)
            {
                for (int i = 0; i < 100; i++)
                {
                    WorldGen_UpdateWorld_OvergroundTile.Invoke(null, new object[] { x, y, false, wallDist, });
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    WorldGen_UpdateWorld_UndergroundTile.Invoke(null, new object[] { x, y, false, wallDist, });
                }
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