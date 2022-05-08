using Aequus.NPCs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
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
            NPCID.Sets.HatOffsetY[NPC.type] = 8;

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
            .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Hate)
            .SetNPCAffection(NPCID.Pirate, AffectionLevel.Love)
            .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);
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
            if (player.armor[0].headSlot == 97 && Main.rand.NextBool())
            {
                return GetChatText("VanityEyePatch");
            }
            if (player.armor[0].headSlot == 68 && Main.rand.NextBool())
            {
                return GetChatText("PirateVanitySet");
            }
            if (Main.xMas && Main.rand.NextBool())
            {
                return GetChatText("XMas");
            }

            return GetTextFromList(player);
        }
        private string GetTextFromList(Player player)
        {
            List<string> textChoices = new List<string>()
            {
                GetChatText("Basic.0"),
                GetChatText("Basic.1"),
                GetChatText("Basic.2"),
                GetChatText("Basic.3"),
                GetChatText("Basic.4"),
            };

            if (!Main.dayTime)
            {
                textChoices.Add(GetChatText("Night.0"));
                textChoices.Add(GetChatText("Night.1"));
                textChoices.Add(GetChatText("Night.2"));
            }

            if (player.ZoneBeach)
            {
                textChoices.Add(GetChatText("Ocean.0"));
                textChoices.Add(GetChatText("Ocean.1"));
                textChoices.Add(GetChatText("Ocean.2"));
                // temporary
                textChoices.Add(GetChatText("CrabCrevice.0"));
                textChoices.Add(GetChatText("CrabCrevice.1"));
                textChoices.Add(GetChatText("CrabCrevice.2"));
            }

            if (Main.rand.NextBool())
            {
                FindAndAddText(textChoices, NPCID.Angler, "AnglerTownNPC");
                FindAndAddText(textChoices, NPCID.Pirate, "PirateTownNPC");
                FindAndAddText(textChoices, NPCID.Dryad, "DryadTownNPC");
                FindAndAddText(textChoices, NPCID.BestiaryGirl, "ZoologistTownNPC");
                FindAndAddText(textChoices, NPCID.Truffle, "TruffleTownNPC");
            }
            else
            {
                FindAndAddText(textChoices, NPCID.ArmsDealer, "ArmsDealerTownNPC");
                FindAndAddText(textChoices, NPCID.Demolitionist, "DemolitionistTownNPC");
                FindAndAddText(textChoices, NPCID.TaxCollector, "TaxCollectorTownNPC");
                FindAndAddText(textChoices, NPCID.TravellingMerchant, "TravellingMerchantTownNPC");
                FindAndAddText(textChoices, NPCID.Stylist, "StylistTownNPC");
            }

            if (Main.rand.NextBool(4) || NPC.AnyNPCs(ModContent.NPCType<Crabson>()))
            {
                textChoices.Add(GetChatText("Crabson"));
            }

            if (Main.invasionType == InvasionID.PirateInvasion || (Main.rand.NextBool(4) && NPC.downedPirates))
            {
                textChoices.Add(GetChatText("PirateInvasion"));
            }

            return textChoices[Main.rand.Next(textChoices.Count)];
        }
        private void FindAndAddText(List<string> textChoices, int npc, string key)
        {
            int n = NPC.FindFirstNPC(npc);
            if (n != -1)
            {
                textChoices.Add(GetChatText(key, Main.npc[n].GivenOrTypeName));
            }
        }
        private string GetChatText(string key, params object[] args)
        {
            return Language.GetTextValue("Mods.Aequus.Chat.Exporter." + key, args);
        }
        private string GetChatText(string key)
        {
            return Language.GetTextValue("Mods.Aequus.Chat.Exporter." + key);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("Mods.Aequus.UpgradeGrapplingHooks");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                Main.playerInventory = true;
                Main.npcChatText = "";
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            if (Main.hardMode)
            {
                if (NPC.downedPirates)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.DiscountCard);
                    nextSlot++;
                }
                //shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                //nextSlot++;
                //shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpoilsPotion>());
                //nextSlot++;
                //shop.item[nextSlot].SetDefaults(ModContent.ItemType<GoldPowder>());
                //nextSlot++;
            }
            else
            {
                //shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                //nextSlot++;
            }
            if (NPC.downedBoss3 && Main.moonPhase % 2 == 1)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 15);
                nextSlot++;
            }
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<FishingCraftingStation>());
            //nextSlot++;
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<CrabClock>());
            //nextSlot++;
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