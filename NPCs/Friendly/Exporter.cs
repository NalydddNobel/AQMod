using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Vanity;
using Aequus.Items.Consumables.LootBags.Roulettes;
using Aequus.Items.Consumables.Summons;
using Aequus.Items.Placeable;
using Aequus.Items.Placeable.Furniture;
using Aequus.Items.Tools;
using Aequus.NPCs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly
{
    [AutoloadHead()]
    public class Exporter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 3; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
            NPCID.Sets.AttackTime[NPC.type] = 10;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1,
            });

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Lovestruck,
                }
            });

            NPC.Happiness
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Like)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Love)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Clothier, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);

            NPCHappiness.Get(NPCID.Dryad).SetNPCAffection(Type, AffectionLevel.Love);
            NPCHappiness.Get(NPCID.Angler).SetNPCAffection(Type, AffectionLevel.Love);
            NPCHappiness.Get(NPCID.Pirate).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.ArmsDealer).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.TaxCollector).SetNPCAffection(Type, AffectionLevel.Hate);

            ShopQuotes.Database
                .GetNPC(Type)
                .WithColor(Color.Orange * 1.2f)
                .LegacyAddQuote<GrandReward>()
                .LegacyAddQuote<SkeletonKey>()
                .LegacyAddQuote<RecyclingMachine>()
                .LegacyAddQuote<ForgedCard>()
                .LegacyAddQuote<FaultyCoin>()
                .LegacyAddQuote<FoolsGoldRing>()
                .LegacyAddQuote(ItemID.DiscountCard)
                .LegacyAddQuote(ItemID.LuckyCoin)
                .LegacyAddQuote(ItemID.GoldRing);

            ExporterQuests.NPCTypesNoSpawns.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = NPC.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddSpawn(BestiaryBuilder.OceanBiome);
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            Main.LocalPlayer.discount = false;

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ForgedCard>());
            if (NPC.downedPirates)
            {
                shop.item[nextSlot].SetDefaults(ItemID.DiscountCard);
                shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FaultyCoin>());
            if (NPC.downedPirates)
            {
                shop.item[nextSlot].SetDefaults(ItemID.LuckyCoin);
                shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FoolsGoldRing>());
            if (NPC.downedPirates)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoldRing);
                shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GrandReward>());

            if (NPC.downedBoss3)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkeletonKey>());
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<RecyclingMachine>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<RichMansMonocle>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FishyFins>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CrabClock>());
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<HypnoticPearl>());
            shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 5);
        }

        //public static bool TryPlaceQuestTile(int x, int y)
        //{
        //    int choice = Main.rand.Next(3);
        //    if (choice == 2)
        //    {
        //        if (!Framing.GetTileSafely(x, y - 1).active() || !Main.tile[x, y - 1].Solid())
        //        {
        //            return false;
        //        }
        //        for (int i = -1; i <= 1; i++)
        //        {
        //            for (int j = 0; j < 3; j++)
        //            {
        //                if (Framing.GetTileSafely(x + i, y + j).active())
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //        for (int i = -1; i <= 1; i++)
        //        {
        //            for (int j = 0; j < 3; j++)
        //            {
        //                var t = Main.tile[x + i, y + j];
        //                t.active(active: true);
        //                t.type = (ushort)ModContent.TileType<JeweledChandlierTile>();
        //                t.frameX = (short)(18 * (i + 1));
        //                t.frameY = (short)(18 * j);
        //                t.slope(slope: 0);
        //                t.halfBrick(halfBrick: false);
        //                t.color(color: 0);
        //            }
        //        }
        //        if (Main.netMode == NetmodeID.Server)
        //            NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
        //        return true;
        //    }
        //    else if (choice == 1)
        //    {
        //        if (!Framing.GetTileSafely(x, y).active() && !Framing.GetTileSafely(x, y + 1).active() && !Framing.GetTileSafely(x + 1, y).active() && !Framing.GetTileSafely(x + 1, y + 1).active() && Framing.GetTileSafely(x, y + 2).active() && Main.tileSolidTop[Main.tile[x, y + 2].type] && Framing.GetTileSafely(x + 1, y + 2).active() && Main.tileSolidTop[Main.tile[x + 1, y + 2].type])
        //        {
        //            WorldGen.PlaceTile(x, y, ModContent.TileType<JeweledCandelabraTile>(), true, false, -1, 0);
        //            if (Framing.GetTileSafely(x, y).type == ModContent.TileType<JeweledCandelabraTile>())
        //            {
        //                if (Main.netMode == NetmodeID.Server)
        //                    NetMessage.SendTileSquare(-1, x - 2, y - 2, 4);
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (!Framing.GetTileSafely(x, y).active() && Framing.GetTileSafely(x, y + 1).active() && Main.tileSolidTop[Main.tile[x, y + 1].type])
        //        {
        //            WorldGen.PlaceTile(x, y, ModContent.TileType<JeweledChaliceTile>(), true, false, -1, 0);
        //            if (Framing.GetTileSafely(x, y).type == ModContent.TileType<JeweledChaliceTile>())
        //            {
        //                if (Main.netMode == NetmodeID.Server)
        //                    NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        public override void AI()
        {
            NPC.breath = 200;
            //try
            //{
            //    if (Main.netMode != NetmodeID.MultiplayerClient)
            //    {
            //        NPCCheck++;
            //        if (NPCCheck >= 240)
            //        {
            //            if (NPCCheck >= 241 || Main.rand.NextBool(50))
            //            {
            //                List<int> townNPCs = new List<int>();
            //                for (int i = 0; i < Main.maxNPCs; i++)
            //                {
            //                    if (Main.npc[i] != null && Main.npc[i].active && Main.npc[i].townNPC && !Main.npc[i].homeless && Main.npc[i].type != NPC.type)
            //                    {
            //                        townNPCs.Add(i);
            //                    }
            //                }
            //                if (townNPCs.Count <= 0)
            //                {
            //                    NPCCheck = 0;
            //                }
            //                else
            //                {
            //                    for (int i = 0; i < 10; i++)
            //                    {
            //                        byte npc = (byte)townNPCs[Main.rand.Next(townNPCs.Count)];
            //                        int x = Main.npc[npc].homeTileX;
            //                        int y = Main.npc[npc].homeTileY;
            //                        var checkRectangle = new Rectangle(x - 75, y - 75, 150, 150).KeepInWorld();
            //                        for (int k = checkRectangle.X; k < checkRectangle.X + checkRectangle.Width; k++)
            //                        {
            //                            for (int l = checkRectangle.Y; l < checkRectangle.Y + checkRectangle.Height; l++)
            //                            {
            //                                if (Main.tile[k, l] == null)
            //                                {
            //                                    Main.tile[k, l] = new Tile();
            //                                    continue;
            //                                }
            //                                if (Main.tile[k, l].active() && AQTile.Sets.Instance.ExporterQuestFurniture.Contains(Main.tile[k, l].type))
            //                                {
            //                                    return;
            //                                }
            //                            }
            //                        }
            //                        checkRectangle = new Rectangle(x - 8, y - 8, 16, 16).KeepInWorld();
            //                        for (int k = checkRectangle.X; k < checkRectangle.X + checkRectangle.Width; k++)
            //                        {
            //                            for (int l = checkRectangle.Y; l < checkRectangle.Y + checkRectangle.Height; l++)
            //                            {
            //                                int randomX = checkRectangle.X + Main.rand.Next(checkRectangle.Width);
            //                                int randomY = checkRectangle.Y + Main.rand.Next(checkRectangle.Height);
            //                                if (TryPlaceQuestTile(randomX, randomY))
            //                                {
            //                                    NPCCheck = 0;
            //                                    return;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                NPCCheck = 0;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    AQMod.Instance.Logger.Error(ex);
            //}
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return AequusWorld.downedCrabson;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Larry",
                "Captain",
                "Jailer",
                "Reaver",
                "Barnacle",
                "Eugene",
                "Robster",
                "Clicky",
                "Clacky",
                "Clocky",
                "Snapper",
                "Catcher",
                "Latcher",
            };
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return base.TownNPCProfile();
        }

        public override string GetChat()
        {
            var player = Main.LocalPlayer;
            //if (player.armor[0].headSlot == 97 && Main.rand.NextBool())
            //{
            //    return GetChatText("VanityEyePatch");
            //}
            //if (player.armor[0].headSlot == 68 && Main.rand.NextBool())
            //{
            //    return GetChatText("PirateVanitySet");
            //}
            //if (Main.xMas && Main.rand.NextBool())
            //{
            //    return GetChatText("XMas");
            //}

            var chat = new SelectableChat("Mods.Aequus.Chat.Exporter.");

            chat.Add("Basic.0");
            chat.Add("Basic.1");

            if (!Main.dayTime)
            {
                chat.Add("Night.0");
                chat.Add("Night.1");
                //textChoices.Add(GetChatText("Night.2"));
                if (Main.bloodMoon)
                {
                    chat.Add("BloodMoon.0");
                    chat.Add("BloodMoon.1");

                    if (NPC.killCount[NPCID.WanderingEye] > 0)
                    {
                        chat.Add("BloodMoon.WanderingEyeFish");
                    }
                }
            }

            if (player.ZoneBeach)
            {
                chat.Add("Ocean.0");
                //chat.Add("Ocean.1");
                //chat.Add("Ocean.2");

                chat.Add("CrabCrevice.0");
                //chat.Add("CrabCrevice.1");
                //chat.Add("CrabCrevice.2");
            }

            if (NPC.AnyNPCs(NPCID.Angler))
                chat.Add("Angler", () => new { Angler = NPC.GetFirstNPCNameOrNull(NPCID.Angler) });
            if (NPC.AnyNPCs(NPCID.Pirate))
                chat.Add("Pirate", () => new { Pirate = NPC.GetFirstNPCNameOrNull(NPCID.Pirate) });
            if (NPC.AnyNPCs(NPCID.Truffle))
                chat.Add("Truffle", () => new { Truffle = NPC.GetFirstNPCNameOrNull(NPCID.Truffle) });
            if (NPC.AnyNPCs(NPCID.TaxCollector))
                chat.Add("TaxCollector", () => new { TaxCollector = NPC.GetFirstNPCNameOrNull(NPCID.TaxCollector) });
            if (NPC.AnyNPCs(NPCID.Stylist))
                chat.Add("Stylist", () => new { Stylist = NPC.GetFirstNPCNameOrNull(NPCID.Stylist) });

            if (Main.rand.NextBool(4) || NPC.AnyNPCs(ModContent.NPCType<Crabson>()))
            {
                chat.Add("Crabson");
            }

            if (Main.invasionType == InvasionID.PirateInvasion || (Main.rand.NextBool(4) && NPC.downedPirates))
            {
                chat.Add("PirateInvasion");
            }

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = AequusText.GetText("Chat.Exporter.ThieveryButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                {
                    if (QuestItem(Main.LocalPlayer, i))
                    {
                        OnQuestCompleted(Main.LocalPlayer, i);
                        return;
                    }
                }
                OnQuestFailed(Main.LocalPlayer);
            }
        }
        public bool QuestItem(Player player, int i)
        {
            return !player.inventory[i].IsAir && player.inventory[i].createTile >= TileID.Dirt && ExporterQuests.TilePlacements.ContainsKey(player.inventory[i].createTile);
        }
        public void OnQuestCompleted(Player player, int i)
        {
            player.inventory[i].stack--;
            if (player.inventory[i].stack <= 0)
            {
                player.inventory[i].TurnToAir();
            }

            InnerOnQuestCompleted_SpawnLoot(player, i);

            SoundEngine.PlaySound(SoundID.Grab);
            Main.npcChatText = AequusText.GetTextWith("Chat.Exporter.ThieveryComplete." + Main.rand.Next(5), new { ItemName = player.inventory[i].Name });
        }
        public void InnerOnQuestCompleted_SpawnLoot(Player player, int i)
        {
            var source = player.GetSource_GiftOrReward("Robster");

            if (Main.rand.NextBool(4))
            {
                player.QuickSpawnItem(source, ModContent.ItemType<GoldenRoulette>(), 1);
            }
            else
            {
                player.QuickSpawnItem(source, ModContent.ItemType<Roulette>(), 1);
            }

            int amtRolled = Math.Max(ExporterQuests.QuestsCompleted / 15, 1);
            for (int k = 0; k < amtRolled; k++)
            {
                int roulette = SpawnLoot_ChooseRoulette(player, i);

                if (roulette != 0)
                {
                    player.QuickSpawnItem(source, roulette, 1);
                }
            }

            int extraMoney = Item.silver * 3 * ExporterQuests.QuestsCompleted;
            AequusHelpers.DropMoney(source, player.getRect(), Main.rand.Next(Item.silver * 50 + extraMoney / 2, Item.gold + extraMoney));

            ExporterQuests.QuestsCompleted++;

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                PacketHandler.Send(PacketType.SetExporterQuestsCompleted);
            }
        }
        public int SpawnLoot_ChooseRoulette(Player player, int i)
        {
            var choices = new List<int>();
            if (Main.rand.NextBool(3))
            {
                choices.Add(ModContent.ItemType<Roulette>());
                choices.Add(ModContent.ItemType<GoldenRoulette>());
            }
            if (ExporterQuests.QuestsCompleted > 5)
            {
                choices.Add(ModContent.ItemType<GlowingMushroomsRoulette>()); ;
                choices.Add(ModContent.ItemType<SnowRoulette>());
            }
            if (ExporterQuests.QuestsCompleted > 10)
            {
                choices.Add(ModContent.ItemType<SkyRoulette>()); ;
                choices.Add(ModContent.ItemType<DesertRoulette>());
            }
            return choices.Count > 0 ? choices[Main.rand.Next(choices.Count)] : ItemID.None;
        }
        public void OnQuestFailed(Player player)
        {
            Main.npcChatText = AequusText.GetText("Chat.Exporter.ThieveryFailed." + Main.rand.Next(2));
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 12;
            randExtraCooldown = 20;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            int itemType = ItemID.TerraBlade;
            Main.instance.LoadItem(itemType);
            item = TextureAssets.Item[itemType].Value;
            itemSize = 40;
            scale = 0.5f;
            offset = new Vector2(0f, 0f);
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 30;
            itemHeight = 30;
        }
    }
}