using AQMod.Common.Configuration;
using AQMod.Common.ID;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Potions.Foods;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Melee;
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
            public bool? corruption;
            public bool? crimson;
            public bool? hallow;
            public bool? jungle;
            public bool hardmodeOnly;

            public DropConditions(bool setup = false) : this()
            {
                if (setup)
                {
                    SetupStatics();
                }
            }
            public DropConditions(Player player) : this(setup: true)
            {
                FillOutPlayer(player, player.GetModPlayer<AQPlayer>());
            }

            public void SetupStatics()
            {
                moonPhase = Main.moonPhase;
                hardmodeOnly = Main.hardMode;
            }

            private void GiveValue(ref bool? me, bool value)
            {
                if (value)
                {
                    me = true;
                }
                else if (opposingPotion == null)
                {
                    me = false;
                }
            }
            public void FillOutPlayer(Player player, AQPlayer aQPlayer)
            {
                GiveValue(ref opposingPotion, aQPlayer.altEvilDrops);
                GiveValue(ref corruption, player.ZoneCorrupt);
                GiveValue(ref crimson, player.ZoneCrimson);
                GiveValue(ref hallow, player.ZoneHoly);
                GiveValue(ref jungle, player.ZoneJungle);
            }

            private bool CompareEqual(bool? mine, bool? theirs)
            {
                return mine == null || theirs == mine;
            }
            private bool OnlyFlag(bool mine, bool theirs)
            {
                return mine ? theirs : true;
            }
            public override bool Equals(object obj)
            {
                if (obj is DropConditions conditions)
                {
                    return CompareEqual(opposingPotion, conditions.opposingPotion) ||
                        CompareEqual(corruption, conditions.corruption) ||
                        CompareEqual(crimson, conditions.crimson) ||
                        CompareEqual(hallow, conditions.hallow) ||
                        CompareEqual(jungle, conditions.jungle) ||
                        OnlyFlag(conditions.hardmodeOnly, hardmodeOnly);
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

            public int DropItemConditions(NPC npc, List<(Player, AQPlayer)> nearbyPlayers, DropConditions allValidConditions)
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

        internal static void LootTables()
        {
            CustomLoot = new Dictionary<int, AQUtils.ArrayInterpreter<Loot>>()
            {
                [NPCID.DevourerHead] = new Loot(ModContent.ItemType<SpicyEel>()) { chance = 50, },
                [NPCID.GiantWormHead] = new Loot(ModContent.ItemType<SpicyEel>()) { chance = 50, },
                [NPCID.BoneSerpentHead] = new Loot(ModContent.ItemType<SpicyEel>()) { chance = 50, },
                [NPCID.BlueJellyfish] = new Loot(ModContent.ItemType<ShockCollar>()) { chance = 15, },
                [NPCID.GreenJellyfish] = new Loot(ModContent.ItemType<Terraroot>(), 1, new DropConditions() { hardmodeOnly = true, }) { chance = 15, },
                [NPCID.DarkMummy] = new Loot(ItemID.LightShard, 1, new DropConditions() { opposingPotion = true, }) { chance = 10, },
                [NPCID.LightMummy] = new Loot(ItemID.DarkShard, 1, new DropConditions() { opposingPotion = true, }) { chance = 10, },
                [NPCID.DiggerHead] = new Loot(ModContent.ItemType<SpicyEel>()) { chance = 15, },
                [NPCID.SeekerHead] = new Loot(ModContent.ItemType<SpicyEel>()) { chance = 15, },
                [NPCID.UndeadViking] = new Loot(ModContent.ItemType<CrystalDagger>()) { chance = 10, },
                [NPCID.MossHornet] = new Loot(ModContent.ItemType<Beeswax>()) { chance = 80, },
                [NPCID.Golem] = new Loot(ModContent.ItemType<RustyKnife>(), 1, new DropConditions() { moonPhase = MoonPhases.FullMoon }),
                [NPCID.DungeonSpirit] = new Loot(ModContent.ItemType<Breadsoul>()) { chance = 45, },
                [NPCID.DuneSplicerHead] = new Loot(ModContent.ItemType<SpicyEel>()) { chance = 15, },
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