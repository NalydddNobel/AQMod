using Aequus.Common.Utilities;
using Aequus.Content.Boss.DustDevil;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Fishing.Poles;
using Aequus.Content.Town.SkyMerchantNPC.NameTags;
using Aequus.Items;
using Aequus.Items.Accessories.Defensive;
using Aequus.Items.Consumables.SlotMachines;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Dyes.Ancient;
using Aequus.Items.Mounts;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ShopQuotesMod;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Town.SkyMerchantNPC
{
    [AutoloadHead()]
    public class SkyMerchant : ModNPC
    {
        public const int BalloonFrames = 5;

        public int currentAction;
        public bool setupShop;
        public int oldSpriteDirection;
        public int basketFrameCounter;
        public int basketFrame;
        public int balloonColor;
        public bool init;

        public Item shopBanner;
        public Item shopAccessory;

        public static bool IsActive => Main.WindyEnoughForKiteDrops;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 50;
            NPCID.Sets.HatOffsetY[Type] = 0;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.SpawnsWithCustomName[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = -1f,
                Direction = -1
            });

            ModContent.GetInstance<QuoteDatabase>().AddNPC(Type, Mod, "Mods.Aequus.ShopQuote.")
                .AddDefaultText((i) =>
                {
                    int bannerID = AequusItem.ItemToBanner(i);
                    if (bannerID != 0)
                    {
                        return Language.GetTextValue("Mods.Aequus.ShopQuote.SkyMerchant.Banners");
                    }
                    return null;
                })
                .AddDefaultText((i) =>
                {
                    if (i == ModContent.ItemType<FlashwayNecklace>())
                        return null;
                    return ContentSamples.ItemsByType[i].accessory ? Language.GetTextValue("Mods.Aequus.ShopQuote.SkyMerchant.EquippedAcc") : null;
                })
                .SetQuote(ModContent.ItemType<SkyrimRock1>(), "Mods.Aequus.ShopQuote.SkyMerchant.SkyrimRocks")
                .SetQuote(ModContent.ItemType<SkyrimRock2>(), "Mods.Aequus.ShopQuote.SkyMerchant.SkyrimRocks")
                .SetQuote(ModContent.ItemType<SkyrimRock3>(), "Mods.Aequus.ShopQuote.SkyMerchant.SkyrimRocks")
                .UseColor(Color.DarkOliveGreen * 1.75f);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.SkeletonMerchant;
            currentAction = 7;

            init = false;
            setupShop = false;
            shopBanner = null;
            shopAccessory = null;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.SkyBiome)
                .AddSpawn(BestiaryBuilder.WindyDayEvent);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = TextHelper.GetTextValue("Chat.SkyMerchant.RenameChatButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
                return;
            }

            Main.playerInventory = true;
            Main.npcChatText = "";
            Aequus.UserInterface.SetState(new RenameItemUIState());
        }

        public void DyeItems(Chest shop, ref int nextSlot)
        {
            switch (Main.GetMoonPhase())
            {
                case MoonPhase.Full:
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AncientBreakdownDye>());
                    break;
                case MoonPhase.ThreeQuartersAtLeft:
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CensorDye>());
                    break;
                case MoonPhase.HalfAtLeft:
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OutlineDye>());
                    break;
                case MoonPhase.QuarterAtLeft:
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ScrollDye>());
                    break;
                case MoonPhase.QuarterAtRight:
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SimplifiedDye>());
                    break;
                case MoonPhase.HalfAtRight:
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<AncientHueshiftDye>());
                    break;
            }
        }
        public void SkyrimRocksPaintings(Chest shop, ref int nextSlot)
        {
            int bossDefeated = 0;
            if (NPC.downedBoss1)
                bossDefeated++;
            if (NPC.downedBoss2)
                bossDefeated++;
            if (NPC.downedBoss3)
                bossDefeated++;

            if (bossDefeated >= 1)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkyrimRock1>());
            }
            if (bossDefeated >= 2)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkyrimRock2>());
            }
            if (bossDefeated >= 3)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkyrimRock3>());
            }
        }
        public void PaintingItems(Chest shop, ref int nextSlot)
        {
            if (Main.getGoodWorld || Main.GetMoonPhase() == MoonPhase.Full)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BongBongPainting>());
            }
            if (NPC.downedMoonlord)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CatalystPainting>());
            }
            if (NPC.downedEmpressOfLight && NPC.downedAncientCultist)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<YinYangPainting>());
            }
            if (AequusWorld.downedEventDemon || NPC.downedAncientCultist)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<YinPainting>());
            }
            if (NPC.downedQueenSlime || NPC.downedEmpressOfLight)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<YangPainting>());
            }
            SkyrimRocksPaintings(shop, ref nextSlot);
        }
        public void SlotMachineItems(Chest shop, ref int nextSlot)
        {
            if (!Main.dayTime)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GoldenRoulette>());
                switch (Main.GetMoonPhase())
                {
                    case MoonPhase.QuarterAtRight:
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<JungleSlotMachine>());
                        break;
                    case MoonPhase.ThreeQuartersAtRight:
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OceanSlotMachine>());
                        break;
                    case MoonPhase.Full:
                        if (NPC.downedBoss3 || AequusWorld.downedEventDemon)
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ShadowRoulette>());
                        break;
                    case MoonPhase.ThreeQuartersAtLeft:
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkyRoulette>());
                        break;
                    case MoonPhase.QuarterAtLeft:
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DesertRoulette>());
                        break;
                    case MoonPhase.Empty:
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SnowRoulette>());
                        break;
                }
            }
            else
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Roulette>());
            }
        }
        public void MerchantInstanceItems(SkyMerchant merchant, Chest shop, ref int nextSlot)
        {
            if (merchant.shopAccessory != null)
            {
                shop.item[nextSlot] = merchant.shopAccessory.Clone();
                shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].value * 1.5f);
                shop.item[nextSlot].shopCustomPrice /= 100;
                shop.item[nextSlot].shopCustomPrice *= 100;
                shop.item[nextSlot].shopCustomPrice = Math.Max(shop.item[nextSlot].shopCustomPrice.Value, Item.buyPrice(gold: 5));
                nextSlot++;
            }
            if (merchant.shopBanner != null)
            {
                shop.item[nextSlot] = merchant.shopBanner.Clone();
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value * 10;
                nextSlot++;
            }
        }
        public void SkywareChestItems(Chest shop, ref int nextSlot)
        {
            switch (Main.GetMoonPhase())
            {
                case MoonPhase.Full:
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Starfury);
                        shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
                        if (Main.getGoodWorld || Main.bloodMoon || Main.eclipse || GlimmerBiomeManager.EventActive)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.CreativeWings);
                            shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
                        }
                    }
                    break;

                case MoonPhase.HalfAtLeft:
                case MoonPhase.HalfAtRight:
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.ShinyRedBalloon);
                        shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
                    }
                    break;

                case MoonPhase.QuarterAtLeft:
                case MoonPhase.QuarterAtRight:
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.LuckyHorseshoe);
                        shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
                    }
                    break;

                case MoonPhase.ThreeQuartersAtLeft:
                case MoonPhase.ThreeQuartersAtRight:
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.CelestialMagnet);
                        shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 10);
                    }
                    break;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            try
            {
                if (Main.LocalPlayer.talkNPC == -1 || !(Main.npc[Main.LocalPlayer.talkNPC].ModNPC is SkyMerchant))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            var merchant = (SkyMerchant)npc.ModNPC;
            if (!merchant.setupShop)
            {
                merchant.SetupShopCache(Main.LocalPlayer);
                npc.netUpdate = true;
                merchant.setupShop = true;
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BalloonKit>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Pumpinator>());
            if (Main.raining)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Nimrod>());
            }
            if (NPC.downedBoss1)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FlashwayNecklace>());
            }
            if (NPC.AnyNPCs(NPCID.Dryad))
            {
                SkywareChestItems(shop, ref nextSlot);
            }
            if (Main.dayTime)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.SkyMill);
                shop.item[nextSlot].SetDefaults(Main.raining ? ItemID.RainCloud : ItemID.Cloud);
                shop.item[nextSlot++].shopCustomPrice = Item.sellPrice(copper: 3);
            }
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<NameTag>());

            SlotMachineItems(shop, ref nextSlot);

            if (merchant != null)
            {
                MerchantInstanceItems(merchant, shop, ref nextSlot);
            }

            PaintingItems(shop, ref nextSlot);
            DyeItems(shop, ref nextSlot);

            if (AequusWorld.downedDustDevil)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<TornadoInABottle>());
            }
        }
        public void SetupShopCache(Player player)
        {
            shopBanner = GetBannerItem();
            shopAccessory = GetAccessoryItem(player);
        }
        public Item GetAccessoryItem(Player player)
        {
            var selectable = new List<Item>();
            var searchPrefixes = AccessoryPrefixLookups();
            int maxPrice = Item.buyPrice(gold: 50);
            //Main.NewText("Comparison Value: " + maxPrice, Colors.CoinPlatinum);
            for (int i = 3; i < Player.SupportedSlotsAccs + 3; i++)
            {
                if (player.IsAValidEquipmentSlotForIteration(i) && !player.armor[i].IsAir && player.armor[i].accessory)
                {
                    var testItem = player.armor[i].Clone();
                    int prefix = testItem.prefix;
                    testItem.SetDefaults(testItem.type);
                    //Main.NewText(testItem.Name + ": " + testItem.value * 1.5f, (testItem.value * 1.5f > maxPrice) ? Color.Red : Color.Cyan);
                    if (testItem.value * 1.5f > maxPrice)
                    {
                        continue;
                    }
                    bool add = true;

                    foreach (var p in searchPrefixes)
                    {
                        if (!testItem.Prefix(p))
                        {
                            add = false;
                            break;
                        }
                    }

                    if (add)
                    {
                        testItem.Prefix(prefix);
                        selectable.Add(testItem);
                    }
                }
            }
            if (selectable.Count > 0)
            {
                var item = Main.rand.Next(selectable);
                searchPrefixes.Remove(item.prefix);
                item.SetDefaults(item.type);
                item.Prefix(Main.rand.Next(searchPrefixes));
                return item;
            }
            return null;
        }
        public List<int> AccessoryPrefixLookups()
        {
            return new List<int>() { PrefixID.Menacing, PrefixID.Warding, PrefixID.Lucky, };
        }
        public Item GetBannerItem()
        {
            var potentialBanners = GetPotentialBannerItems();
            if (potentialBanners.Count >= 1)
            {
                var result = new Item();
                result.SetDefaults(Main.rand.Next(potentialBanners));
                return result;
            }
            return null;
        }
        public List<int> GetPotentialBannerItems()
        {
            var potentialBanners = new List<int>();
            for (int npcID = 0; npcID < NPCLoader.NPCCount; npcID++)
            {
                int bannerID = Item.NPCtoBanner(npcID);
                if (bannerID > 0 && !NPCID.Sets.PositiveNPCTypesExcludedFromDeathTally[npcID] && !NPCID.Sets.BelongsToInvasionOldOnesArmy[npcID] &&
                    NPC.killCount[bannerID] >= ItemID.Sets.KillsToBanner[Item.BannerToItem(bannerID)])
                {
                    potentialBanners.Add(Item.BannerToItem(bannerID));
                }
            }
            return potentialBanners;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = (int)Math.Clamp(damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Link",
                "Buddy",
                "Dobby",
                "Winky",
                "Hermey",
                "Altmer",
                "Summerset",
                "Calcelmo",
                "Ancano",
                "Nurelion",
                "Vingalmo",
                "Bosmer",
                "Faendal",
                "Malborn",
                "Niruin",
                "Enthir",
                "Dunmer",
                "Araena",
                "Ienith",
                "Brand-Shei",
                "Telvanni",
                "Erandur",
                "Neloth",
                "Gelebor",
                "Vyrthur",
            };
        }

        public override bool CheckActive() => false;

        public override bool CanChat() => true;

        public override string GetChat()
        {
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.SkyMerchant.");

            chat.Add("Basic.0");
            chat.Add("Basic.1");
            chat.Add("Basic.2");
            chat.Add("Basic.3", new { WorldName = Main.worldName, });

            if (!Main.dayTime)
            {
                chat.Add("Night");
                if (Main.bloodMoon)
                {
                    chat.Add("BloodMoon");
                }
                if (GlimmerBiomeManager.EventActive)
                {
                    chat.Add("Glimmer");
                }
            }
            if (Main.eclipse)
            {
                chat.Add("Eclipse");
            }
            if (Main.IsItStorming)
            {
                chat.Add("Thunderstorm");
            }
            if (Main.LocalPlayer.ZoneGraveyard)
            {
                chat.Add("Graveyard");
            }

            if (NPC.AnyNPCs(NPCID.Merchant))
                chat.Add("Merchant", new { Merchant = NPC.GetFirstNPCNameOrNull(NPCID.Merchant) });
            if (NPC.AnyNPCs(NPCID.TravellingMerchant))
                chat.Add("TravellingMerchant", new { TravellingMerchant = NPC.GetFirstNPCNameOrNull(NPCID.TravellingMerchant) });
            if (NPC.AnyNPCs(NPCID.Pirate))
                chat.Add("Pirate", new { Pirate = NPC.GetFirstNPCNameOrNull(NPCID.Pirate) });
            if (NPC.AnyNPCs(NPCID.Steampunker))
                chat.Add("Steampunker", new { Steampunker = NPC.GetFirstNPCNameOrNull(NPCID.Steampunker) });
            if (NPC.AnyNPCs(NPCID.Demolitionist))
                chat.Add("Demolitionist", new { Demolitionist = NPC.GetFirstNPCNameOrNull(NPCID.Demolitionist) });

            return chat.Get();
        }

        public override bool PreAI()
        {
            return currentAction == 7;
        }
        public override void PostAI()
        {
            bool offscreen = IsOffscreen();
            if (NPC.life < 80 && !NPC.dontTakeDamage)
            {
                if (currentAction == 7)
                    currentAction = -4;
                else
                {
                    currentAction = -3;
                }
                NPC.ai[0] = 0f;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                NPC.dontTakeDamage = true;
                if (NPC.velocity.X <= 0)
                {
                    NPC.direction = -1;
                    NPC.spriteDirection = NPC.direction;
                }
                else
                {
                    NPC.direction = 1;
                    NPC.spriteDirection = NPC.direction;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(Aequus.GetSound("slideWhistle", 0.5f), NPC.Center);
                }
            }

            if (currentAction == -4)
            {
                if ((int)NPC.ai[0] == 0)
                {
                    NPC.ai[0]++;
                    NPC.velocity.Y = -6f;
                }
                else
                {
                    NPC.velocity.Y += 0.3f;
                }
                if (offscreen)
                {
                    NPC.active = false;
                    NPC.netSkip = -1;
                    NPC.life = 0;
                }
                return;
            }
            else if (currentAction == -3)
            {
                NPC.velocity.Y -= 0.6f;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                if (offscreen)
                {
                    NPC.active = false;
                    NPC.netSkip = -1;
                    NPC.life = 0;
                }
                return;
            }
            if (!init)
            {
                init = true;
                if (Helper.CheckForSolidGroundBelow(NPC.Center.ToTileCoordinates(), 40, out var _))
                {
                    bool notInTown = true;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && NPC.Distance(Main.npc[i].Center) < 400f)
                        {
                            SetTownNPCState();
                            notInTown = false;
                            break;
                        }
                    }
                    if (notInTown)
                        SetBalloonState();
                }
                else
                {
                    SetBalloonState();
                }
            }
            if (!IsActive && offscreen)
            {
                NPC.active = false;
                NPC.netSkip = -1;
                NPC.life = 0;
                return;
            }
            if (NPC.position.X <= 240f || NPC.position.X + NPC.width > Main.maxTilesX * 16f - 240f
                || currentAction == 7 && offscreen && Main.rand.NextBool(1500))
            {
                NPC.active = false;
                NPC.netUpdate = true;
                //AirHunterWorldData.SpawnMerchant(NPC.whoAmI);
                return;
            }

            if (currentAction == -1)
                SetBalloonState();
            if (currentAction == -2)
            {
                NPC.noGravity = true;
                if (offscreen)
                    NPC.noTileCollide = true;
                else if (NPC.noTileCollide && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.noTileCollide = false;
                }
                bool canSwitchDirection = true;
                if (NPC.position.Y > 3600f)
                {
                    currentAction = -3;
                    NPC.netUpdate = true;
                }
                else if (NPC.position.Y > 3000f)
                {
                    NPC.velocity.Y -= 0.0125f;
                }
                else if (NPC.position.Y < 1600)
                {
                    NPC.velocity.Y += 0.0125f;
                }
                else
                {
                    if (NPC.velocity.Y.Abs() > 3f)
                        NPC.velocity.Y *= 0.99f;
                    else
                    {
                        NPC.velocity.Y += Main.rand.NextFloat(-0.005f, 0.005f) + NPC.velocity.Y * 0.0025f;
                    }
                    bool foundStoppingSpot = false;
                    if (IsActive)
                    {
                        if (!NPC.noTileCollide)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].active && !Main.player[i].dead && (NPC.Center - Main.player[i].Center).Length() < 300f)
                                {
                                    NPC.velocity.X *= 0.94f;
                                    if (NPC.position.Y < Main.player[i].position.Y)
                                    {
                                        NPC.velocity.Y += 0.05f;
                                    }
                                    else if (NPC.position.Y > Main.player[i].position.Y + 40f)
                                    {
                                        NPC.velocity.Y -= 0.05f;
                                    }
                                    else
                                    {
                                        NPC.velocity.Y *= 0.94f;
                                    }
                                    if (NPC.velocity.Y.Abs() > 2f)
                                    {
                                        NPC.velocity.Y *= 0.9f;
                                    }
                                    foundStoppingSpot = true;
                                    break;
                                }
                            }
                        }
                        if (NPC.ai[0] <= 0f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && (NPC.Center - Main.npc[i].Center).Length() < 800f)
                                {
                                    if (offscreen)
                                    {
                                        NPC.position.X = Main.npc[i].position.X + (Main.npc[i].width - NPC.width);
                                        NPC.position.Y = Main.npc[i].position.Y + (Main.npc[i].height - NPC.height);
                                        SetTownNPCState();
                                    }
                                    else if (!NPC.noTileCollide)
                                    {
                                        foundStoppingSpot = true;
                                        NPC.velocity *= 0.975f;
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            NPC.ai[0]--;
                        }
                    }
                    if (!foundStoppingSpot)
                    {
                        float windSpeed = Math.Max(Main.windSpeedCurrent.Abs() * 3f, 1.5f) * Math.Sign(Main.windSpeedCurrent);
                        if (windSpeed < 0f)
                        {
                            if (NPC.velocity.X > windSpeed)
                                NPC.velocity.X -= 0.025f;
                        }
                        else
                        {
                            if (NPC.velocity.X < windSpeed)
                                NPC.velocity.X += 0.025f;
                        }
                    }
                    else
                    {
                        canSwitchDirection = false;
                    }
                }

                if (canSwitchDirection)
                {
                    if (NPC.spriteDirection == oldSpriteDirection)
                    {
                        if (NPC.velocity.X <= 0)
                        {
                            NPC.direction = -1;
                            NPC.spriteDirection = NPC.direction;
                        }
                        else
                        {
                            NPC.direction = 1;
                            NPC.spriteDirection = NPC.direction;
                        }
                    }
                }
            }
        }
        private void SetBalloonState()
        {
            currentAction = -2;
            if (NPC.velocity.X <= 0)
                NPC.spriteDirection = -1;
            else
            {
                NPC.spriteDirection = 1;
            }
            oldSpriteDirection = NPC.spriteDirection;
            //NPC.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            NPC.netUpdate = true;
            NPC.ai[0] = 1000f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.localAI[0] = 0f;
            NPC.localAI[1] = 0f;
            NPC.localAI[2] = 0f;
            NPC.localAI[3] = 0f;
        }
        private void SetTownNPCState()
        {
            currentAction = 7;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.netUpdate = true;
            NPC.velocity.X = 0f;
            NPC.velocity.Y = 0f;
            NPC.ai[0] = 0f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.localAI[0] = 0f;
            NPC.localAI[1] = 0f;
            NPC.localAI[2] = 0f;
            NPC.localAI[3] = 0f;
        }
        private bool IsOffscreen()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && (player.Center - NPC.Center).Length() < 1250f)
                    return false;
            }
            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float val = 0f;
            if (spawnInfo.Player.ZoneSkyHeight && !NPC.AnyNPCs(Type))
            {
                val += 0.01f;
                if (spawnInfo.Player.townNPCs >= 2f)
                {
                    val += 0.2f;
                }
                if (spawnInfo.Player.HeldItemFixed()?.ModItem is Pumpinator)
                {
                    val *= 3f;
                }
            }
            return val;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy && currentAction != -2)
            {
                NPC.velocity.X = -1f;
                SetBalloonState();
            }
            if (currentAction == -4)
            {
                DrawFlee(spriteBatch, screenPos, drawColor);
                return false;
            }
            if (currentAction == 7)
            {
                return true;
            }

            DrawBalloon(spriteBatch, screenPos, drawColor);
            return false;
        }
        public void DrawBalloon(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = ModContent.Request<Texture2D>($"{Texture}Basket", AssetRequestMode.ImmediateLoad).Value;
            int frameX = -1;
            DrawBalloon_UpdateBasketFrame(ref frameX);
            if (frameX == -1)
                frameX = basketFrame / 18;
            if (balloonColor == 0)
                balloonColor = Main.rand.Next(5) + 1;
            var frame = new Rectangle(texture.Width / 4 * frameX, texture.Height / 18 * (basketFrame % 18), texture.Width / 4, texture.Height / 18);
            spriteBatch.Draw(texture, NPC.Center - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);

            float yOff = frame.Height / 2f;
            texture = ModContent.Request<Texture2D>($"{Texture}Balloon", AssetRequestMode.ImmediateLoad).Value;
            frame = new Rectangle(0, texture.Height / BalloonFrames * (balloonColor - 1), texture.Width, texture.Height / BalloonFrames);
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, -yOff + 4f), frame, NPC.GetNPCColorTintedByBuffs(drawColor), 0f, new Vector2(frame.Width / 2f, frame.Height), 1f, SpriteEffects.None, 0f);
        }
        public void DrawBalloon_UpdateBasketFrame(ref int frameX)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
            {
                basketFrameCounter++;
                if (basketFrameCounter > 4)
                {
                    basketFrameCounter = 0;
                    if (oldSpriteDirection == -1)
                    {
                        if (basketFrame < 5 || basketFrame > 23)
                            basketFrame = 5;
                        else
                        {
                            basketFrame++;
                        }
                        if (basketFrame > 23)
                        {
                            oldSpriteDirection = NPC.spriteDirection;
                            basketFrame = 37;
                        }
                    }
                    else
                    {
                        basketFrame++;
                        if (basketFrame < 41)
                            basketFrame = 41;
                        if (basketFrame > 59)
                        {
                            oldSpriteDirection = NPC.spriteDirection;
                            basketFrame = 1;
                        }
                    }
                }
            }
            else
            {
                if (NPC.spriteDirection == 1)
                {
                    if (basketFrame < 37)
                    {
                        basketFrame = 37;
                        frameX = basketFrame / 18;
                    }
                    basketFrameCounter++;
                    if (basketFrameCounter > 20)
                    {
                        basketFrameCounter = 0;
                        basketFrame++;
                        if (basketFrame > 40)
                            basketFrame = 37;
                    }
                }
                else
                {
                    if (basketFrame < 1)
                    {
                        basketFrame = 1;
                        frameX = 0;
                    }
                    basketFrameCounter++;
                    if (basketFrameCounter > 20)
                    {
                        basketFrameCounter = 0;
                        basketFrame++;
                        if (basketFrame > 4)
                            basketFrame = 1;
                    }
                }
            }
        }
        public void DrawFlee(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = ModContent.Request<Texture2D>($"{Texture}Flee", AssetRequestMode.ImmediateLoad).Value;
            var frame = GetFleeFrame(texture);
            var effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                effects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), 0f, frame.Size() / 2f, 1f, effects, 0f);
        }
        public Rectangle GetFleeFrame(Texture2D fleeTexture)
        {
            return new Rectangle(0, fleeTexture.Height / 2 * ((int)(Main.GlobalTimeWrappedHourly * 10f) % 2), fleeTexture.Width, fleeTexture.Height / 2);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(currentAction);
            PacketSystem.WriteNullableItem(shopBanner, writer, writeStack: true);
            PacketSystem.WriteNullableItem(shopAccessory, writer, writeStack: true);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentAction = reader.ReadInt32();
            shopBanner = PacketSystem.ReadNullableItem(reader, readStack: true);
            shopAccessory = PacketSystem.ReadNullableItem(reader, readStack: true);
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            CapDamage(ref damage, ref crit);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CapDamage(ref damage, ref crit);
        }
        public void CapDamage(ref int damage, ref bool crit)
        {
            crit = false;
            if (damage > 79)
                damage = 79;
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

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.PoisonedKnife;
            attackDelay = 10;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override bool CheckDead()
        {
            if (currentAction == -4 || currentAction == -3)
                return true;
            NPC.ai[0] = 0f;
            if (currentAction == 7)
                currentAction = -4;
            else
            {
                currentAction = -3;
            }
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.life = NPC.lifeMax;
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(Aequus.GetSound("slideWhistle", 0.5f), NPC.Center);
            }
            if (NPC.velocity.X <= 0)
            {
                NPC.direction = -1;
                NPC.spriteDirection = NPC.direction;
            }
            else
            {
                NPC.direction = 1;
                NPC.spriteDirection = NPC.direction;
            }
            NPC.dontTakeDamage = true;
            return false;
        }
    }
}