using Aequus.Common;
using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
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
    public sealed class AequusWorld : ModSystem
    {
        [SaveData("SpaceSquid")]
        [NetBool]
        public static bool downedSpaceSquid;
        [SaveData("RedSprite")]
        [NetBool]
        public static bool downedRedSprite;
        [SaveData("GaleStreams")]
        [NetBool]
        public static bool downedEventGaleStreams;
        [SaveData("Crabson")]
        [NetBool]
        public static bool downedCrabson;
        [SaveData("OmegaStarite")]
        [NetBool]
        public static bool downedOmegaStarite;

        public static bool HardmodeTier => Main.hardMode || downedOmegaStarite;

        public override void OnWorldLoad()
        {
            Aequus.SkiesDarkness = 1f;
            downedSpaceSquid = false;
            downedRedSprite = false;
            downedEventGaleStreams = false;
            downedCrabson = false;
            downedOmegaStarite = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
        }

        public override void NetSend(BinaryWriter writer)
        {
            NetTypeAttribute.SendData(writer, this);
        }

        public override void NetReceive(BinaryReader reader)
        {
            NetTypeAttribute.ReadData(reader, this);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            foreach (var t in tasks)
            {
                Aequus.Instance.Logger.Debug(t.Name);
            }
            AddPass("Underworld", "Gore Nests", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.GoreNests");
                int goreNestCount = 0;
                for (int i = 0; i < (goreNestCount == 0 ? 1000000 : 10000); i++)
                {
                    int x = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                    int y = WorldGen.genRand.Next(GoreNestTile.MinY, GoreNestTile.MaxY);
                    if (GoreNestTile.TryGrowGoreNest(x, y))
                    {
                        goreNestCount++;
                        if (goreNestCount > 4)
                            break;
                    }
                }
            }, tasks);
            AddPass("Tile Cleanup", "Remove Lava From Nests", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.GoreNests");
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = GoreNestTile.MinY; j < GoreNestTile.MaxY; j++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<GoreNestTile>())
                        {
                            Aequus.Instance.Logger.Debug("Cleaning lava");
                            GoreNestTile.CleanLava(i, j);
                        }
                    }
                }
            }, tasks);
        }
        private void AddPass(string task, string myName, WorldGenLegacyMethod generation, List<GenPass> tasks)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals(task));
            if (i != -1)
                tasks.Insert(i + 1, new PassLegacy("Aequus: " + myName, generation));
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