﻿using Aequus;
using Aequus.Content.CrossMod;
using Aequus.Items;
using Aequus.NPCs.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.NPCs
{
    public class ShopQuotes : GlobalItem
    {
        public class NPCQuotes
        {
            public static Func<Color> DefaultColor => () => Colors.RarityBlue;

            private readonly int NPC;
            private Func<Color> getColor;
            public readonly Dictionary<int, Func<string>> ItemToQuote;

            internal NPCQuotes(int npc)
            {
                NPC = npc;
                getColor = DefaultColor;
                ItemToQuote = new Dictionary<int, Func<string>>();
            }

            public NPCQuotes AddQuote(Func<string> key, int item)
            {
                ItemToQuote.Add(item, key);
                return this;
            }
            public NPCQuotes AddQuote(string key, int item)
            {
                return AddQuote(() => key, item);
            }

            internal NPCQuotes AddQuote(int item)
            {
                return AddQuote(KeyFromIDs("Aequus", NPC, item), item);
            }
            internal NPCQuotes AddQuote<T>() where T : ModItem
            {
                return AddQuote(ModContent.ItemType<T>());
            }

            internal NPCQuotes AddQuoteWithPrefix(int item, string prefix)
            {
                string key = KeyFromIDs("Aequus", NPC, item);
                return AddQuote(() => AequusText.GetText(prefix) + " " + Language.GetTextValue(key), item);
            }
            internal NPCQuotes AddQuoteWithPrefix<T>(string prefix) where T : ModItem
            {
                return AddQuoteWithPrefix(ModContent.ItemType<T>(), prefix);
            }

            internal NPCQuotes AddPaintingPainterQuote(int item)
            {
                return AddQuoteWithPrefix(item, "Chat.Painter.ShopQuote.Painting");
            }

            public NPCQuotes AddQuote<T>(string key) where T : ModItem
            {
                return AddQuote(key, ModContent.ItemType<T>());
            }
            public NPCQuotes AddQuote(Mod mod, int item)
            {
                return AddQuote(LegacyKeyFromIDs(mod.Name, NPC, item), item);
            }
            public NPCQuotes AddQuote<T>(Mod mod) where T : ModItem
            {
                return AddQuote(mod, ModContent.ItemType<T>());
            }
            internal NPCQuotes LegacyAddQuote(int item)
            {
                return AddQuote(LegacyKeyFromIDs("Aequus", NPC, item), item);
            }
            internal NPCQuotes LegacyAddQuote<T>() where T : ModItem
            {
                return LegacyAddQuote(ModContent.ItemType<T>());
            }

            public NPCQuotes AddQuoteWithSubstitutions(string key, int item, Func<object> substitutions)
            {
                return AddQuote(() => Language.GetTextValueWith(key, substitutions()), item);
            }
            internal NPCQuotes AddQuoteWithSubstitutions(int item, Func<object> substitutions)
            {
                return AddQuoteWithSubstitutions(LegacyKeyFromIDs("Aequus", NPC, item), item, substitutions);
            }

            public NPCQuotes AddQuoteWithSubstitutions(string key, int item, object substitutions)
            {
                return AddQuote(() => Language.GetTextValueWith(key, substitutions), item);
            }
            internal NPCQuotes AddQuoteWithSubstitutions(int item, object substitutions)
            {
                return AddQuoteWithSubstitutions(LegacyKeyFromIDs("Aequus", NPC, item), item, substitutions);
            }

            public NPCQuotes MaleFemaleQuote(string mod, int item)
            {
                string baseQuote = LegacyKeyFromIDs(mod, NPC, item);
                return AddQuote(() =>
                {
                    return baseQuote + (Main.LocalPlayer.Male ? "_Male" : "_Female");
                }, item);
            }
            public NPCQuotes MaleFemaleQuote(Mod mod, int item)
            {
                return MaleFemaleQuote(mod.Name, item);
            }
            internal NPCQuotes MaleFemaleQuote(int item)
            {
                string baseQuote = LegacyKeyFromIDs("Aequus", NPC, item);
                return AddQuote(() =>
                {
                    return baseQuote + (Main.LocalPlayer.Male ? "_Male" : "_Female");
                }, item);
            }

            public NPCQuotes AddQuotesWithConditions(int item, string baseQuote, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuote(() =>
                {
                    foreach (var r in rules)
                    {
                        if (r.condition())
                        {
                            return baseQuote + r.quote;
                        }
                    }
                    return baseQuote;
                }, item);
            }
            public NPCQuotes AddQuotesWithConditions(Mod mod, int item, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditions(item, LegacyKeyFromIDs(mod.Name, NPC, item), rules);
            }
            internal NPCQuotes AddQuotesWithConditions(string mod, int item, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditions(item, LegacyKeyFromIDs(mod, NPC, item), rules);
            }
            internal NPCQuotes AddQuotesWithConditions(int item, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditions("Aequus", item, rules);
            }

            public NPCQuotes AddQuotesWithConditionsWithSubsitution(int item, string baseQuote, Func<object> subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuote(() =>
                {
                    foreach (var r in rules)
                    {
                        if (r.condition())
                        {
                            return Language.GetTextValueWith(baseQuote + r.quote, subsitutions());
                        }
                    }
                    return Language.GetTextValueWith(baseQuote, subsitutions());
                }, item);
            }
            public NPCQuotes AddQuotesWithConditionsWithSubsitution(Mod mod, int item, Func<object> subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution(item, LegacyKeyFromIDs(mod.Name, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(string mod, int item, Func<object> subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution(item, LegacyKeyFromIDs(mod, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(int item, Func<object> subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution("Aequus", item, subsitutions, rules);
            }

            public NPCQuotes AddQuotesWithConditionsWithSubsitution(int item, string baseQuote, object subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuote(() =>
                {
                    foreach (var r in rules)
                    {
                        if (r.condition())
                        {
                            return Language.GetTextValueWith(baseQuote + r.quote, subsitutions);
                        }
                    }
                    return Language.GetTextValueWith(baseQuote, subsitutions);
                }, item);
            }
            public NPCQuotes AddQuotesWithConditionsWithSubsitution(Mod mod, int item, object subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution(item, LegacyKeyFromIDs(mod.Name, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(string mod, int item, object subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution(item, LegacyKeyFromIDs(mod, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(int item, object subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution("Aequus", item, subsitutions, rules);
            }

            internal NPCQuotes AddZoologistQuote(int item)
            {
                string key = LegacyKeyFromIDs("Aequus", NPC, item);
                string altKey = key + ".Lycantrope";
                return Language.GetTextValue(altKey) == altKey ? LegacyAddQuote(item)
                    : AddQuote(() => ZoologistAltText(TalkingNPC()) ? altKey : key, item);
            }

            public NPCQuotes WithColor(Func<Color> color)
            {
                getColor = color;
                return this;
            }
            public NPCQuotes WithColor(Color color)
            {
                return WithColor(() => color);
            }

            public Color GetColor()
            {
                return getColor();
            }
        }

        public class QuoteDatabase : IModCallable
        {
            private Dictionary<int, NPCQuotes> database;

            public QuoteDatabase(bool init = true)
            {
                database = new Dictionary<int, NPCQuotes>();
                if (init)
                {
                    Initalize();
                }
            }

            internal void Initalize()
            {
                database = new Dictionary<int, NPCQuotes>()
                {
                    [NPCID.Merchant] = new NPCQuotes(NPCID.Merchant)
                    .WithColor(Color.Yellow)
                    .LegacyAddQuote(ItemID.MiningHelmet)
                    .AddQuotesWithConditions(ItemID.PiggyBank,
                    (() => Main.GetMoonPhase() == MoonPhase.Full && LanternNight.LanternsUp, "_Goober"))
                    .LegacyAddQuote(ItemID.IronAnvil)
                    .LegacyAddQuote(ItemID.LeadAnvil)
                    .LegacyAddQuote(ItemID.BugNet)
                    .LegacyAddQuote(ItemID.CopperPickaxe)
                    .LegacyAddQuote(ItemID.CopperAxe)
                    .AddQuote("LegacyDialog.5", ItemID.Torch)
                    .LegacyAddQuote(ItemID.LesserHealingPotion)
                    .LegacyAddQuote(ItemID.LesserManaPotion)
                    .LegacyAddQuote(ItemID.WoodenArrow)
                    .LegacyAddQuote(ItemID.Shuriken)
                    .LegacyAddQuote(ItemID.Rope)
                    .LegacyAddQuote(ItemID.Marshmallow)
                    .LegacyAddQuote(ItemID.Furnace)
                    .LegacyAddQuote(ItemID.PinWheel)
                    .LegacyAddQuote(ItemID.ThrowingKnife)
                    .LegacyAddQuote(ItemID.Glowstick)
                    .LegacyAddQuote(ItemID.SharpeningStation)
                    .LegacyAddQuote(ItemID.Safe)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.DiscoBall, () => new { PartyGirl = NPC.GetFirstNPCNameOrNull(NPCID.PartyGirl), },
                    (() => NPC.AnyNPCs(NPCID.PartyGirl), "_PartyGirl"))
                    .LegacyAddQuote(ItemID.Flare)
                    .LegacyAddQuote(ItemID.BlueFlare)
                    .LegacyAddQuote(ItemID.Sickle)
                    .LegacyAddQuote(ItemID.GoldDust)
                    .LegacyAddQuote(ItemID.DrumSet)
                    .LegacyAddQuote(ItemID.DrumStick)
                    .LegacyAddQuote(ItemID.Nail)
                    ,

                    [NPCID.ArmsDealer] = new NPCQuotes(NPCID.ArmsDealer)
                    .WithColor(Color.Gray * 1.45f)
                    .AddQuote("LegacyDialog.67", ItemID.MusketBall)
                    .LegacyAddQuote(ItemID.SilverBullet)
                    .LegacyAddQuote(ItemID.TungstenBullet)
                    .LegacyAddQuote(ItemID.UnholyArrow)
                    .LegacyAddQuote(ItemID.FlintlockPistol)
                    .AddQuote("LegacyDialog.66", ItemID.Minishark)
                    .LegacyAddQuote(ItemID.IllegalGunParts)
                    .LegacyAddQuote(ItemID.Shotgun)
                    .LegacyAddQuote(ItemID.EmptyBullet)
                    .LegacyAddQuote(ItemID.StyngerBolt)
                    .LegacyAddQuote(ItemID.Stake)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.Nail, () => new { Merchant = NPC.GetFirstNPCNameOrNull(NPCID.Merchant), },
                    (() => NPC.AnyNPCs(NPCID.Merchant), "_Merchant"))
                    .LegacyAddQuote(ItemID.CandyCorn)
                    .LegacyAddQuote(ItemID.ExplosiveJackOLantern)
                    .AddQuote(NurseOutfitText, ItemID.NurseHat)
                    .AddQuote(NurseOutfitText, ItemID.NurseShirt)
                    .AddQuote(NurseOutfitText, ItemID.NursePants)
                    .LegacyAddQuote(ItemID.QuadBarrelShotgun)
                    ,

                    [NPCID.Demolitionist] = new NPCQuotes(NPCID.Demolitionist)
                    .WithColor(Color.Gray * 1.45f)
                    .LegacyAddQuote(ItemID.Grenade)
                    .AddQuoteWithSubstitutions("LegacyDialog.93", ItemID.Bomb,
                    new { WorldEvilStone = WorldGen.crimson ? Language.GetTextValue("Misc.Crimstone") : Language.GetTextValue("Misc.Ebonstone"), })
                    .AddQuote("LegacyDialog.101", ItemID.Dynamite)
                    .LegacyAddQuote(ItemID.HellfireArrow)
                    .LegacyAddQuote(ItemID.LandMine)
                    .LegacyAddQuote(ItemID.ExplosivePowder)
                    .LegacyAddQuote(ItemID.DryBomb)
                    .LegacyAddQuote(ItemID.WetBomb)
                    .LegacyAddQuote(ItemID.LavaBomb)
                    .LegacyAddQuote(ItemID.HoneyBomb)
                    ,

                    [NPCID.GoblinTinkerer] = new NPCQuotes(NPCID.GoblinTinkerer)
                    .WithColor(new Color(200, 70, 105, 255))
                    .LegacyAddQuote(ItemID.RocketBoots)
                    .LegacyAddQuote(ItemID.Ruler)
                    .LegacyAddQuote(ItemID.TinkerersWorkshop)
                    .LegacyAddQuote(ItemID.GrapplingHook)
                    .LegacyAddQuote(ItemID.Toolbelt)
                    .LegacyAddQuote(ItemID.SpikyBall),

                    [NPCID.Wizard] = new NPCQuotes(NPCID.Wizard)
                    .WithColor(Color.BlueViolet * 1.5f)
                    .AddQuote(ItemID.CrystalBall)
                    .AddQuote(ItemID.IceRod)
                    .AddQuote(ItemID.GreaterManaPotion)
                    .AddQuote(ItemID.Bell)
                    .AddQuote(ItemID.Harp)
                    .AddQuote(ItemID.SpellTome)
                    .AddQuote(ItemID.Book)
                    .AddQuote(ItemID.MusicBox)
                    .AddQuote(ItemID.EmptyDropper)
                    .AddQuote(ItemID.WizardsHat),

                    [NPCID.Cyborg] = new NPCQuotes(NPCID.Cyborg)
                    .WithColor(Color.Cyan * 1.5f)
                    .LegacyAddQuote(ItemID.RocketI)
                    .LegacyAddQuote(ItemID.RocketII)
                    .LegacyAddQuote(ItemID.RocketIII)
                    .LegacyAddQuote(ItemID.RocketIV)
                    .LegacyAddQuote(ItemID.DryRocket)
                    .LegacyAddQuote(ItemID.ProximityMineLauncher)
                    .LegacyAddQuote(ItemID.Nanites)
                    .LegacyAddQuote(ItemID.ClusterRocketI)
                    .LegacyAddQuote(ItemID.ClusterRocketII)
                    .LegacyAddQuote(ItemID.HiTekSunglasses)
                    .LegacyAddQuote(ItemID.NightVisionHelmet)
                    .LegacyAddQuote(ItemID.PortalGunStation)
                    .LegacyAddQuote(ItemID.EchoBlock)
                    .LegacyAddQuote(ItemID.SpectreGoggles),

                    [NPCID.Painter] = new NPCQuotes(NPCID.Painter)
                    .WithColor(() => AequusHelpers.LerpBetween(new Color[]
                    {
                        Color.Red,
                        Color.Orange,
                        Color.Yellow,
                        Color.Lime,
                        Color.Green,
                        Color.Teal,
                        Color.Cyan,
                        Color.SkyBlue,
                        Color.Blue,
                        Color.Purple,
                        Color.Violet,
                        Color.Pink,
                    }, Main.GlobalTimeWrappedHourly * 0.08f).MaxRGBA(100))
                    .AddQuote(ItemID.Paintbrush)
                    .AddQuote(ItemID.PaintRoller)
                    .AddQuote(ItemID.PaintScraper)
                    .AddQuote(ItemID.RedPaint)
                    .AddQuote(ItemID.OrangePaint)
                    .AddQuote(ItemID.YellowPaint)
                    .AddQuote(ItemID.LimePaint)
                    .AddQuote(ItemID.GreenPaint)
                    .AddQuote(ItemID.TealPaint)
                    .AddQuote(ItemID.CyanPaint)
                    .AddQuote(ItemID.SkyBluePaint)
                    .AddQuote(ItemID.BluePaint)
                    .AddQuote(ItemID.PurplePaint)
                    .AddQuote(ItemID.VioletPaint)
                    .AddQuote(ItemID.PinkPaint)
                    .AddQuote(ItemID.BlackPaint)
                    .AddQuote(ItemID.GrayPaint)
                    .AddQuote(ItemID.WhitePaint)
                    .AddQuote(ItemID.BrownPaint)
                    .AddQuote(ItemID.ShadowPaint)
                    .AddQuote(ItemID.NegativePaint)
                    .AddQuote(ItemID.GlowPaint)
                    .AddPaintingPainterQuote(ItemID.Daylight)
                    .AddPaintingPainterQuote(ItemID.FirstEncounter)
                    .AddPaintingPainterQuote(ItemID.GoodMorning)
                    .AddPaintingPainterQuote(ItemID.UndergroundReward)
                    .AddPaintingPainterQuote(ItemID.ThroughtheWindow)
                    .AddPaintingPainterQuote(ItemID.DeadlandComesAlive)
                    .AddPaintingPainterQuote(ItemID.LightlessChasms)
                    .AddPaintingPainterQuote(ItemID.TheLandofDeceivingLooks)
                    .AddPaintingPainterQuote(ItemID.DoNotStepontheGrass)
                    .AddPaintingPainterQuote(ItemID.ColdWatersintheWhiteLand)
                    .AddPaintingPainterQuote(ItemID.SecretoftheSands)
                    .AddPaintingPainterQuote(ItemID.EvilPresence)
                    .AddPaintingPainterQuote(ItemID.PlaceAbovetheClouds)
                    .AddPaintingPainterQuote(ItemID.SkyGuardian)
                    .AddPaintingPainterQuote(ItemID.Nevermore)
                    .AddPaintingPainterQuote(ItemID.Reborn)
                    .AddPaintingPainterQuote(ItemID.Graveyard)
                    .AddPaintingPainterQuote(ItemID.GhostManifestation)
                    .AddPaintingPainterQuote(ItemID.WickedUndead)
                    .AddPaintingPainterQuote(ItemID.BloodyGoblet)
                    .AddPaintingPainterQuote(ItemID.StillLife)
                    .AddPaintingPainterQuote(ItemID.BubbleWallpaper)
                    .AddPaintingPainterQuote(ItemID.CopperPipeWallpaper)
                    .AddPaintingPainterQuote(ItemID.DuckyWallpaper)
                    .AddPaintingPainterQuote(ItemID.FancyGreyWallpaper)
                    .AddPaintingPainterQuote(ItemID.IceFloeWallpaper)
                    .AddPaintingPainterQuote(ItemID.MusicWallpaper)
                    .AddPaintingPainterQuote(ItemID.PurpleRainWallpaper)
                    .AddPaintingPainterQuote(ItemID.RainbowWallpaper)
                    .AddPaintingPainterQuote(ItemID.SparkleStoneWallpaper)
                    .AddPaintingPainterQuote(ItemID.StarlitHeavenWallpaper)
                    .AddQuoteWithPrefix(ItemID.ChristmasTreeWallpaper, "Chat.Painter.ShopQuote.ChristmasWallpapers")
                    .AddQuoteWithPrefix(ItemID.OrnamentWallpaper, "Chat.Painter.ShopQuote.ChristmasWallpapers")
                    .AddQuoteWithPrefix(ItemID.CandyCaneWallpaper, "Chat.Painter.ShopQuote.ChristmasWallpapers")
                    .AddQuote("Mods.Aequus.Chat.Painter.ShopQuote.ChristmasWallpapers", ItemID.FestiveWallpaper)
                    .AddQuote("Mods.Aequus.Chat.Painter.ShopQuote.ChristmasWallpapers", ItemID.BluegreenWallpaper)
                    .AddQuoteWithPrefix(ItemID.StarsWallpaper, "Chat.Painter.ShopQuote.ChristmasWallpapers")
                    .AddQuoteWithPrefix(ItemID.SquigglesWallpaper, "Chat.Painter.ShopQuote.ChristmasWallpapers")
                    .AddQuoteWithPrefix(ItemID.SnowflakeWallpaper, "Chat.Painter.ShopQuote.ChristmasWallpapers")
                    .AddQuote(ItemID.KrampusHornWallpaper)
                    .AddQuote(ItemID.GrinchFingerWallpaper)
                    ,

                    [NPCID.WitchDoctor] = new NPCQuotes(NPCID.WitchDoctor)
                    .WithColor(Color.GreenYellow)
                    .AddQuote(ItemID.ImbuingStation)
                    .AddQuote(ItemID.Blowgun)
                    .AddQuote(ItemID.StyngerBolt)
                    .AddQuote(ItemID.Stake)
                    .AddQuote(ItemID.Cauldron)
                    .AddQuote(ItemID.TikiTotem)
                    .AddQuote(ItemID.LeafWings)
                    .AddQuote(ItemID.VialofVenom)
                    .AddQuote(ItemID.TikiMask)
                    .AddQuote(ItemID.TikiShirt)
                    .AddQuote(ItemID.TikiPants)
                    .AddQuote(ItemID.PygmyNecklace)
                    .AddQuote(ItemID.HerculesBeetle)
                    .AddQuoteWithPrefix(ItemID.PureWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.DesertWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.JungleWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.IcyWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.CorruptWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.CrimsonWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.HallowedWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.BloodWaterFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.CavernFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuoteWithPrefix(ItemID.OasisFountain, "Chat.WitchDoctor.ShopQuote.Fountains")
                    .AddQuote(ItemID.BewitchingTable)
                    ,

                    [NPCID.Pirate] = new NPCQuotes(NPCID.Pirate)
                    .WithColor(Color.Orange * 1.2f)
                    .LegacyAddQuote(ItemID.Cannon)
                    .LegacyAddQuote(ItemID.Cannonball)
                    .LegacyAddQuote(ItemID.PirateHat)
                    .LegacyAddQuote(ItemID.PirateShirt)
                    .LegacyAddQuote(ItemID.PiratePants)
                    .LegacyAddQuote(ItemID.Sail)
                    .LegacyAddQuote(ItemID.ParrotCracker)
                    .LegacyAddQuote(ItemID.BunnyCannon)
                    .LegacyAddQuote(ItemID.ExplosiveBunny),

                    [NPCID.DD2Bartender] = new NPCQuotes(NPCID.DD2Bartender)
                    .WithColor(Color.Lerp(Color.Orange, Color.White, 0.66f))
                    .AddQuote(ItemID.Ale)
                    .AddQuote(ItemID.DD2ElderCrystal)
                    .AddQuote(ItemID.DD2ElderCrystalStand)
                    .AddQuote(ItemID.DefendersForge)
                    .AddQuote(ItemID.DD2FlameburstTowerT1Popper)
                    .AddQuote(ItemID.DD2FlameburstTowerT2Popper)
                    .AddQuote(ItemID.DD2FlameburstTowerT3Popper)
                    .AddQuote(ItemID.DD2BallistraTowerT1Popper)
                    .AddQuote(ItemID.DD2BallistraTowerT2Popper)
                    .AddQuote(ItemID.DD2BallistraTowerT3Popper)
                    .AddQuote(ItemID.DD2ExplosiveTrapT1Popper)
                    .AddQuote(ItemID.DD2ExplosiveTrapT2Popper)
                    .AddQuote(ItemID.DD2ExplosiveTrapT3Popper)
                    .AddQuote(ItemID.DD2LightningAuraT1Popper)
                    .AddQuote(ItemID.DD2LightningAuraT2Popper)
                    .AddQuote(ItemID.DD2LightningAuraT3Popper)
                    .AddQuote(ItemID.MonkBrows)
                    .AddQuote(ItemID.MonkShirt)
                    .AddQuote(ItemID.MonkPants)
                    .AddQuote(ItemID.MonkAltHead)
                    .AddQuote(ItemID.MonkAltShirt)
                    .AddQuote(ItemID.MonkAltPants)
                    .AddQuote(ItemID.SquireGreatHelm)
                    .AddQuote(ItemID.SquirePlating)
                    .AddQuote(ItemID.SquireGreaves)
                    .AddQuote(ItemID.SquireAltHead)
                    .AddQuote(ItemID.SquireAltShirt)
                    .AddQuote(ItemID.SquireAltPants)
                    .AddQuote(ItemID.ApprenticeHat)
                    .AddQuote(ItemID.ApprenticeRobe)
                    .AddQuote(ItemID.ApprenticeTrousers)
                    .AddQuote(ItemID.ApprenticeAltHead)
                    .AddQuote(ItemID.ApprenticeAltShirt)
                    .AddQuote(ItemID.ApprenticeAltPants)
                    .AddQuote(ItemID.HuntressWig)
                    .AddQuote(ItemID.HuntressJerkin)
                    .AddQuote(ItemID.HuntressPants)
                    .AddQuote(ItemID.HuntressAltHead)
                    .AddQuote(ItemID.HuntressAltShirt)
                    .AddQuote(ItemID.HuntressAltPants)
                    ,

                    [NPCID.BestiaryGirl] = new NPCQuotes(NPCID.BestiaryGirl)
                    .WithColor(() => ZoologistAltText(TalkingNPC()) ? Color.Red : new Color(255, 140, 160, 255))
                    .AddZoologistQuote(ItemID.DontHurtCrittersBook)
                    .AddZoologistQuote(ItemID.SquirrelHook)
                    .AddZoologistQuote(ItemID.BlandWhip)
                    .AddZoologistQuote(ItemID.MolluskWhistle)
                    .AddZoologistQuote(ItemID.CritterShampoo)
                    .AddZoologistQuote(ItemID.DogEars)
                    .AddZoologistQuote(ItemID.DogTail)
                    .AddZoologistQuote(ItemID.FoxEars)
                    .AddZoologistQuote(ItemID.FoxTail)
                    .AddZoologistQuote(ItemID.LizardEars)
                    .AddZoologistQuote(ItemID.LizardTail)
                    .AddZoologistQuote(ItemID.BunnyEars)
                    .AddZoologistQuote(ItemID.BunnyTail)
                    .AddZoologistQuote(ItemID.FullMoonSqueakyToy)
                    .AddZoologistQuote(ItemID.MudBud)
                    .AddZoologistQuote(ItemID.LicenseCat)
                    .AddZoologistQuote(ItemID.LicenseDog)
                    .AddZoologistQuote(ItemID.LicenseBunny)
                    .AddZoologistQuote(ItemID.KiteKoi)
                    .AddZoologistQuote(ItemID.KiteCrawltipede)
                    .AddZoologistQuote(ItemID.PaintedHorseSaddle)
                    .AddZoologistQuote(ItemID.MajesticHorseSaddle)
                    .AddZoologistQuote(ItemID.DarkHorseSaddle)
                    .AddZoologistQuote(ItemID.VanityTreeSakuraSeed)
                    .AddZoologistQuote(ItemID.VanityTreeYellowWillowSeed)
                    .AddZoologistQuote(ItemID.RabbitOrder)
                    .AddZoologistQuote(ItemID.JoustingLance)
                    .AddZoologistQuote(ItemID.FairyGlowstick)
                    .AddZoologistQuote(ItemID.WorldGlobe)
                    .AddZoologistQuote(ItemID.TreeGlobe)
                    .AddZoologistQuote(ItemID.LightningCarrot)
                    .AddZoologistQuote(ItemID.DiggingMoleMinecart)
                    .AddZoologistQuote(ItemID.BallOfFuseWire),

                    [NPCID.Princess] = new NPCQuotes(NPCID.Princess)
                    .WithColor(Main.creativeModeColor * 1.25f)
                    .LegacyAddQuote(ItemID.RoyalTiara)
                    .LegacyAddQuote(ItemID.RoyalDressTop)
                    .LegacyAddQuote(ItemID.RoyalDressBottom)
                    .LegacyAddQuote(ItemID.RoyalScepter)
                    .MaleFemaleQuote(ItemID.GlassSlipper)
                    .LegacyAddQuote(ItemID.PrinceUniform)
                    .LegacyAddQuote(ItemID.PrincePants)
                    .LegacyAddQuote(ItemID.PrinceCape)
                    .LegacyAddQuote(ItemID.PottedCrystalPlantFern)
                    .LegacyAddQuote(ItemID.PottedCrystalPlantSpiral)
                    .LegacyAddQuote(ItemID.PottedCrystalPlantTeardrop)
                    .LegacyAddQuote(ItemID.PottedCrystalPlantTree)
                    .LegacyAddQuote(ItemID.Princess64)
                    .LegacyAddQuote(ItemID.PaintingOfALass)
                    .LegacyAddQuote(ItemID.DarkSideHallow)
                    .LegacyAddQuote(ItemID.BerniePetItem)
                    .AddQuotesWithConditions(ItemID.MusicBoxCredits,
                    (() =>
                    {
                        int guide = NPC.FindFirstNPC(NPCID.Guide);
                        return guide != -1 ? Main.npc[guide].GivenName == "Andrew" : false;
                    }, "_GuideAndrew"))
                    .LegacyAddQuote(ItemID.SlimeStaff)
                    .LegacyAddQuote(ItemID.HeartLantern)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.FlaskofParty, () => new { PartyGirl = NPC.GetFirstNPCNameOrNull(NPCID.PartyGirl), },
                    (() => NPC.AnyNPCs(NPCID.PartyGirl), "_PartyGirl"))
                    .LegacyAddQuote(ItemID.SandstorminaBottle)
                    .LegacyAddQuote(ItemID.Terragrim)
                    .LegacyAddQuote(ItemID.PirateStaff)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.DiscountCard, () => new { Exporter = NPC.GetFirstNPCNameOrNull(ModContent.NPCType<Exporter>()), },
                    (() => NPC.AnyNPCs(ModContent.NPCType<Exporter>()), "_Exporter"))
                    .AddQuotesWithConditionsWithSubsitution(ItemID.LuckyCoin, () => new { Exporter = NPC.GetFirstNPCNameOrNull(ModContent.NPCType<Exporter>()), },
                    (() => NPC.AnyNPCs(ModContent.NPCType<Exporter>()), "_Exporter"))
                    .LegacyAddQuote(ItemID.CoinGun)
                    ,
                };
            }

            public string NurseOutfitText()
            {
                string key = "Mods.Aequus.Chat.Terraria_ArmsDealer.ShopQuote.NurseOutfit";
                int nurse = NPC.FindFirstNPC(NPCID.Nurse);
                if (nurse != -1)
                {
                    return Language.GetTextValueWith(key + "_Nurse", new { Nurse = Main.npc[nurse].GivenName });
                }
                return key;
            }

            public NPCQuotes this[int npc]
            {
                get => database[npc];
            }

            public bool TryGetValue(int npc, out NPCQuotes quote)
            {
                return database.TryGetValue(npc, out quote);
            }

            /// <summary>
            /// Adds an NPC entry if one doesn't exist already
            /// </summary>
            /// <param name="npc"></param>
            /// <returns></returns>
            public NPCQuotes AddNPC(int npc)
            {
                if (TryGetValue(npc, out var quote))
                {
                    return quote;
                }
                database.Add(npc, new NPCQuotes(npc));
                return this[npc];
            }

            /// <summary>
            /// Adds shop quote data for an npc
            /// <para>Parameter 1: NPC Type (<see cref="int"/>)</para>
            /// <para>Parameter 2: Key (<see cref="string"/>)</para>
            /// <para>Parameter 2 (Alternative): Mod (<see cref="Mod"/>) Would generate a key which looks like: <code>Mods.MYMODNAME.Chat.(NPCKEY).ShopQuotes.(ITEMKEY)</code></para>
            /// <para>Parameter 3: Item Type (<see cref="int"/>) OR (<see cref="int"/>[])</para>
            /// <para>To make a town npc use a specific quote color:</para>
            /// <para>Parameter 1: NPC Type (<see cref="int"/>)</para>
            /// <para>Parameter 2: Color (<see cref="Color"/>)</para>
            /// <para>A successful mod call would look like:</para>
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, <see cref="Color"/>);</code> OR
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, this, <see cref="ItemID"/>);</code> OR
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, "This is text. I can also use language keys here.", <see cref="ItemID"/>);</code>
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, "This is text. I can also use language keys here.", new int[] { <see cref="ItemID"/>, });</code>
            /// <para>Please handle these mod calls in <see cref="Mod.PostSetupContent"/>.</para>
            /// </summary>
            /// <param name="aequus"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object HandleModCall(Aequus aequus, object[] args)
            {
                int npc = (int)args[1];

                AddNPC(npc);
                if (args[2] is Color color)
                {
                    this[npc].WithColor(color);
                    return IModCallable.Success;
                }
                int[] items = new int[1];
                if (args[3] is int[] arr)
                {
                    items = arr;
                }
                else
                {
                    items[0] = (int)args[3];
                }

                foreach (var item in items)
                {
                    if (args[2] is string quoteText)
                    {
                        this[npc].AddQuote(quoteText, item);
                    }
                    else if (args[2] is Mod mod)
                    {
                        this[npc].AddQuote(mod, item);
                    }
                }

                return IModCallable.Success;
            }

            void ILoadable.Load(Mod mod)
            {
            }

            void ILoadable.Unload()
            {
            }
        }

        public static QuoteDatabase Database { get; private set; }

        public override void Load()
        {
            Database = new QuoteDatabase(init: true);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ClientConfig.Instance.NPCShopQuotes)
            {
                return;
            }

            if (item.buy && item.tooltipContext == ItemSlot.Context.ShopItem && Main.LocalPlayer.talkNPC != -1)
            {
                try
                {
                    AddShopQuote(item, tooltips);
                }
                catch
                {
                }
            }
        }

        public void AddShopQuote(Item item, List<TooltipLine> tooltips)
        {
            var talkNPC = Main.npc[Main.LocalPlayer.talkNPC];

            if (talkNPC.type < Main.maxNPCTypes && !ClientConfig.Instance.OtherNPCShopQuotes)
            {
                return;
            }

            if (TryGetQuote(talkNPC, item, out string text, out var textColor))
            {
                string lineText = "";
                lineText += Language.GetTextValue(text);
                int index = tooltips.GetIndex("JourneyResearch");
                tooltips.Insert(index, new TooltipLine(Mod, "Fake", "_"));
                tooltips.Insert(index, new TooltipLine(Mod, "ShopQuote", FixNewlines(lineText)) { OverrideColor = textColor, });
            }
        }
        public bool TryGetQuote(NPC talkNPC, Item item, out string text, out Color color)
        {
            text = "";
            color = default(Color);
            if (Database.TryGetValue(talkNPC.type, out var value))
            {
                if (value.ItemToQuote.TryGetValue(item.type, out var itemQuote))
                {
                    text = itemQuote();
                    color = value.GetColor();
                    return true;
                }
            }
            return false;
        }

        public static void DrawShopQuote(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color, NPC npc)
        {
            var statusBubble = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/StatusBubble").Value;
            var chatBubbleFrame = statusBubble.Frame(horizontalFrames: TextureCache.StatusBubbleFramesX, frameX: 2);
            var chatBubbleScale = baseScale * 0.9f;
            chatBubbleScale.X *= 1.1f;
            var chatBubblePosition = new Vector2(x + chatBubbleFrame.Width / 2f * chatBubbleScale.X, y + chatBubbleFrame.Height / 2f * chatBubbleScale.Y);

            Main.spriteBatch.Draw(statusBubble, chatBubblePosition + new Vector2(2f) * chatBubbleScale,
                chatBubbleFrame, Color.Black * 0.4f, 0f, chatBubbleFrame.Size() / 2f, chatBubbleScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(statusBubble, chatBubblePosition,
                chatBubbleFrame, Color.White, 0f, chatBubbleFrame.Size() / 2f, chatBubbleScale, SpriteEffects.None, 0f);


            var headTexture = GetHeadTexture(npc);

            if (headTexture.IsLoaded)
            {
                var headFrame = headTexture.Value.Frame();
                var headOrigin = headFrame.Size() / 2f;

                Main.spriteBatch.Draw(headTexture.Value, chatBubblePosition + new Vector2(2f),
                    headFrame, Color.Black * 0.4f, 0f, headOrigin, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(headTexture.Value, chatBubblePosition,
                    headFrame, Color.White, 0f, headOrigin, 1f, SpriteEffects.None, 0f);
            }

            var font = FontAssets.MouseText.Value;
            string measurementString = text;
            if (text.Contains('\n'))
            {
                measurementString = text.Split('\n')[0];
            }
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, chatBubblePosition.Y - font.MeasureString(measurementString).Y / 2f), color, rotation, origin, baseScale);
        }
        public static void DrawShopQuote(string text, int x, int y, Color color, NPC npc)
        {
            DrawShopQuote(text, x, y, 0f, Vector2.Zero, Vector2.One, color, npc);
        }
        public static void DrawShopQuote(DrawableTooltipLine line, NPC npc)
        {
            DrawShopQuote(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color), npc);
        }

        private static Asset<Texture2D> GetHeadTexture(NPC npc)
        {
            if (npc.type == NPCID.BestiaryGirl && ZoologistAltText(npc))
            {
                return ModContent.Request<Texture2D>("Aequus/Assets/UI/ZoologistAltHead");
            }

            int headType = TownNPCProfiles.Instance.GetProfile(npc, out var npcProfile)
                ? npcProfile.GetHeadTextureIndex(npc)
                : NPC.TypeToDefaultHeadIndex(npc.type);

            return TextureAssets.NpcHead[headType];
        }

        public static string KeyFromIDs(string mod, int npc, int item)
        {
            string npcName = npc > Main.maxNPCTypes ? AequusText.CreateKeyNameFromSearch(ModContent.GetModNPC(npc).FullName) : NPCID.Search.GetName(npc);
            return $"Mods.{mod}.Chat.{npcName}.ShopQuote.{AequusText.CreateKeyNameFromSearch(ItemID.Search.GetName(item))}";
        }

        public static string LegacyKeyFromIDs(string mod, int npc, int item)
        {
            string npcName = AequusText.CreateSearchNameFromNPC(npc);
            string itemName = (item < Main.maxItemTypes ? "Terraria_" : "") + AequusText.CreateKeyNameFromSearch(ItemID.Search.GetName(item));

            return $"Mods.{mod}.Chat.{npcName}.ShopQuote.{itemName}";
        }

        public static NPC TalkingNPC()
        {
            if (Main.LocalPlayer.talkNPC == -1)
            {
                return null;
            }

            return Main.npc[Main.LocalPlayer.talkNPC];
        }

        public static bool ZoologistAltText(NPC npc)
        {
            return npc?.ShouldBestiaryGirlBeLycantrope() == true;
        }

        public static string FixNewlines(string text)
        {
            var lines = text.Split('\n');
            int maxCharacters = Main.screenWidth / 10;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > maxCharacters)
                {
                    int period = 0;
                    var punctuation = lines[i].Split(". ");

                    for (int j = 0; j < punctuation.Length - 1; j++)
                    {
                        punctuation[j] += ". ";
                    }

                    punctuation = FurtherSplit(", ", punctuation);
                    punctuation = FurtherSplit("- ", punctuation);
                    punctuation = FurtherSplit("! ", punctuation);
                    lines[i] = string.Empty;
                    for (int j = 0; j < punctuation.Length; j++)
                    {
                        if (period + punctuation[j].Length >= maxCharacters)
                        {
                            if (j != 0)
                            {
                                lines[i] += '\n' + "         ";
                            }
                            lines[i] += punctuation[j];
                            period = 0;
                        }
                        else
                        {
                            lines[i] += punctuation[j];
                            period += lines[i].Length;
                        }
                    }
                }
                lines[i] = "         " + lines[i];
            }

            text = string.Join('\n', lines);
            if (text.Trim().EndsWith('\n'))
            {
                text.Remove(text.Length - 2, 1);
            }

            return text;
        }

        public static string[] FurtherSplit(string value, string[] text)
        {
            var t = new List<string>();
            foreach (var s in text)
            {
                var t2 = s.Split(value);
                for (int i = 0; i < t2.Length - 1; i++)
                {
                    t2[i] += value;
                }
                t.AddRange(t2);
            }
            return t.ToArray();
        }
    }
}