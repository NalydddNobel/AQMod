using Aequus.Common.NPCs;
using Aequus.Common.Personalities;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.CrossMod;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.UI.GrabBagReroll;
using Aequus.Items.Accessories.Combat.OnHit.Anchor;
using Aequus.Items.Accessories.Combat.Sentry.SentrySquid;
using Aequus.Items.Accessories.Life.Water;
using Aequus.Items.Accessories.Misc.Luck;
using Aequus.Items.Accessories.Misc.Money;
using Aequus.Items.Misc.Spawners;
using Aequus.Items.Tools;
using Aequus.Items.Vanity;
using Aequus.Items.Vanity.Equipable;
using Aequus.Items.Weapons.Melee.Mallet;
using Aequus.Items.Weapons.Ranged;
using Aequus.NPCs.Monsters.BossMonsters.Crabson;
using Aequus.Tiles.CraftingStations;
using Aequus.Tiles.Furniture.Crab;
using Aequus.Tiles.Furniture.Jeweled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Town.ExporterNPC {
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
            NPCID.Sets.AttackType[NPC.type] = 3;
            NPCID.Sets.AttackTime[NPC.type] = 10;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new(0) {
                Velocity = 1f,
                Direction = -1,
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
            shop.Add<Mallet>(Condition.InGraveyard, Condition.NotRemixWorld)
                .Add(ItemID.Kraken, Condition.RemixWorld)
                .Add<GrandReward>(Condition.NotRemixWorld)
                .Add<RichMansMonocle>(Condition.NotRemixWorld)
                .Add<MermanFins>(Condition.NotRemixWorld)
                .Add<SkeletonKey>(Condition.Hardmode)
                .Add<Items.Weapons.Melee.DynaKnife.Dynaknife>()
                .Add<Items.Weapons.Melee.LihzahrdKusariyari.LihzahrdKusariyari>(Condition.DownedPlantera)

                .Add(ItemID.BreathingReed, Condition.MoonPhaseFull)
                .Add(ItemID.Flipper, Condition.MoonPhaseWaningGibbous)
                .Add(ItemID.Trident, Condition.MoonPhaseThirdQuarter)
                .Add(ItemID.FloatingTube, Condition.MoonPhaseWaningCrescent)
                .Add(ItemID.WaterWalkingBoots, Condition.MoonPhaseNew)
                .Add<SentrySquid>(Condition.MoonPhaseNew)
                .Add<DavyJonesAnchor>(Condition.MoonPhaseWaxingCrescent)
                .Add<StarPhish>(Condition.MoonPhaseFirstQuarter)
                .Add<BreathConserver>(Condition.MoonPhaseWaxingGibbous)

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

                .Add<Headless>(Condition.DownedPumpking)
                .Add<PumpkingCursor>(Condition.DownedPumpking)
                .Add<PumpkingCloak>(Condition.DownedPumpking)
                .Add<EyeGlint>(Condition.DownedPumpking)
                .Add<XmasCursor>(Condition.DownedPumpking)
                .Add<GiftingSpirit>(Condition.DownedPumpking)
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
    }
}