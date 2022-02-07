using AQMod.Common.NoHitting;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using AQMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public sealed class NPCLootChanger : GlobalNPC
    {
        public override bool PreNPCLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.BlueJellyfish:
                    {
                        if (ModContent.GetInstance<AQConfigServer>().removeJellyfishNecklace)
                        {
                            NPCLoader.blockLoot.Add(ItemID.JellyfishNecklace);
                        }
                    }
                    break;
            }
            return base.PreNPCLoot(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            byte p = Player.FindClosest(npc.position, npc.width, npc.height);
            var plr = Main.player[p];
            var aQPlayer = plr.GetModPlayer<AQPlayer>();
            if (npc.townNPC && npc.position.Y > (Main.maxTilesY - 200) * 16f) // does this for any town NPC because why not?
            {
                var check = new Rectangle((int)npc.position.X / 16, (int)npc.position.Y / 16, 2, 3);
                bool spawnedItem = false;
                for (int i = check.X; i <= check.X + check.Width; i++)
                {
                    for (int j = check.Y; j <= check.Y + check.Height; j++)
                    {
                        if (Framing.GetTileSafely(i, j).liquid > 0 && Main.tile[i, j].lava())
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<IWillBeBack>());
                            spawnedItem = true;
                            break;
                        }
                    }
                    if (spawnedItem)
                        break;
                }
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (npc.type == NPCID.Ghost)
                {
                    if (!aQPlayer.ghostAmuletHeld && Main.rand.NextBool(15))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<GhostAmulet>());
                }
            }
            if (!AQNPC.Sets.NoGlobalDrops[npc.type] && !npc.boss && npc.lifeMax > 5 && !npc.friendly && !npc.townNPC)
            {
                if (Main.hardMode && npc.position.Y > Main.rockLayer * 16.0 && npc.value > 0f)
                {
                    if (aQPlayer.altEvilDrops && Main.rand.NextBool(5))
                    {
                        if (plr.ZoneCorrupt || plr.ZoneCrimson)
                            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofLight);
                        if (plr.ZoneHoly)
                            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofNight);
                    }
                }
                var tile = Framing.GetTileSafely(Main.player[p].Center.ToTileCoordinates());
                if (!Main.wallHouse[tile.wall])
                {
                    if (Main.player[p].ZoneJungle && tile.wall != TileID.LihzahrdBrick)
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
            if (npc.type >= Main.maxNPCTypes)
                return;
            switch (npc.type)
            {
                case NPCID.BlueJellyfish:
                case NPCID.GreenJellyfish:
                    {
                        if (Main.rand.NextBool(15))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<ShockCollar>());
                    }
                    break;

                case NPCID.SeekerHead:
                    {
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<SpicyEel>());
                    }
                    break;

                case NPCID.DiggerHead:
                    {
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<SpicyEel>());
                    }
                    break;

                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                    {
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<GrapePhanta>());
                    }
                    break;

                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                    {
                        if (Main.rand.NextBool(40))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<GrapePhanta>());
                    }
                    break;

                case NPCID.UndeadViking:
                    {
                        if (Main.rand.NextBool(6))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<CrystalDagger>());
                    }
                    break;

                case NPCID.DarkMummy:
                    {
                        if (aQPlayer.altEvilDrops && Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ItemID.LightShard);
                    }
                    break;

                case NPCID.LightMummy:
                    {
                        if (aQPlayer.altEvilDrops && Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ItemID.DarkShard);
                    }
                    break;

                case NPCID.DungeonSpirit:
                    if (Main.rand.NextBool(45))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Breadsoul>());
                    break;

                case NPCID.Necromancer:
                    if (Main.rand.NextBool(30))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Breadsoul>());
                    break;

                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                    if (Main.rand.NextBool(30))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Dreadsoul>());
                    break;

                case NPCID.Mothron:
                    if (NPC.downedAncientCultist && ((npc.playerInteraction[p] && npc.GetGlobalNPC<NoHitManager>().hitPlayer[p]) || Main.rand.NextBool(10)))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<MothmanMask>());
                    break;

                case NPCID.Golem:
                    {
                        if (Main.moonPhase == 0)
                            Item.NewItem(npc.getRect(), ModContent.ItemType<RustyKnife>());
                    }
                    break;
            }
        }
    }
}