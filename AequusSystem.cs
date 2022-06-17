﻿using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Content.Generation;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.Items.Weapons.Summon.Necro;
using Aequus.Projectiles;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Aequus
{
    public sealed class AequusSystem : ModSystem
    {
        public const int DungeonChestItemTypesMax = 4;

        public static int GoreNestCount;

        [SaveData("GaleStreams")]
        [SaveDataAttribute.IsListedBoolean]
        [NetBool]
        public static bool downedEventGaleStreams;

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

        public static Structures Structures { get; private set; }

        public static GoreNestGen GoreNests { get; private set; }

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void Load()
        {
            GoreNests = new GoreNestGen();
        }

        public override void OnWorldLoad()
        {
            Aequus.SkiesDarkness = 1f;
            shadowOrbsBrokenTotal = 0;
            downedSpaceSquid = false;
            downedRedSprite = false;
            downedEventGaleStreams = false;
            downedCrabson = false;
            downedOmegaStarite = false;
            downedDustDevil = false;
            Structures = new Structures();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
            Structures.Save(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
            Structures.Load(tag);
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
            AddPass("Beaches", "Crab Crevice", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.CrabCrevice");
                var crabCrevice = new CrabCreviceGen();
                crabCrevice.GenerateCrabCrevice(out var savePoint);
                Structures.Add("CrabCrevice", savePoint);
            }, tasks);

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

            for (int k = 0; k < Main.maxChests; k++)
            {
                Chest c = Main.chest[k];
                if (c != null)
                {
                    if (Main.tile[c.x, c.y].TileType == TileID.Containers)
                    {
                        int style = ChestTypes.GetChestStyle(c);
                        if (style == ChestTypes.Gold || style == ChestTypes.Frozen)
                        {
                            rockmanChests.Add(k);

                            if (Main.rand.NextBool(6))
                            {
                                AddGlowCore(c, placedItems);
                            }
                        }
                        if (style == ChestTypes.LockedGold)
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
                            if (choice == -1 && WorldGen.genRand.NextBool(DungeonChestItemTypesMax))
                            {
                                choice = DungeonChestItem(WorldGen.genRand.Next(DungeonChestItemTypesMax));
                            }

                            if (choice != -1)
                            {
                                c.Insert(choice, 1);
                                placedItems.Add(choice);
                            }
                        }
                        else if (style == ChestTypes.LockedShadow)
                        {
                            if (!placedItems.Contains(ModContent.ItemType<AshCandle>()) || Main.rand.NextBool(6))
                            {
                                c.Insert(ModContent.ItemType<AshCandle>(), 1);
                                placedItems.Add(ModContent.ItemType<AshCandle>());
                            }
                        }
                        else if (style == ChestTypes.Frozen)
                        {
                            if (!placedItems.Contains(ModContent.ItemType<CrystalDagger>()) || Main.rand.NextBool(6))
                            {
                                c.Insert(ModContent.ItemType<CrystalDagger>(), 1);
                                placedItems.Add(ModContent.ItemType<CrystalDagger>());
                            }
                        }
                        else if (style == ChestTypes.Skyware)
                        {
                            if (!placedItems.Contains(ModContent.ItemType<Slingshot>()) || Main.rand.NextBool())
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
                            if (Main.rand.NextBool())
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
                var c = Main.chest[rockmanChests[WorldGen.genRand.Next(rockmanChests.Count)]];
                Structures.Add("RockManChest", new Point(c.x, c.y));
                c.Insert(ModContent.ItemType<RockMan>(), WorldGen.genRand.Next(Chest.maxItems - 1));
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
            switch (Main.rand.Next(4))
            {
                default:
                    return ModContent.ItemType<Valari>();
                case 1:
                    return ModContent.ItemType<Revenant>();
                case 2:
                    return ModContent.ItemType<WretchedCandle>();
                case 3:
                    return ModContent.ItemType<PandorasBox>();
            }
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            GoreNestCount = tileCounts[ModContent.TileType<GoreNestTile>()];
        }

        public override void PostUpdatePlayers()
        {
            AequusProjectile.pWhoAmI = -1;
            AequusProjectile.pIdentity = -1;
            AequusProjectile.pNPC = -1;
            AequusHelpers.EndCaches();
        }

        public static void MarkAsDefeated(ref bool defeated, int npcID)
        {
            NPC.SetEventFlagCleared(ref defeated, -npcID);
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
        /// <item>DustDevil -- Warning, this flag doesn't exist yet, and will instead pull <see cref="downedEventGaleStreams"/></item>
        /// <item>GaleStreams -- <see cref="downedEventGaleStreams"/></item>
        /// </list>
        /// </summary>
        public class DownedCalls : IModCallable
        {
            private Dictionary<string, RefFunc<bool>> providers;

            /// <summary>
            /// Obtains or sets a world flag, a list of world flags are provided in <see cref="DownedCalls"/>' summary.
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
                    ["Crabson"] = () => ref downedCrabson,
                    ["OmegaStarite"] = () => ref downedOmegaStarite,
                    ["RedSprite"] = () => ref downedRedSprite,
                    ["SpaceSquid"] = () => ref downedSpaceSquid,
                    ["DustDevil"] = () => ref downedEventGaleStreams,
                    ["GaleStreams"] = () => ref downedEventGaleStreams,
                };
            }

            void ILoadable.Unload()
            {
            }
        }
    }
}