﻿using Aequus;
using Aequus.Common;
using Aequus.Common.NPCs;
using Aequus.Common.Utilities;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.UI.Renaming;
using Aequus.CrossMod;
using Aequus.Items;
using Aequus.Items.Equipment.Accessories.Combat.Dodge.FlashwayNecklace;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Dyes.Ancient;
using Aequus.Items.Misc.Spawners;
using Aequus.Items.Tools;
using Aequus.Items.Tools.FishingPoles;
using Aequus.Tiles.Paintings.Canvas2x2;
using Aequus.Tiles.Paintings.Canvas3x2;
using Aequus.Tiles.Paintings.Canvas3x3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Town.SkyMerchantNPC {
    [AutoloadHead()]
    public class SkyMerchant : ModNPC {
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

        public override List<string> SetNPCNameList() {
            return new() {
                "Link",
                "Buddy",
                "Dobby",
                "Hermey",
                "Calcelmo",
                "Ancano",
                "Nurelion",
                "Vingalmo",
                "Faendal",
                "Malborn",
                "Niruin",
                "Enthir",
                "Araena",
                "Ienith",
                "Brand-Shei",
                "Erandur",
                "Neloth",
                "Gelebor",
                "Vyrthur",
            };
        }

        internal void SetupShopQuotes(Mod shopQuotes) {
            string skyrimRocksKey = ShopQuotesMod.Key + "SkyMerchant.SkyrimRocks";
            shopQuotes.Call("AddNPC", Mod, Type);
            shopQuotes.Call("SetColor", Type, Color.DarkOliveGreen * 1.75f);
            shopQuotes.Call("SetQuote", Type, ModContent.ItemType<SkyrimRock1>(), skyrimRocksKey);
            shopQuotes.Call("SetQuote", Type, ModContent.ItemType<SkyrimRock2>(), skyrimRocksKey);
            shopQuotes.Call("SetQuote", Type, ModContent.ItemType<SkyrimRock3>(), skyrimRocksKey);
            shopQuotes.Call("AddDefaultText", Type,
                (int i) => {
                    int bannerID = Helper.ItemToBanner(i);
                    if (bannerID != 0) {
                        return Language.GetTextValue("Mods.Aequus.ShopQuote.SkyMerchant.Banners");
                    }
                    return null;
                });
            shopQuotes.Call("AddDefaultText", Type,
                (int i) => {
                    if (i == ModContent.ItemType<FlashwayNecklace>())
                        return null;
                    return ContentSamples.ItemsByType[i].accessory ? Language.GetTextValue("Mods.Aequus.ShopQuote.SkyMerchant.EquippedAcc") : null;
                });
        }

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 50;
            NPCID.Sets.HatOffsetY[Type] = 0;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.SpawnsWithCustomName[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
                Velocity = -1f,
                Direction = -1
            });
        }

        public override void SetDefaults() {
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
            NPC.rarity = 2;
            AnimationType = NPCID.SkeletonMerchant;
            currentAction = 7;

            init = false;
            setupShop = false;
            shopBanner = null;
            shopAccessory = null;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.SkyBiome)
                .AddSpawn(BestiaryBuilder.WindyDayEvent);
        }

        public override void SetChatButtons(ref string button, ref string button2) {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = TextHelper.GetTextValue("Chat.SkyMerchant.RenameChatButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
            if (firstButton) {
                shopName = "Shop";
                return;
            }

            Main.playerInventory = true;
            Main.npcChatText = "";
            Aequus.UserInterface.SetState(new RenameItemUIState());
        }

        #region Shop
        #region Instanced Shop
        private void SetupShopCache(Player player) {
            shopBanner = GetBannerItem();
            shopAccessory = GetAccessoryItem(player);
        }
        private Item GetAccessoryItem(Player player) {
            var selectable = new List<Item>();
            var searchPrefixes = AccessoryPrefixLookups();
            int maxPrice = Item.buyPrice(gold: 50);
            //Main.NewText("Comparison Value: " + maxPrice, Colors.CoinPlatinum);
            for (int i = 3; i < Player.SupportedSlotsAccs + 3; i++) {
                if (player.IsItemSlotUnlockedAndUsable(i) && !player.armor[i].IsAir && player.armor[i].accessory) {
                    var testItem = player.armor[i].Clone();
                    int prefix = testItem.prefix;
                    testItem.SetDefaults(testItem.type);
                    //Main.NewText(testItem.Name + ": " + testItem.value * 1.5f, (testItem.value * 1.5f > maxPrice) ? Color.Red : Color.Cyan);
                    if (testItem.value * 1.5f > maxPrice) {
                        continue;
                    }
                    bool add = true;

                    foreach (var p in searchPrefixes) {
                        if (!testItem.Prefix(p)) {
                            add = false;
                            break;
                        }
                    }

                    if (add) {
                        testItem.Prefix(prefix);
                        selectable.Add(testItem);
                    }
                }
            }
            if (selectable.Count > 0) {
                var item = Main.rand.Next(selectable);
                searchPrefixes.Remove(item.prefix);
                item.SetDefaults(item.type);
                item.Prefix(Main.rand.Next(searchPrefixes));
                return item;
            }
            return null;
        }
        private List<int> AccessoryPrefixLookups() {
            return new List<int>() { PrefixID.Menacing, PrefixID.Warding, PrefixID.Lucky, };
        }
        private Item GetBannerItem() {
            var potentialBanners = GetPotentialBannerItems();
            if (potentialBanners.Count >= 1) {
                var result = new Item();
                result.SetDefaults(Main.rand.Next(potentialBanners));
                return result;
            }
            return null;
        }
        private List<int> GetPotentialBannerItems() {
            var potentialBanners = new List<int>();
            for (int npcID = 0; npcID < NPCLoader.NPCCount; npcID++) {
                int bannerID = Item.NPCtoBanner(npcID);
                if (bannerID > 0 && !NPCID.Sets.PositiveNPCTypesExcludedFromDeathTally[npcID] && !NPCID.Sets.BelongsToInvasionOldOnesArmy[npcID] &&
                    NPC.killCount[bannerID] >= ItemID.Sets.KillsToBanner[Item.BannerToItem(bannerID)]) {
                    potentialBanners.Add(Item.BannerToItem(bannerID));
                }
            }
            return potentialBanners;
        }
        private void ModifyActiveShop_InstancedItems(SkyMerchant merchant, string shopName, Item[] items) {
            if (merchant.shopAccessory != null) {
                int slot = Helper.FindNextShopSlot(items);
                items[slot] = merchant.shopAccessory.Clone();
                items[slot].shopCustomPrice = (int)(items[slot].value * 1.5f);
                items[slot].shopCustomPrice /= 100;
                items[slot].shopCustomPrice *= 100;
                items[slot].shopCustomPrice = Math.Max(items[slot].shopCustomPrice.Value, Item.buyPrice(gold: 5));
            }
            if (merchant.shopBanner != null) {
                int slot = Helper.FindNextShopSlot(items);
                items[slot] = merchant.shopBanner.Clone();
                items[slot].shopCustomPrice = items[slot].value * 10;
            }
        }
        #endregion

        public override void AddShops() {
            var dryadCondition = Condition.NpcIsPresent(NPCID.Dryad);
            new NPCShop(Type)
                .Add<Items.Equipment.Mounts.HotAirBalloon.BalloonKit>()
                .Add<Pumpinator>()
                .Add<Nimrod>(Condition.InRain)
                .Add<FlashwayNecklace>(Condition.DownedEyeOfCthulhu)

                .AddWithCustomValue(ItemID.Starfury, Item.buyPrice(gold: 10), dryadCondition, Condition.MoonPhases04, Condition.NotRemixWorld, Condition.NotForTheWorthy)
                .AddWithCustomValue(ItemID.CreativeWings, Item.buyPrice(gold: 35), dryadCondition, Condition.MoonPhaseFull, Condition.NotRemixWorld, Condition.NotForTheWorthy)
                .AddWithCustomValue(ItemID.CelestialMagnet, Item.buyPrice(gold: 10), dryadCondition, Condition.MoonPhases15, Condition.NotRemixWorld, Condition.NotForTheWorthy)
                .AddWithCustomValue(ItemID.ShinyRedBalloon, Item.buyPrice(gold: 10), dryadCondition, Condition.MoonPhases26, Condition.NotRemixWorld, Condition.NotForTheWorthy)
                .AddWithCustomValue(ItemID.LuckyHorseshoe, Item.buyPrice(gold: 10), dryadCondition, Condition.MoonPhases37, Condition.NotRemixWorld, Condition.NotForTheWorthy)

                .Add<BongBongPainting>(Condition.MoonPhaseFull)
                .Add<CatalystPainting>(Condition.DownedMoonLord)
                .Add<YinYangPainting>(Condition.DownedEmpressOfLight, Condition.DownedCultist)
                .Add<YinPainting>(AequusConditions.DownedDemonSiege)
                .Add<YangPainting>(Condition.DownedQueenSlime)
                .Add<SkyrimRock1>(Condition.DownedEyeOfCthulhu)
                .Add<SkyrimRock2>(Condition.DownedEowOrBoc)
                .Add<SkyrimRock3>(Condition.DownedSkeletron)

                .Add<AncientBreakdownDye>(Condition.MoonPhaseFull)
                .Add<CensorDye>(Condition.MoonPhaseWaningGibbous)
                .Add<OutlineDye>(Condition.MoonPhaseThirdQuarter)
                .Add<ScrollDye>(Condition.MoonPhaseWaningCrescent)
                .Add<SimplifiedDye>(Condition.MoonPhaseNew)
                .Add<AncientHueshiftDye>(Condition.MoonPhaseWaxingCrescent)

                .Add(ItemID.SkyMill, Condition.NotRemixWorld)
                .AddWithCustomValue(ItemID.Cloud, Item.buyPrice(copper: 3), Condition.NotRemixWorld)
                .AddWithCustomValue(ItemID.RainCloud, Item.buyPrice(copper: 3), Condition.InRain, Condition.NotRemixWorld)
                .Add<NameTag>()
                .Add<TornadoInABottle>(AequusConditions.DownedDustDevil)
                .Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items) {
            int talkNPC = Main.LocalPlayer.talkNPC;
            if (shopName != "Shop" || talkNPC == -1 || Main.npc[talkNPC].ModNPC is not SkyMerchant merchant) {
                return;
            }

            if (!merchant.setupShop) {
                merchant.SetupShopCache(Main.LocalPlayer);
                merchant.setupShop = true;
            }

            ModifyActiveShop_InstancedItems(merchant, shopName, items);
        }
        #endregion

        public override void HitEffect(NPC.HitInfo hit) {
            int dustAmount = Math.Clamp(hit.Damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        public override bool CheckActive() => true;

        public override bool CanChat() => true;

        public override string GetChat() {
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.SkyMerchant.");

            chat.Add("Basic.0");
            chat.Add("Basic.1");
            chat.Add("Basic.2");
            chat.Add("Basic.3", new { WorldName = Main.worldName, });

            if (!Main.dayTime) {
                chat.Add("Night");
                if (Main.bloodMoon) {
                    chat.Add("BloodMoon");
                }
                if (GlimmerZone.EventActive) {
                    chat.Add("Glimmer");
                }
            }
            if (Main.eclipse) {
                chat.Add("Eclipse");
            }
            if (Main.IsItStorming) {
                chat.Add("Thunderstorm");
            }
            if (Main.LocalPlayer.ZoneGraveyard) {
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

        public override bool PreAI() {
            return currentAction == 7;
        }

        private void SetBalloonState() {
            currentAction = -2;
            if (NPC.velocity.X <= 0)
                NPC.spriteDirection = -1;
            else {
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

        private void SetTownNPCState() {
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

        private bool IsOffscreen() {
            for (int i = 0; i < Main.maxPlayers; i++) {
                Player player = Main.player[i];
                if (player.active && (player.Center - NPC.Center).Length() < 1250f)
                    return false;
            }
            return true;
        }

        public override void PostAI() {
            bool offscreen = IsOffscreen();
            var tileCoordinates = NPC.Center.ToTileCoordinates();
            tileCoordinates.X += Math.Sign(NPC.velocity.X);

            if (!WorldGen.InWorld(tileCoordinates.X, tileCoordinates.Y) || (currentAction == 7 && offscreen && Main.rand.NextBool(1500))) {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (NPC.life < 80 && !NPC.dontTakeDamage) {
                if (currentAction == 7)
                    currentAction = -4;
                else {
                    currentAction = -3;
                }
                NPC.ai[0] = 0f;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                NPC.dontTakeDamage = true;
                if (NPC.velocity.X <= 0) {
                    NPC.direction = -1;
                    NPC.spriteDirection = NPC.direction;
                }
                else {
                    NPC.direction = 1;
                    NPC.spriteDirection = NPC.direction;
                }
                if (Main.netMode != NetmodeID.Server) {
                    SoundEngine.PlaySound(AequusSounds.slidewhistle with { Volume = 0.5f, }, NPC.Center);
                }
            }

            if (currentAction == -4) {
                if ((int)NPC.ai[0] == 0) {
                    NPC.ai[0]++;
                    NPC.velocity.Y = -6f;
                }
                else {
                    NPC.velocity.Y += 0.3f;
                }
                if (offscreen) {
                    NPC.active = false;
                    NPC.netSkip = -1;
                    NPC.life = 0;
                }
                return;
            }
            else if (currentAction == -3) {
                NPC.velocity.Y -= 0.6f;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                if (offscreen) {
                    NPC.active = false;
                    NPC.netSkip = -1;
                    NPC.life = 0;
                }
                return;
            }
            if (!init) {
                init = true;
                if (TileHelper.ScanDown(NPC.Center.ToTileCoordinates(), 40, out var _)) {
                    bool notInTown = true;
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && NPC.Distance(Main.npc[i].Center) < 400f) {
                            SetTownNPCState();
                            notInTown = false;
                            break;
                        }
                    }
                    if (notInTown)
                        SetBalloonState();
                }
                else {
                    SetBalloonState();
                }
            }
            if (!IsActive && offscreen) {
                NPC.active = false;
                NPC.netSkip = -1;
                NPC.life = 0;
                return;
            }

            if (currentAction == -1)
                SetBalloonState();
            if (currentAction == -2) {
                NPC.noGravity = true;
                bool canSwitchDirection = true;
                if (offscreen)
                    NPC.noTileCollide = true;
                else if (NPC.noTileCollide && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
                    NPC.noTileCollide = false;
                }

                bool foundStoppingSpot = false;
                if (IsActive) {
                    if (!NPC.noTileCollide) {
                        for (int i = 0; i < Main.maxPlayers; i++) {
                            if (Main.player[i].active && !Main.player[i].dead && (NPC.Center - Main.player[i].Center).Length() < 300f) {
                                NPC.velocity.X *= 0.94f;
                                if (NPC.position.Y < Main.player[i].position.Y) {
                                    NPC.velocity.Y += 0.05f;
                                }
                                else if (NPC.position.Y > Main.player[i].position.Y + 40f) {
                                    NPC.velocity.Y -= 0.05f;
                                }
                                else {
                                    NPC.velocity.Y *= 0.94f;
                                }
                                if (NPC.velocity.Y.Abs() > 2f) {
                                    NPC.velocity.Y *= 0.9f;
                                }
                                foundStoppingSpot = true;
                                break;
                            }
                        }
                    }
                    if (NPC.ai[0] <= 0f) {
                        for (int i = 0; i < Main.maxNPCs; i++) {
                            if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && (NPC.Center - Main.npc[i].Center).Length() < 800f) {
                                if (offscreen) {
                                    NPC.position.X = Main.npc[i].position.X + (Main.npc[i].width - NPC.width);
                                    NPC.position.Y = Main.npc[i].position.Y + (Main.npc[i].height - NPC.height);
                                    SetTownNPCState();
                                }
                                else if (!NPC.noTileCollide) {
                                    foundStoppingSpot = true;
                                    NPC.velocity *= 0.975f;
                                }
                                break;
                            }
                        }
                    }
                    else {
                        NPC.ai[0]--;
                    }
                }

                if (!foundStoppingSpot) {
                    float windSpeed = Math.Max(Main.windSpeedCurrent.Abs() * 3f, 1.5f) * Math.Sign(Main.windSpeedCurrent);
                    if (windSpeed < 0f) {
                        if (NPC.velocity.X > windSpeed)
                            NPC.velocity.X -= 0.025f;
                    }
                    else {
                        if (NPC.velocity.X < windSpeed)
                            NPC.velocity.X += 0.025f;
                    }

                    float maxHeight = 3000f;
                    float minHeight = 1600f;
                    float getOffBalloonHeight = 3600f;
                    if (Main.remixWorld) {
                        maxHeight = Main.maxTilesY * 16f;
                        minHeight = Main.UnderworldLayer * 16f;
                        getOffBalloonHeight = float.MaxValue;
                    }

                    bool roof = TileHelper.ScanUp(tileCoordinates, 10, out var roofTile, TileHelper.HasAnyLiquid, TileHelper.IsFullySolid);
                    bool floor = TileHelper.ScanDown(tileCoordinates, 10, out var floorTile, TileHelper.HasAnyLiquid, TileHelper.IsFullySolid);

                    // Choose which one is farther and go to it, trying to keep an equal distance
                    if (floor && roof) {
                        int averageTileY = (roofTile.Y + floorTile.Y) / 2;
                        if (tileCoordinates.Y < averageTileY) {
                            floor = false;
                        }
                        else {
                            roof = false;
                        }
                    }

                    // Adjust vertical if there's a roof or floor which is too close
                    if (floor) {
                        if (NPC.velocity.Y > 0f) {
                            NPC.velocity.Y *= 0.95f;
                        }
                        NPC.velocity.Y -= 0.01f;
                    }
                    else if (roof) {
                        if (NPC.velocity.Y < 0f) {
                            NPC.velocity.Y *= 0.95f;
                        }
                        NPC.velocity.Y += 0.01f;
                    }

                    if (NPC.position.Y > getOffBalloonHeight) {
                        currentAction = -3;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.position.Y > maxHeight) {
                        NPC.velocity.Y -= 0.0125f;
                    }
                    else if (NPC.position.Y < minHeight) {
                        NPC.velocity.Y += 0.0125f;
                    }
                    else {
                        if (NPC.velocity.Y.Abs() > 3f)
                            NPC.velocity.Y *= 0.99f;
                        else {
                            NPC.velocity.Y += Main.rand.NextFloat(-0.005f, 0.005f) + NPC.velocity.Y * 0.0025f;
                        }
                    }

                    if (canSwitchDirection) {
                        if (NPC.spriteDirection == oldSpriteDirection) {
                            if (NPC.velocity.X <= 0) {
                                NPC.direction = -1;
                                NPC.spriteDirection = NPC.direction;
                            }
                            else {
                                NPC.direction = 1;
                                NPC.spriteDirection = NPC.direction;
                            }
                        }
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) {
            if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].WallType > 0 || (Main.remixWorld ? spawnInfo.SpawnTileY < Main.UnderworldLayer + 50 : !Helper.ZoneSkyHeight(spawnInfo.SpawnTileY)) || NPC.AnyNPCs(Type) || TileHelper.ScanTilesSquare(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY, 4, TileHelper.HasAnyLiquid)) {
                return 0f;
            }

            float spawnRate = 0.015f;
            if (spawnInfo.Player.townNPCs >= 2f) {
                spawnRate += 0.2f;
            }
            if (spawnInfo.Player.HeldItemFixed()?.ModItem is Pumpinator) {
                spawnRate *= 6f;
            }
            return spawnRate;
        }

        #region Drawing
        private void DrawBalloon(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            var texture = AequusTextures.SkyMerchantBasket;
            int frameX = -1;
            DrawBalloon_UpdateBasketFrame(ref frameX);
            if (frameX == -1)
                frameX = basketFrame / 18;
            if (balloonColor == 0)
                balloonColor = Main.rand.Next(5) + 1;
            var frame = new Rectangle(texture.Width / 4 * frameX, texture.Height / 18 * (basketFrame % 18), texture.Width / 4, texture.Height / 18);
            spriteBatch.Draw(texture, NPC.Center - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);

            float yOff = frame.Height / 2f;
            texture = AequusTextures.SkyMerchantBalloon;
            frame = new Rectangle(0, texture.Height / BalloonFrames * (balloonColor - 1), texture.Width, texture.Height / BalloonFrames);
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, -yOff + 4f), frame, NPC.GetNPCColorTintedByBuffs(drawColor), 0f, new Vector2(frame.Width / 2f, frame.Height), 1f, SpriteEffects.None, 0f);
        }

        private void DrawBalloon_UpdateBasketFrame(ref int frameX) {
            if (NPC.spriteDirection != oldSpriteDirection) {
                basketFrameCounter++;
                if (basketFrameCounter > 4) {
                    basketFrameCounter = 0;
                    if (oldSpriteDirection == -1) {
                        if (basketFrame < 5 || basketFrame > 23)
                            basketFrame = 5;
                        else {
                            basketFrame++;
                        }
                        if (basketFrame > 23) {
                            oldSpriteDirection = NPC.spriteDirection;
                            basketFrame = 37;
                        }
                    }
                    else {
                        basketFrame++;
                        if (basketFrame < 41)
                            basketFrame = 41;
                        if (basketFrame > 59) {
                            oldSpriteDirection = NPC.spriteDirection;
                            basketFrame = 1;
                        }
                    }
                }
            }
            else {
                if (NPC.spriteDirection == 1) {
                    if (basketFrame < 37) {
                        basketFrame = 37;
                        frameX = basketFrame / 18;
                    }
                    basketFrameCounter++;
                    if (basketFrameCounter > 20) {
                        basketFrameCounter = 0;
                        basketFrame++;
                        if (basketFrame > 40)
                            basketFrame = 37;
                    }
                }
                else {
                    if (basketFrame < 1) {
                        basketFrame = 1;
                        frameX = 0;
                    }
                    basketFrameCounter++;
                    if (basketFrameCounter > 20) {
                        basketFrameCounter = 0;
                        basketFrame++;
                        if (basketFrame > 4)
                            basketFrame = 1;
                    }
                }
            }
        }

        private void DrawFlee(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            var texture = AequusTextures.SkyMerchantFlee;
            var frame = GetFleeFrame(texture);
            var effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                effects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), 0f, frame.Size() / 2f, 1f, effects, 0f);
        }

        private Rectangle GetFleeFrame(Texture2D fleeTexture) {
            return new Rectangle(0, fleeTexture.Height / 2 * ((int)(Main.GlobalTimeWrappedHourly * 10f) % 2), fleeTexture.Width, fleeTexture.Height / 2);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (NPC.IsABestiaryIconDummy && currentAction != -2) {
                NPC.velocity.X = -1f;
                SetBalloonState();
            }
            if (currentAction == -4) {
                DrawFlee(spriteBatch, screenPos, drawColor);
                return false;
            }
            if (currentAction == 7) {
                return true;
            }

            DrawBalloon(spriteBatch, screenPos, drawColor);
            return false;
        }
        #endregion

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(currentAction);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            currentAction = reader.ReadInt32();
        }

        public override bool CanGoToStatue(bool toKingStatue) {
            return toKingStatue;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
            cooldown = 12;
            randExtraCooldown = 20;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
            projType = ProjectileID.PoisonedKnife;
            attackDelay = 10;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override bool CheckDead() {
            if (currentAction == -4 || currentAction == -3)
                return true;
            NPC.ai[0] = 0f;
            if (currentAction == 7)
                currentAction = -4;
            else {
                currentAction = -3;
            }
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.life = NPC.lifeMax;
            if (Main.netMode != NetmodeID.Server) {
                SoundEngine.PlaySound(AequusSounds.slidewhistle, NPC.Center);
            }
            if (NPC.velocity.X <= 0) {
                NPC.direction = -1;
                NPC.spriteDirection = NPC.direction;
            }
            else {
                NPC.direction = 1;
                NPC.spriteDirection = NPC.direction;
            }
            NPC.dontTakeDamage = true;
            return false;
        }
    }
}