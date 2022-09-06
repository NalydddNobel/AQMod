using Aequus.Biomes.Glimmer;
using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Content.Generation;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Projectiles;
using Aequus.Tiles;
using Aequus.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Aequus
{
    public class AequusWorld : ModSystem
    {
        public const int DungeonChestItemTypesMax = 4;
        public static int TileCountsMultiplier;

        private static FieldInfo SceneMetrics__tileCounts;

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

        public static bool BloodMoonDisabled;
        public static bool GlimmerDisabled;
        public static bool EclipseDisabled;

        public static Structures Structures { get; private set; }

        public static GoreNestGen GoreNests { get; private set; }

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void Load()
        {
            GoreNests = new GoreNestGen();
            On.Terraria.WorldGen.CanCutTile += WorldGen_CanCutTile;
            On.Terraria.Main.ShouldNormalEventsBeAbleToStart += Main_ShouldNormalEventsBeAbleToStart;
            On.Terraria.Main.UpdateTime_StartNight += Main_UpdateTime_StartNight;
            On.Terraria.SceneMetrics.ExportTileCountsToMain += SceneMetrics_ExportTileCountsToMain;
            SceneMetrics__tileCounts = typeof(SceneMetrics).GetField("_tileCounts", AequusHelpers.LetMeIn);
        }

        private static bool WorldGen_CanCutTile(On.Terraria.WorldGen.orig_CanCutTile orig, int x, int y, Terraria.Enums.TileCuttingContext context)
        {
            if (orig(x, y, context))
            {
                return !Main.tile[x, y].Get<AequusTileData>().Uncuttable;
            }
            return false;
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
            Structures = new Structures();
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
        }

        public override void NetReceive(BinaryReader reader)
        {
            NetTypeAttribute.ReadData(reader, this);
            shadowOrbsBrokenTotal = reader.ReadInt32();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            Structures = new Structures();
            //AddPass("Beaches", "Crab Crevice", (progress, configuration) =>
            //{
            //    progress.Message = AequusText.GetText("WorldGeneration.CrabCrevice");
            //    var crabCrevice = new CrabCreviceGen();
            //    crabCrevice.GenerateCrabCrevice(out var savePoint);
            //    Structures.Add("CrabCrevice", savePoint);
            //}, tasks);

            AddPass("Tile Cleanup", "Gore Nest Cleanup", (progress, configuration) =>
            {
                GoreNests.Cleanup();
            }, tasks);
            AddPass("Underworld", "Gore Nests", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.GoreNests");
                GoreNests.Generate();
            }, tasks);
        }
        private void AddPass(string task, string myName, WorldGenLegacyMethod generation, List<GenPass> tasks)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals(task));
            if (i != -1)
                tasks.Insert(i + 1, new PassLegacy("Aequus: " + myName, generation));
        }

        public override void PostWorldGen()
        {
            var rockmanChests = new List<int>();

            var placedItems = new HashSet<int>();
            var r = WorldGen.genRand;
            for (int k = 0; k < Main.maxChests; k++)
            {
                Chest c = Main.chest[k];
                if (c != null)
                {
                    if (Main.tile[c.x, c.y].TileType == TileID.Containers)
                    {
                        int style = ChestTypes.GetChestStyle(c);
                        if (style == ChestTypes.Gold || style == ChestTypes.Marble || style == ChestTypes.Granite || style == ChestTypes.Mushroom)
                        {
                            rockmanChests.Add(k);

                            if (r.NextBool(5))
                            {
                                AddGlowCore(c, placedItems);
                            }

                            switch (r.Next(5))
                            {
                                case 0:
                                    c.Insert(ModContent.ItemType<BoneRing>(), 1);
                                    break;

                                case 1:
                                    c.Insert(ModContent.ItemType<BattleAxe>(), 1);
                                    break;

                                case 2:
                                    c.Insert(ModContent.ItemType<Bellows>(), 1);
                                    break;
                            }
                        }
                        else if (style == ChestTypes.LockedGold)
                        {
                            int choice = -1;
                            for (int i = 0; i < DungeonChestItemTypesMax; i++)
                            {
                                int item = DungeonChestItem(i);
                                if (!placedItems.Contains(item))
                                {
                                    choice = item;
                                }
                            }
                            if (choice == -1 && r.NextBool(DungeonChestItemTypesMax))
                            {
                                choice = DungeonChestItem(r.Next(DungeonChestItemTypesMax));
                            }

                            if (choice != -1)
                            {
                                c.Insert(choice, 1);
                                placedItems.Add(choice);
                            }
                        }
                        else if (style == ChestTypes.Frozen)
                        {
                            rockmanChests.Add(k);

                            if (r.NextBool(6))
                            {
                                AddGlowCore(c, placedItems);
                            }
                            if (!placedItems.Contains(ModContent.ItemType<CrystalDagger>()) || r.NextBool(6))
                            {
                                c.Insert(ModContent.ItemType<CrystalDagger>(), 1);
                                placedItems.Add(ModContent.ItemType<CrystalDagger>());
                            }
                        }
                        else if (style == ChestTypes.Skyware)
                        {
                            if (!placedItems.Contains(ModContent.ItemType<Slingshot>()) || r.NextBool())
                            {
                                c.Insert(ModContent.ItemType<Slingshot>(), 1);
                                placedItems.Add(ModContent.ItemType<Slingshot>());
                            }
                        }
                    }
                    else if (Main.tile[c.x, c.y].TileType == TileID.Containers2)
                    {
                        int style = ChestTypes.GetChestStyle(c);
                        if (style == ChestTypes.deadMans)
                        {
                            rockmanChests.Add(k);
                            if (r.NextBool())
                            {
                                AddGlowCore(c, placedItems);
                            }
                        }
                        else if (style == ChestTypes.sandstone)
                        {
                            rockmanChests.Add(k);
                        }
                    }
                }
            }

            if (rockmanChests.Count > 0)
            {
                var c = Main.chest[rockmanChests[r.Next(rockmanChests.Count)]];
                Structures.Add("RockManChest", new Point(c.x, c.y));
                c.Insert(ModContent.ItemType<RockMan>(), r.Next(Chest.maxItems - 1));
            }
        }
        public static bool AddGlowCore(Chest c, HashSet<int> placedItems = null)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!c.item[i].IsAir && (c.item[i].type == ItemID.Torch || c.item[i].type == ItemID.Glowstick))
                {
                    c.item[i].SetDefaults(ModContent.ItemType<GlowCore>());
                    placedItems?.Add(ModContent.ItemType<GlowCore>());
                    return true;
                }
            }
            return false;
        }
        public static int DungeonChestItem(int type)
        {
            switch (WorldGen.genRand.Next(4))
            {
                default:
                    return ModContent.ItemType<Valari>();
                case 1:
                    return ModContent.ItemType<Revenant>();
                case 2:
                    return ModContent.ItemType<DungeonCandle>();
                case 3:
                    return ModContent.ItemType<PandorasBox>();
            }
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            GoreNestTile.BiomeCount = tileCounts[ModContent.TileType<GoreNestTile>()];
            TileCountsMultiplier = 1;
        }

        public override void PreUpdateEntities()
        {
            ResetCaches();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Enabled = false;
                AdvancedRulerInterface.Instance.Holding = false;
                OmniPaintUI.Instance.Enabled = false;
            }
            BloodMoonDisabled = false;
            GlimmerDisabled = false;
            EclipseDisabled = false;
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = false;
        }

        public override void PostUpdatePlayers()
        {
            ResetCaches();
        }

        public void ResetCaches()
        {
            AequusProjectile.pWhoAmI = -1;
            AequusProjectile.pIdentity = -1;
            AequusProjectile.pNPC = -1;
            AequusPlayer.PlayerContext = -1;
            AequusHelpers.EndCaches();
        }

        public override void PostUpdateTime()
        {
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
        }

        public static void MarkAsDefeated(ref bool defeated, int npcID)
        {
            NPC.SetEventFlagCleared(ref defeated, -1);
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

        /// <summary>
        /// World Flags:
        /// <list type="table">
        /// <item>Crabson -- <see cref="downedCrabson"/></item>
        /// <item>OmegaStarite -- <see cref="downedOmegaStarite"/></item>
        /// <item>RedSprite -- <see cref="downedRedSprite"/></item>
        /// <item>SpaceSquid -- <see cref="downedSpaceSquid"/></item>
        /// <item>DustDevil -- Warning, this flag doesn't exist yet, and will instead pull <see cref="downedEventAtmosphere"/></item>
        /// <item>GaleStreams -- <see cref="downedEventAtmosphere"/></item>
        /// </list>
        /// </summary>
        public class ModCalls : IModCallable
        {
            private Dictionary<string, RefFunc<bool>> providers;

            /// <summary>
            /// Obtains or sets a world flag, a list of world flags are provided in <see cref="ModCalls"/>' summary.
            /// <para>Obtaining a flag:</para>
            /// <code>aequus.Call("Downed", "Crabson" -- {Or any of the flag names provided})</code>
            /// <para>Setting a flag:</para>
            /// <code>aequus.Call("Downed", "Set", "Crabson" -- {Or any of the flag names provided}, {true/false})</code>
            /// </summary>
            /// <param name="aequus"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object HandleModCall(Aequus aequus, object[] args)
            {
                if (args.Length > 1 && args[1] is string key)
                {
                    if (key == "Set")
                    {
                        if (args.Length > 2 && args[2] is string key2)
                        {
                            if (args.Length > 3 && args[3] is bool flag)
                            {
                                if (providers.TryGetValue(key2, out var value))
                                {
                                    if (Aequus.LogMore)
                                        Aequus.Instance.Logger.Info("Set world flag '" + key2 + "'. Original Value: " + value() + ", New Value: " + flag);
                                    return value() = flag;
                                }
                            }
                            else
                            {
                                Aequus.Instance.Logger.Error("Invalid arguments. Parameter 3 for setting a world flag should be a Boolean.");
                            }
                        }
                        else
                        {
                            Aequus.Instance.Logger.Error("Invalid arguments. Parameter 2 for setting a world flag should be a String.");
                        }
                    }
                    else
                    {
                        if (providers.TryGetValue(key, out var value))
                        {
                            if (Aequus.LogMore)
                                Aequus.Instance.Logger.Info("Obtained world flag '" + key + "'. Value: " + value());
                            return value();
                        }
                    }
                    Aequus.Instance.Logger.Error("There is no world flag named '" + key + "'");
                }
                return IModCallable.Failure;
            }

            void ILoadable.Load(Mod mod)
            {
                providers = new Dictionary<string, RefFunc<bool>>()
                {
                    ["WhiteFlag"] = () => ref whiteFlag,
                    ["HyperStarite"] = () => ref downedHyperStarite,
                    ["UltraStarite"] = () => ref downedUltraStarite,
                    ["Glimmer"] = () => ref downedEventCosmic,
                    ["DemonSiege"] = () => ref downedEventDemon,
                    ["Crabson"] = () => ref downedCrabson,
                    ["OmegaStarite"] = () => ref downedOmegaStarite,
                    ["RedSprite"] = () => ref downedRedSprite,
                    ["SpaceSquid"] = () => ref downedSpaceSquid,
                    ["DustDevil"] = () => ref downedDustDevil,
                    ["GaleStreams"] = () => ref downedEventAtmosphere,
                };
            }

            void ILoadable.Unload()
            {
            }
        }
    }
}