using Aequus.Common.Personalities;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Boss.Crabson;
using Aequus.Content.Boss.Crabson.Misc;
using Aequus.Content.CrossMod;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Town.ExporterNPC.Quest;
using Aequus.Content.Town.ExporterNPC.RerollSystem;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Accessories.Offense.Sentry;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee.DynaKnife;
using Aequus.Items.Weapons.Melee.Misc;
using Aequus.Items.Weapons.Ranged;
using Aequus.NPCs;
using Aequus.Tiles.CraftingStations;
using Aequus.Tiles.Furniture.Crab;
using Aequus.Tiles.Furniture.Jeweled;
using Aequus.Unused.Items.SlotMachines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Town.ExporterNPC {
    [AutoloadHead()]
    public class Exporter : ModNPC, IModifyShoppingSettings {
        public override List<string> SetNPCNameList() {
            return new() {
                "Larry",
                "Reaver",
                "Barnacle",
                "Eugene",
                "Robster",
                "Catcher",
            };
        }

        internal void SetupShopQuotes(Mod shopQuotes) {
            shopQuotes.Call("AddNPC", Mod, Type);
            shopQuotes.Call("SetColor", Type, Color.Orange * 1.2f);
            shopQuotes.Call("SetQuote", Type, ModContent.ItemType<FoolsGoldRing>(),
                () => ShopQuotesMod.GetTextValue($"Exporter.FoolsGoldRing_{(Main.LocalPlayer.Male ? "Male" : "Female")}"));
            shopQuotes.Call("SetQuote", Type, ModContent.ItemType<RichMansMonocle>(),
                () => {
                    string s = Language.GetTextValue(ShopQuotesMod.GetTextValue("Exporter.RichMansMonocle"));
                    string taxCollector = NPC.GetFirstNPCNameOrNull(NPCID.TaxCollector);
                    if (!string.IsNullOrEmpty(taxCollector)) {
                        s += Language.GetTextValueWith(ShopQuotesMod.GetTextValue("Exporter.RichMansMonocle_TaxCollector"),
                            new { TaxCollector = taxCollector, });
                    }
                    return s;
                });
        }

        public override void SetStaticDefaults() {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 3; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
            NPCID.Sets.AttackTime[NPC.type] = 10;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
                Velocity = 1f,
                Direction = -1,
            });

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData() {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Lovestruck,
                }
            });

            NPC.Happiness
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
                .SetBiomeAffection<CrabCreviceBiome>(AffectionLevel.Hate)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Love)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Like)
                .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Golfer, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);

            NPCHappiness.Get(NPCID.Dryad).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.Angler).SetNPCAffection(Type, AffectionLevel.Love);
            NPCHappiness.Get(NPCID.Pirate).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.ArmsDealer).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.TaxCollector).SetNPCAffection(Type, AffectionLevel.Hate);

            ExporterQuestSystem.NPCTypesNoSpawns.Add(Type);
        }

        public override void SetDefaults() {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 50;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override void HitEffect(NPC.HitInfo hit) {
            int dustAmount = Math.Clamp(hit.Damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_Slime, newColor: new Color(200, 200, 200, 100));
            }
            if (NPC.life <= 0) {
                for (int i = -1; i <= 1; i += 2) {
                    NPC.DeathGore("Exporter_5", new Vector2(NPC.width / 2f * i, NPC.height / 2f));
                    NPC.DeathGore("Exporter_5", new Vector2(NPC.width / 4f * i, NPC.height / 2f));
                    NPC.DeathGore("Exporter_4", new Vector2(NPC.width / 2f * i, 0f));
                }

                NPC.DeathGore("Exporter_3");
                NPC.DeathGore("Exporter_2");
                NPC.DeathGore("Exporter_1");
                NPC.DeathGore("Exporter_0", new Vector2(0f, -NPC.height / 2f));
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry)
                .AddSpawn(BestiaryBuilder.OceanBiome);
        }

        public override void AddShops() {
            NPCShop shop = new(Type);
            shop.Add<Mallet>(Condition.InGraveyard)
                .Add<GrandReward>()
                .Add<RichMansMonocle>()
                .Add<FishyFins>()
                .Add<SkeletonKey>(Condition.Hardmode)
                .Add<Dynaknife>()

                .Add(ItemID.BreathingReed, Condition.MoonPhaseFull)
                .Add(ItemID.Flipper, Condition.MoonPhaseWaningGibbous)
                .Add(ItemID.Trident, Condition.MoonPhaseThirdQuarter)
                .Add(ItemID.FloatingTube, Condition.MoonPhaseWaningCrescent)
                .Add(ItemID.WaterWalkingBoots, Condition.MoonPhaseNew)
                .Add<SentrySquid>(Condition.MoonPhaseNew)
                .Add<DavyJonesAnchor>(Condition.MoonPhaseWaxingCrescent)
                .Add<StarPhish>(Condition.MoonPhaseFirstQuarter)
                .Add<ArmFloaties>(Condition.MoonPhaseWaxingGibbous)

                .AddWithCustomValue(ItemID.TatteredCloth, Item.buyPrice(silver: 50), Condition.DownedGoblinArmy)
                .AddWithCustomValue(ItemID.PirateMap, Item.buyPrice(gold: 5), Condition.DownedPirates)
                .AddWithCustomValue(ItemID.SnowGlobe, Item.buyPrice(gold: 5), Condition.DownedFrostLegion)

                .AddWithCustomValue(ItemID.WaterChest, Item.buyPrice(gold: 1), Condition.TimeDay)
                .AddWithCustomValue(ItemID.GoldChest, Item.buyPrice(gold: 1), Condition.TimeNight)

                .Add<RecyclingMachine>()
                .Add<CrabClock>()
                .Add<JeweledChalice>()
                .Add<JeweledCandelabra>()
                .Add<HypnoticPearl>()
                .Register();
        }

        public override void AI() {
            NPC.breath = 200;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) {
            return AequusWorld.downedCrabson;
        }

        public override ITownNPCProfile TownNPCProfile() {
            return base.TownNPCProfile();
        }

        public override string GetChat() {
            var player = Main.LocalPlayer;
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.Exporter.");

            string gender = player.GenderString();
            chat.Add("Basic.0");
            chat.Add("Basic.1");
            chat.Add("Basic.2");
            chat.Add($"Basic.{gender}", new { PlayerName = Main.LocalPlayer.name, });

            if (!Main.dayTime) {
                chat.Add("Night.0");
                chat.Add($"Night.{gender}");
                if (Main.bloodMoon) {
                    chat.Add("BloodMoon.0");
                    chat.Add("BloodMoon.1");

                    if (NPC.killCount[NPCID.WanderingEye] > 0) {
                        chat.Add("BloodMoon.WanderingEyeFish");
                    }
                }
                if (GlimmerBiomeManager.EventActive) {
                    chat.Add("Glimmer");
                }
            }

            if (Main.IsItAHappyWindyDay) {
                chat.Add("WindyDay");
            }

            if (Main.raining) {
                chat.Add("Rain");
            }
            if (Main.IsItStorming) {
                chat.Add("Thunderstorm");
            }
            if (BirthdayParty.PartyIsUp) {
                chat.Add("Party");
            }

            if (player.ZoneBeach) {
                chat.Add("Ocean");
            }
            if (player.Aequus().ZoneCrabCrevice) {
                chat.Add("CrabCrevice");
            }
            if (player.ZoneGraveyard) {
                chat.Add("Graveyard");
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

            if (Main.rand.NextBool(4) || NPC.AnyNPCs(ModContent.NPCType<Crabson>())) {
                chat.Add("Crabson");
            }

            if (Main.invasionType == InvasionID.PirateInvasion || NPC.downedPirates) {
                chat.Add("PirateInvasion");
            }
            if (NPC.downedFishron) {
                chat.Add("PirateInvasion");
            }
            if (NPC.downedMoonlord) {
                chat.Add("MoonLord");
            }

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2) {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = TextHelper.GetTextValue("Chat.Exporter.SlotMachineButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
            if (firstButton) {
                shopName = "Shop";
            }
            else {
                Main.playerInventory = true;
                Main.npcChatText = "";
                Aequus.UserInterface.SetState(new RerollUI());
            }
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

        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset) {
            int itemType = ItemID.DyeTradersScimitar;
            Main.instance.LoadItem(itemType);
            item = TextureAssets.Item[itemType].Value;
            itemSize = 40;
            scale = 0.5f;
            offset = new Vector2(0f, 0f);
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight) {
            itemWidth = 30;
            itemHeight = 30;
        }

        public void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
            string gender = player.GenderString();
            Helper.ReplaceText(ref settings.HappinessReport, "[NeutralQuote]", TextHelper.GetTextValue($"TownNPCMood.Exporter.Content_{gender}"));
            Helper.ReplaceText(ref settings.HappinessReport, "[HomelessQuote]", TextHelper.GetTextValue($"TownNPCMood.Exporter.NoHome_{gender}"));
            Helper.ReplaceText(ref settings.HappinessReport, "[CrowdedQuote1]", TextHelper.GetTextValue($"TownNPCMood.Exporter.DislikeCrowded_{gender}"));
            Helper.ReplaceText(ref settings.HappinessReport, "[DislikeBiomeQuote]", TextHelper.GetTextValue($"TownNPCMood.Exporter.DislikeBiome_{(player.ZoneDesert ? "Desert" : "Snow")}"));
            Helper.ReplaceTextWithStringArgs(ref settings.HappinessReport, "[HateBiomeQuote]|",
                $"Mods.Aequus.TownNPCMood.Exporter.HateBiome_{(player.Aequus().ZoneCrabCrevice ? "CrabCrevice" : "Evils")}", (s) => new { BiomeName = s[1], });
        }

        #region Quests
        [Obsolete("Exporter quests were removed.")]
        public static void CheckQuest() {
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
                if (GetQuestItem(Main.LocalPlayer.inventory[i], out var info)) {
                    info.SpawnLoot(Main.LocalPlayer, i);
                    info.OnQuestCompleted(Main.LocalPlayer, i);
                    OnQuestCompleted(Main.LocalPlayer, i);
                    return;
                }
            }
            OnQuestFailed(Main.LocalPlayer);
        }
        public static bool GetQuestItem(Item item, out IThieveryItemInfo info) {
            info = null;
            return !item.IsAir && ExporterQuestSystem.QuestItems.TryGetValue(item.type, out info);
        }
        public static void OnQuestCompleted(Player player, int i) {
            ExporterQuestSystem.QuestsCompleted++;
            if (Main.netMode != NetmodeID.SinglePlayer) {
                PacketSystem.Send(PacketType.ExporterQuestsCompleted);
            }

            InnerOnQuestCompleted_SpawnLoot(player, i);

            player.inventory[i].stack--;
            if (player.inventory[i].stack <= 0) {
                player.inventory[i].TurnToAir();
            }
            SoundEngine.PlaySound(SoundID.Grab);
            int type = Main.rand.Next(6);
            if (type == 5) {
                Main.npcChatText = TextHelper.GetTextValue($"Chat.Exporter.ThieveryFailed.{player.GenderString()}");
                return;
            }
            Main.npcChatText = TextHelper.GetTextValueWith("Chat.Exporter.ThieveryComplete." + type, new { ItemName = player.inventory[i].Name });
        }
        public static void InnerOnQuestCompleted_SpawnLoot(Player player, int i) {
            var source = player.GetSource_GiftOrReward("Robster");

            if (Main.rand.NextBool(4)) {
                player.QuickSpawnItem(source, ModContent.ItemType<GoldenRoulette>(), 1);
            }
            else {
                player.QuickSpawnItem(source, ModContent.ItemType<Roulette>(), 1);
            }

            int amtRolled = Math.Max(ExporterQuestSystem.QuestsCompleted / 15, 1);
            for (int k = 0; k < amtRolled; k++) {
                int roulette = SpawnLoot_ChooseRoulette(player, i);
                if (roulette != 0) {
                    player.QuickSpawnItem(source, roulette, 1);
                }
            }

            int extraMoney = Item.silver * 3 * ExporterQuestSystem.QuestsCompleted;
            Helper.DropMoney(source, player.getRect(), Main.rand.Next(Item.silver * 50 + extraMoney / 2, Item.gold + extraMoney));

            ExporterQuestSystem.QuestsCompleted++;

            if (Main.netMode != NetmodeID.SinglePlayer) {
                PacketSystem.Send(PacketType.ExporterQuestsCompleted);
            }
        }
        public static int SpawnLoot_ChooseRoulette(Player player, int i) {
            var choices = new List<int>();
            if (Main.rand.NextBool(3)) {
                choices.Add(ModContent.ItemType<Roulette>());
                choices.Add(ModContent.ItemType<GoldenRoulette>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 2) {
                choices.Add(ModContent.ItemType<SnowRoulette>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 4) {
                choices.Add(ModContent.ItemType<DesertRoulette>());
                choices.Add(ModContent.ItemType<OceanSlotMachine>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 8) {
                choices.Add(ModContent.ItemType<JungleSlotMachine>());
                choices.Add(ModContent.ItemType<SkyRoulette>());
            }
            if (ExporterQuestSystem.QuestsCompleted > 14 && AequusWorld.downedEventDemon) {
                choices.Add(ModContent.ItemType<ShadowRoulette>());
            }
            return choices.Count > 0 ? choices[Main.rand.Next(choices.Count)] : ItemID.None;
        }

        public static void OnQuestFailed(Player player) {
            int type = Main.rand.Next(3);
            if (type == 2) {
                Main.npcChatText = TextHelper.GetTextValue($"Chat.Exporter.ThieveryFailed.{player.GenderString()}");
                return;
            }
            Main.npcChatText = TextHelper.GetTextValueWith($"Chat.Exporter.ThieveryFailed.{type}", new { WorldName = Main.worldName, });
        }
        #endregion
    }
}