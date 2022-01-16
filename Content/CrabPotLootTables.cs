using AQMod.Common;
using AQMod.Content.Fishing;
using AQMod.Items.Fish.BloodMoon;
using AQMod.Items.Fish.Corruption;
using AQMod.Items.Fish.Crimson;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public sealed class CrabPotLootTables : IAutoloadType
    {
        public struct CrabPotLoot
        {
            public readonly int CatchItem;
            /// <summary>
            /// true is day, false is night
            /// </summary>
            public readonly bool? Time;
            public readonly bool AfterBoss2;
            public readonly bool HardmodeOnly;
            public readonly byte[] validWorldLayers;
            private Func<bool> _customCaptureCheck;

            public CrabPotLoot(int item, byte[] validWorldLayers = null, bool? time = null, bool afterBoss2 = false, bool hardmodeOnly = false, Func<bool> customCaptureCheck = null)
            {
                CatchItem = item;
                this.validWorldLayers = validWorldLayers;
                Time = time;
                AfterBoss2 = afterBoss2;
                HardmodeOnly = hardmodeOnly;
                _customCaptureCheck = customCaptureCheck;
            }

            public bool CanCatch(byte worldLayer)
            {
                if (validWorldLayers != null)
                {
                    for (int i = 0; i < validWorldLayers.Length; i++)
                    {
                        if (worldLayer != validWorldLayers[i])
                            return false;
                    }
                }
                return Time != null && Main.dayTime != Time.Value
                    ? false
                    : AfterBoss2 && !NPC.downedBoss2
                    ? false
                    : HardmodeOnly && !Main.hardMode ? false : _customCaptureCheck != null ? _customCaptureCheck() : true;
            }
        }

        public static bool CanCatchNormalFish { get; private set; }
        private static void FishCheck_NormalFish()
        {
            CanCatchNormalFish = !Crimson && !Corruption && !Desert && !Ocean && !Jungle && !Snow;
        }

        public static bool Ocean;
        public static bool BloodMoon;
        public static bool Hallow;
        public static bool Crimson;
        public static bool Corruption;
        public static bool Jungle;
        public static bool Desert;
        public static bool Snow;
        public static byte WorldLayer;

        public static List<CrabPotLoot> NormalLoot { get; private set; }
        public static List<CrabPotLoot> BloodMoonLoot { get; private set; }
        public static List<CrabPotLoot> CrimsonLoot { get; private set; }
        public static List<CrabPotLoot> CorruptionLoot { get; private set; }
        public static List<CrabPotLoot> JungleLoot { get; private set; }
        public static List<CrabPotLoot> HallowLoot { get; private set; }
        public static List<CrabPotLoot> SnowLoot { get; private set; }
        public static List<CrabPotLoot> OceanLoot { get; private set; }
        public static List<CrabPotLoot> SpaceLoot { get; private set; }
        public static List<CrabPotLoot> UndergroundCavernLoot { get; private set; }

        public static Action<int, int> FishingCheck_OnCheckTile;
        public static Action FishingCheck_AfterCheckTiles;
        public static Action<List<int>> FishingCheck_FinalLootAdditions;

        internal static void InternalInitialize()
        {
            NormalLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.Bass)
            };

            BloodMoonLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ModContent.ItemType<PalePufferfish>()),
                new CrabPotLoot(ModContent.ItemType<VampireSquid>(), hardmodeOnly: true)
            };

            CrimsonLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.Hemopiranha),
                new CrabPotLoot(ItemID.CrimsonTigerfish),
                new CrabPotLoot(ModContent.ItemType<Fleshscale>(), afterBoss2: true)
            };

            CorruptionLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.Ebonkoi),
                new CrabPotLoot(ModContent.ItemType<Fizzler>(), time: false),
                new CrabPotLoot(ModContent.ItemType<Depthscale>(), afterBoss2: true)
            };

            JungleLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.DoubleCod),
                new CrabPotLoot(ItemID.VariegatedLardfish)
            };

            HallowLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.PrincessFish),
                new CrabPotLoot(ItemID.Prismite),
                new CrabPotLoot(ItemID.ChaosFish,
                validWorldLayers: new byte[] { FishLoader.WorldLayers.UndergroundLayer, FishLoader.WorldLayers.CavernLayer })
            };

            SnowLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.AtlanticCod),
                new CrabPotLoot(ItemID.FrostMinnow)
            };

            OceanLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.Shrimp),
                new CrabPotLoot(ItemID.BlueJellyfish),
                new CrabPotLoot(ItemID.GreenJellyfish),
                new CrabPotLoot(ItemID.PinkJellyfish)
            };

            SpaceLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.Damselfish)
            };

            UndergroundCavernLoot = new List<CrabPotLoot>
            {
                new CrabPotLoot(ItemID.Stinkfish),
                new CrabPotLoot(ItemID.SpecularFish),
                new CrabPotLoot(ItemID.ArmoredCavefish)
            };
        }

        public static void ResetFishingParameters()
        {
            CanCatchNormalFish = true;
            BloodMoon = false;
            Ocean = false;
            Hallow = false;
            Crimson = false;
            Corruption = false;
            Jungle = false;
            Desert = false;
            Snow = false;
        }

        private static void AddToList(List<int> choices, List<CrabPotLoot> lootTable)
        {
            if (lootTable != null)
            {
                foreach (var loot in lootTable)
                {
                    if (loot.CanCatch(WorldLayer))
                        choices.Add(loot.CatchItem);
                }
            }
        }

        public static int CaptureFish(int x, int y, byte waterType)
        {
            ResetFishingParameters();
            if (waterType == Tile.Liquid_Honey)
            {
                return 0;
            }

            WorldLayer = (byte)((!(y < Main.worldSurface * 0.5)) ? ((y < Main.worldSurface) ? FishLoader.WorldLayers.Overworld : ((y < Main.rockLayer) ? FishLoader.WorldLayers.UndergroundLayer : ((y >= Main.maxTilesY - 300) ? FishLoader.WorldLayers.HellLayer : FishLoader.WorldLayers.CavernLayer))) : FishLoader.WorldLayers.Space);

            //Main.NewText(WorldLayer, Microsoft.Xna.Framework.Color.Aqua);

            if (waterType == Tile.Liquid_Lava)
            {
                return Main.rand.NextBool() ? ItemID.FlarefinKoi : ItemID.Obsidifish;
            }
            else if (WorldLayer == FishLoader.WorldLayers.HellLayer)
            {
                return 0;
            }
            for (int j = 0; j < 30; j++)
            {
                if (y + j > Main.maxTilesY - 10)
                {
                    continue;
                }
                var tile = Main.tile[x, y + j];
                if (tile == null)
                {
                    continue;
                }
                if (!Ocean)
                {
                    switch (tile.type)
                    {
                        case TileID.Sand:
                        case TileID.Ebonsand:
                        case TileID.Crimsand:
                        case TileID.Pearlsand:
                        case TileID.HardenedSand:
                        case TileID.CorruptHardenedSand:
                        case TileID.CrimsonHardenedSand:
                        case TileID.HallowHardenedSand:
                        case TileID.Sandstone:
                        case TileID.CorruptSandstone:
                        case TileID.CrimsonSandstone:
                        case TileID.HallowSandstone:
                            {
                                Desert = true;
                            }
                            break;
                    }
                }
                if (tile.wall == WallID.Sandstone)
                {
                    Desert = true;
                }
                if (TileID.Sets.Snow[tile.type] || TileID.Sets.Conversion.Ice[tile.type])
                {
                    Snow = true;
                }
                if (TileID.Sets.Hallow[tile.type])
                {
                    Hallow = true;
                }
                if (TileID.Sets.Corrupt[tile.type])
                {
                    Corruption = true;
                    Hallow = false;
                    break;
                }
                if (TileID.Sets.Crimson[tile.type])
                {
                    Crimson = true;
                    Hallow = false;
                    break;
                }
                if (tile.type == TileID.JungleGrass || tile.type == TileID.LihzahrdBrick)
                {
                    Jungle = true;
                    break;
                }
                if (FishingCheck_OnCheckTile != null)
                    FishingCheck_OnCheckTile.Invoke(x, y + j);
            }
            Ocean = x < 200 || x > Main.maxTilesX * 16f - 3200f;
            BloodMoon = Main.bloodMoon;
            FishCheck_NormalFish();
            if (FishingCheck_AfterCheckTiles != null)
                FishingCheck_AfterCheckTiles.Invoke();

            List<int> choices = new List<int>();

            if (WorldLayer <= FishLoader.WorldLayers.Overworld)
            {
                if (BloodMoon)
                {
                    AddToList(choices, BloodMoonLoot);
                }
            }
            if (Crimson)
            {
                AddToList(choices, CrimsonLoot);
            }
            else if (Corruption)
            {
                AddToList(choices, CorruptionLoot);
            }
            else if (Jungle)
            {
                AddToList(choices, JungleLoot);
            }
            else if (Hallow)
            {
                AddToList(choices, HallowLoot);
            }
            if (Ocean && WorldLayer == FishLoader.WorldLayers.Overworld)
            {
                AddToList(choices, OceanLoot);
            }
            else if (Snow)
            {
                AddToList(choices, SnowLoot);
            }

            if (CanCatchNormalFish)
            {
                if (WorldLayer == FishLoader.WorldLayers.Space)
                {
                    AddToList(choices, SpaceLoot);
                }
                else if (WorldLayer == FishLoader.WorldLayers.UndergroundLayer || WorldLayer == FishLoader.WorldLayers.CavernLayer)
                {
                    AddToList(choices, UndergroundCavernLoot);
                }
                if (WorldLayer <= FishLoader.WorldLayers.Overworld)
                {
                    AddToList(choices, NormalLoot);
                }
            }

            if (FishingCheck_FinalLootAdditions != null)
                FishingCheck_FinalLootAdditions.Invoke(choices);

            if (choices.Count == 0)
                return 0;
            if (choices.Count == 1)
                return choices[0];
            if (choices.Count > 1)
                return choices[Main.rand.Next(choices.Count)];
            return 0;
        }

        void IAutoloadType.OnLoad()
        {
            InternalInitialize();
        }

        void IAutoloadType.Unload()
        {
            NormalLoot = null;
            BloodMoonLoot = null;
            CrimsonLoot = null;
            CorruptionLoot = null;
            JungleLoot = null;
            HallowLoot = null;
            SnowLoot = null;
            OceanLoot = null;
            SpaceLoot = null;
            UndergroundCavernLoot = null;
        }
    }
}