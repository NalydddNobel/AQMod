using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Misc.Toggles;
using AQMod.Items.Potions.Foods;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public sealed class LootDrops : GlobalNPC
    {
        public struct DropConditions
        {
            public int? moonPhase;
            public bool? opposingPotion;

            public DropConditions(bool setup = false) : this()
            {
                if (setup)
                {
                    SetupStatics();
                }
            }
            public DropConditions(Player player) : this(setup: true)
            {
                FillOutPlayer(player);
            }

            public void SetupStatics()
            {
                moonPhase = Main.moonPhase;
            }

            public void FillOutPlayer(Player player)
            {
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                opposingPotion = aQPlayer.altEvilDrops;
            }

            private bool CompareFlags(bool? mine, bool? theirs)
            {
                return mine == null || theirs == mine;
            }
            public override bool Equals(object obj)
            {
                if (obj is DropConditions conditions)
                {
                    return CompareFlags(opposingPotion, conditions.opposingPotion);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public struct Loot
        {
            public DropConditions? myConditions;
            public bool perPlayer;

            public int chance;
            public List<int> item;
            public int stackMin;
            public int stackMax;

            public Loot(AQUtils.ArrayInterpreter<int> item, int stack = 1, DropConditions? conditions = null, bool perPlayer = false) : this(item, stack, -1, conditions, perPlayer)
            {
            }

            public Loot(AQUtils.ArrayInterpreter<int> item, int minStack, int maxStack, DropConditions? conditions = null, bool perPlayer = false) : this(item, minStack, maxStack, 1, conditions, perPlayer)
            {
            }

            public Loot(AQUtils.ArrayInterpreter<int> item, int minStack, int maxStack, int chance, DropConditions? conditions = null, bool perPlayer = false)
            {
                this.item = item.Arr.ToList();
                stackMin = minStack;
                stackMax = maxStack;
                this.chance = chance;
                this.perPlayer = perPlayer;
                myConditions = conditions;
            }

            public int DropItemConditions(NPC npc, List<Player> nearbyPlayers, DropConditions allValidConditions)
            {
                if (myConditions == null || allValidConditions.Equals(myConditions))
                {
                    if (perPlayer)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (Main.player[i].active && !Main.player[i].dead && (myConditions == null || new DropConditions(Main.player[i]).Equals(myConditions)))
                            {
                                AQItem.DropInstancedItem(i, npc.getRect(), RollItem(), RollStack());
                            }
                        }
                        return -2;
                    }
                    return DropItem(npc);
                }
                return -1;
            }

            public int DropItem(NPC npc)
            {
                if (RollChance())
                {
                    return Item.NewItem(npc.getRect(), RollItem(), RollStack());
                }
                return -1;
            }

            public int RollItem()
            {
                return item.Count == 1 ? item[0] : item[Main.rand.Next(item.Count)];
            }

            public bool RollChance()
            {
                return chance <= 1 || Main.rand.NextBool(chance);
            }

            public int RollStack()
            {
                if (stackMax == -1)
                {
                    return stackMin;
                }
                return Main.rand.Next(stackMin, stackMax);
            }
        }

        public static Dictionary<int, AQUtils.ArrayInterpreter<Loot>> CustomLoot { get; private set; }

        internal static void SetupContent()
        {
            CustomLoot = new Dictionary<int, AQUtils.ArrayInterpreter<Loot>>() 
            { 
                [NPCID.Golem] = new Loot(ModContent.ItemType<RustyKnife>(), 1, new DropConditions() { moonPhase = MoonPhases.FullMoon }) { }
            };
        }

        internal static void Unload()
        {
            CustomLoot?.Clear();
            CustomLoot = null;
        }

        public override bool PreNPCLoot(NPC npc)
        {
            if ((npc.type == NPCID.BlueJellyfish || npc.type == NPCID.GreenJellyfish) && ModContent.GetInstance<AQConfigServer>().removeJellyfishNecklace)
            {
                NPCLoader.blockLoot.Add(ItemID.JellyfishNecklace);
            }
            return true;
        }

        private void DropTerminator(NPC npc)
        {
            if (npc.townNPC && npc.position.Y > (Main.maxTilesY - 200) * 16f && AQMod.UnderworldCheck()) // does this for any town NPC because why not?
            {
                var check = new Rectangle((int)npc.position.X / 16, (int)npc.position.Y / 16, 2, 3).KeepInWorld(fluff: 10);
                for (int i = check.X; i <= check.X + check.Width; i++)
                {
                    for (int j = check.Y; j <= check.Y + check.Height; j++)
                    {
                        if (Framing.GetTileSafely(i, j).liquid > 0 && Main.tile[i, j].lava())
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<IWillBeBack>());
                            WorldDefeats.terminatorArm = true;
                            NetHelper.WorldStatus();
                            return;
                        }
                    }
                }
            }
        }
        private void DropBiomeLoot(NPC npc, Player plr, AQPlayer aQPlayer)
        {
            if (aQPlayer.altEvilDrops && Main.hardMode && npc.position.Y > Main.rockLayer * 16.0 && npc.value > 0f &&
                ((!plr.ZoneCorrupt && !plr.ZoneCrimson) || !plr.ZoneHoly) && Main.rand.NextBool(5))
            {
                if (plr.ZoneCorrupt || plr.ZoneCrimson)
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofLight);
                if (plr.ZoneHoly)
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofNight);
            }
            var tile = Framing.GetTileSafely(plr.Center.ToTileCoordinates());
            if (!Main.wallHouse[tile.wall])
            {
                if (plr.ZoneJungle && tile.wall != TileID.LihzahrdBrick)
                {
                    if (npc.lifeMax > (Main.expertMode ? Main.hardMode ? 150 : 80 : 30))
                    {
                        int chance = 14;
                        if (npc.lifeMax + npc.defDefense > 350 && npc.type != NPCID.MossHornet) // defDefense is the defense of the NPC when it spawns
                            chance /= 2;
                        if (Main.rand.NextBool(chance))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<OrganicEnergy>());
                    }
                }
            }

        }
        public override void NPCLoot(NPC npc)
        {
            byte p = Player.FindClosest(npc.position, npc.width, npc.height);
            var plr = Main.player[p];
            var aQPlayer = Main.player[p].GetModPlayer<AQPlayer>();
            DropTerminator(npc);
            if (!npc.boss && npc.lifeMax > 5 && !npc.friendly && !npc.townNPC && !AQNPC.Sets.Instance.NoGlobalDrops.Contains(npc.type))
            {
                DropBiomeLoot(npc, plr, aQPlayer);
            }

            //if (CustomLoot.TryGetValue(npc.type, out var value))
            //{
            //    foreach (var item in value.Arr)
            //    {
            //        item.DropItemConditions(npc, );
            //    }
            //}

            if (npc.type >= Main.maxNPCTypes)
                return;

            int banner = Item.NPCtoBanner(npc.type);
            if (npc.type == NPCID.MossHornet)
            {
                if (Main.rand.NextBool(80))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Beeswax>());
            }
            else if (npc.type == NPCID.GreenJellyfish)
            {
                if (Main.hardMode && Main.rand.NextBool(15))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Terraroot>());
            }
            else if (npc.type == NPCID.BlueJellyfish)
            {
                if (Main.rand.NextBool(15))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<ShockCollar>());
            }
            else if (npc.type == NPCID.SeekerHead || npc.type == NPCID.DiggerHead || npc.type == NPCID.DuneSplicerHead)
            {
                if (Main.rand.NextBool(15))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<SpicyEel>());
            }
            else if (npc.type == NPCID.GiantWormHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.DevourerHead)
            {
                if (Main.rand.NextBool(50))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<SpicyEel>());
            }
            else if (npc.type == NPCID.UndeadViking || npc.type == NPCID.ArmoredViking)
            {
                if (Main.rand.NextBool(10))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<CrystalDagger>());
            }
            else if (npc.type == NPCID.DarkMummy)
            {
                if (aQPlayer.altEvilDrops && Main.rand.NextBool(10))
                    Item.NewItem(npc.getRect(), ItemID.LightShard);
            }
            else if (npc.type == NPCID.LightMummy)
            {
                if (aQPlayer.altEvilDrops && Main.rand.NextBool(10))
                    Item.NewItem(npc.getRect(), ItemID.DarkShard);
            }
            else if (npc.type == NPCID.DungeonSpirit)
            {
                if (Main.rand.NextBool(45))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Breadsoul>());
            }
            else if (banner == BannerID.RaggedCaster)
            {
                if (Main.rand.NextBool(10))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<GrapePhanta>());
            }
            else if (banner == BannerID.RustyArmoredBones)
            {
                if (Main.rand.NextBool(50))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<GrapePhanta>());
            }
        }

        public static int DropItemChance(NPC npc, AQUtils.ArrayInterpreter<int> item, int chance)
        {
            if (Main.rand.NextBool(chance))
            {
                return -1;
            }
            return DropItem(npc, item);
        }
        public static int DropItemChance(NPC npc, AQUtils.ArrayInterpreter<int> item, int minStack, int maxStack, int chance)
        {
            if (Main.rand.NextBool(chance))
            {
                return -1;
            }
            return DropItem(npc, item, minStack, maxStack);
        }
        public static int DropItemChance(NPC npc, AQUtils.ArrayInterpreter<int> item, int stack, int chance)
        {
            if (Main.rand.NextBool(chance))
            {
                return -1;
            }
            return DropItem(npc, item, stack);
        }

        public static int DropItem(NPC npc, AQUtils.ArrayInterpreter<int> item)
        {
            return DropItem(npc, item, 1);
        }
        public static int DropItem(NPC npc, AQUtils.ArrayInterpreter<int> item, int minStack, int maxStack)
        {
            return DropItem(npc, item, Main.rand.Next(maxStack - minStack + 1) + minStack);
        }
        public static int DropItem(NPC npc, AQUtils.ArrayInterpreter<int> item, int stack)
        {
            return Item.NewItem(npc.getRect(), item.Arr[item.Arr.Length == 1 ? 0 : Main.rand.Next(item.Arr.Length)], stack);
        }
    }
}