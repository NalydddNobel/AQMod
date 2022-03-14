using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public sealed class LootDrops : GlobalNPC
    {
        public override bool PreNPCLoot(NPC npc)
        {
            if (npc.type == NPCID.BlueJellyfish && ModContent.GetInstance<AQConfigServer>().removeJellyfishNecklace)
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
            if (!npc.boss && npc.lifeMax > 5 && !npc.friendly && !npc.townNPC && !AQNPC.Sets.NoGlobalDrops.Contains(npc.type))
            {
                DropBiomeLoot(npc, plr, aQPlayer);
            }
            if (npc.type >= Main.maxNPCTypes)
                return;
            if (Main.moonPhase == MoonPhases.FullMoon)
            {
                if (npc.type == NPCID.Golem)
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<RustyKnife>());
                }
            }
            int banner = Item.NPCtoBanner(npc.type);
            if (npc.type == NPCID.MossHornet)
            {
                if (Main.rand.NextBool(80))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Beeswax>());
            }
            else if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.GreenJellyfish)
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