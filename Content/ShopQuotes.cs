using Aequus;
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

namespace Aequus.Content
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
            public NPCQuotes AddQuote<T>(string key) where T : ModItem
            {
                return AddQuote(key, ModContent.ItemType<T>());
            }
            public NPCQuotes AddQuote(Mod mod, int item)
            {
                return AddQuote(KeyFromIDs(mod.Name, NPC, item), item);
            }
            public NPCQuotes AddQuote<T>(Mod mod) where T : ModItem
            {
                return AddQuote(mod, ModContent.ItemType<T>());
            }
            internal NPCQuotes AddQuote(int item)
            {
                return AddQuote(KeyFromIDs("Aequus", NPC, item), item);
            }
            internal NPCQuotes AddQuote<T>() where T : ModItem
            {
                return AddQuote(ModContent.ItemType<T>());
            }

            public NPCQuotes AddQuoteWithSubstitutions(string key, int item, Func<object> substitutions)
            {
                return AddQuote(() => Language.GetTextValueWith(key, substitutions()), item);
            }
            internal NPCQuotes AddQuoteWithSubstitutions(int item, Func<object> substitutions)
            {
                return AddQuoteWithSubstitutions(KeyFromIDs("Aequus", NPC, item), item, substitutions);
            }

            public NPCQuotes AddQuoteWithSubstitutions(string key, int item, object substitutions)
            {
                return AddQuote(() => Language.GetTextValueWith(key, substitutions), item);
            }
            internal NPCQuotes AddQuoteWithSubstitutions(int item, object substitutions)
            {
                return AddQuoteWithSubstitutions(KeyFromIDs("Aequus", NPC, item), item, substitutions);
            }

            public NPCQuotes MaleFemaleQuote(string mod, int item)
            {
                string baseQuote = KeyFromIDs(mod, NPC, item);
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
                string baseQuote = KeyFromIDs("Aequus", NPC, item);
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
                return AddQuotesWithConditions(item, KeyFromIDs(mod.Name, NPC, item), rules);
            }
            internal NPCQuotes AddQuotesWithConditions(string mod, int item, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditions(item, KeyFromIDs(mod, NPC, item), rules);
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
                return AddQuotesWithConditionsWithSubsitution(item, KeyFromIDs(mod.Name, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(string mod, int item, Func<object> subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution(item, KeyFromIDs(mod, NPC, item), subsitutions, rules);
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
                return AddQuotesWithConditionsWithSubsitution(item, KeyFromIDs(mod.Name, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(string mod, int item, object subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution(item, KeyFromIDs(mod, NPC, item), subsitutions, rules);
            }
            internal NPCQuotes AddQuotesWithConditionsWithSubsitution(int item, object subsitutions, params (Func<bool> condition, string quote)[] rules)
            {
                return AddQuotesWithConditionsWithSubsitution("Aequus", item, subsitutions, rules);
            }

            internal NPCQuotes AddZoologistQuote(int item)
            {
                string key = KeyFromIDs("Aequus", NPC, item);
                string altKey = key + ".Lycantrope";
                return Language.GetTextValue(altKey) == altKey ? AddQuote(item)
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
                    .AddQuote(ItemID.MiningHelmet)
                    .AddQuotesWithConditions(ItemID.PiggyBank,
                    (() => Main.GetMoonPhase() == MoonPhase.Full && LanternNight.LanternsUp, "_Goober"))
                    .AddQuote(ItemID.IronAnvil)
                    .AddQuote(ItemID.LeadAnvil)
                    .AddQuote(ItemID.BugNet)
                    .AddQuote(ItemID.CopperPickaxe)
                    .AddQuote(ItemID.CopperAxe)
                    .AddQuote("LegacyDialog.5", ItemID.Torch)
                    .AddQuote(ItemID.LesserHealingPotion)
                    .AddQuote(ItemID.LesserManaPotion)
                    .AddQuote(ItemID.WoodenArrow)
                    .AddQuote(ItemID.Shuriken)
                    .AddQuote(ItemID.Rope)
                    .AddQuote(ItemID.Marshmallow)
                    .AddQuote(ItemID.Furnace)
                    .AddQuote(ItemID.PinWheel)
                    .AddQuote(ItemID.ThrowingKnife)
                    .AddQuote(ItemID.Glowstick)
                    .AddQuote(ItemID.SharpeningStation)
                    .AddQuote(ItemID.Safe)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.DiscoBall, () => new { PartyGirl = NPC.GetFirstNPCNameOrNull(NPCID.PartyGirl), },
                    (() => NPC.AnyNPCs(NPCID.PartyGirl), "_PartyGirl"))
                    .AddQuote(ItemID.Flare)
                    .AddQuote(ItemID.BlueFlare)
                    .AddQuote(ItemID.Sickle)
                    .AddQuote(ItemID.GoldDust)
                    .AddQuote(ItemID.DrumSet)
                    .AddQuote(ItemID.DrumStick)
                    .AddQuote(ItemID.Nail)
                    ,

                    [NPCID.ArmsDealer] = new NPCQuotes(NPCID.ArmsDealer)
                    .WithColor(Color.Gray * 1.45f)
                    .AddQuote("LegacyDialog.67", ItemID.MusketBall)
                    .AddQuote(ItemID.SilverBullet)
                    .AddQuote(ItemID.TungstenBullet)
                    .AddQuote(ItemID.UnholyArrow)
                    .AddQuote(ItemID.FlintlockPistol)
                    .AddQuote("LegacyDialog.66", ItemID.Minishark)
                    .AddQuote(ItemID.IllegalGunParts)
                    .AddQuote(ItemID.Shotgun)
                    .AddQuote(ItemID.EmptyBullet)
                    .AddQuote(ItemID.StyngerBolt)
                    .AddQuote(ItemID.Stake)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.Nail, () => new { Merchant = NPC.GetFirstNPCNameOrNull(NPCID.Merchant), },
                    (() => NPC.AnyNPCs(NPCID.Merchant), "_Merchant"))
                    .AddQuote(ItemID.CandyCorn)
                    .AddQuote(ItemID.ExplosiveJackOLantern)
                    .AddQuote(NurseOutfitText, ItemID.NurseHat)
                    .AddQuote(NurseOutfitText, ItemID.NurseShirt)
                    .AddQuote(NurseOutfitText, ItemID.NursePants)
                    .AddQuote(ItemID.QuadBarrelShotgun)
                    ,

                    [NPCID.Demolitionist] = new NPCQuotes(NPCID.Demolitionist)
                    .WithColor(Color.Gray * 1.45f)
                    .AddQuote(ItemID.Grenade)
                    .AddQuoteWithSubstitutions("LegacyDialog.93", ItemID.Bomb,
                    new { WorldEvilStone = WorldGen.crimson ? Language.GetTextValue("Misc.Crimstone") : Language.GetTextValue("Misc.Ebonstone"), })
                    .AddQuote("LegacyDialog.101", ItemID.Dynamite)
                    .AddQuote(ItemID.HellfireArrow)
                    .AddQuote(ItemID.LandMine)
                    .AddQuote(ItemID.ExplosivePowder)
                    .AddQuote(ItemID.DryBomb)
                    .AddQuote(ItemID.WetBomb)
                    .AddQuote(ItemID.LavaBomb)
                    .AddQuote(ItemID.HoneyBomb)
                    ,

                    [NPCID.GoblinTinkerer] = new NPCQuotes(NPCID.GoblinTinkerer)
                    .WithColor(new Color(200, 70, 105, 255))
                    .AddQuote(ItemID.RocketBoots)
                    .AddQuote(ItemID.Ruler)
                    .AddQuote(ItemID.TinkerersWorkshop)
                    .AddQuote(ItemID.GrapplingHook)
                    .AddQuote(ItemID.Toolbelt)
                    .AddQuote(ItemID.SpikyBall)
                    ,

                    [NPCID.Cyborg] = new NPCQuotes(NPCID.Cyborg)
                    .WithColor(Color.Cyan * 1.5f)
                    .AddQuote(ItemID.RocketI)
                    .AddQuote(ItemID.RocketII)
                    .AddQuote(ItemID.RocketIII)
                    .AddQuote(ItemID.RocketIV)
                    .AddQuote(ItemID.DryRocket)
                    .AddQuote(ItemID.ProximityMineLauncher)
                    .AddQuote(ItemID.Nanites)
                    .AddQuote(ItemID.ClusterRocketI)
                    .AddQuote(ItemID.ClusterRocketII)
                    .AddQuote(ItemID.HiTekSunglasses)
                    .AddQuote(ItemID.NightVisionHelmet)
                    .AddQuote(ItemID.PortalGunStation)
                    .AddQuote(ItemID.EchoBlock)
                    .AddQuote(ItemID.SpectreGoggles),

                    [NPCID.Pirate] = new NPCQuotes(NPCID.Pirate)
                    .WithColor(Color.Orange * 1.2f)
                    .AddQuote(ItemID.Cannon)
                    .AddQuote(ItemID.Cannonball)
                    .AddQuote(ItemID.PirateHat)
                    .AddQuote(ItemID.PirateShirt)
                    .AddQuote(ItemID.PiratePants)
                    .AddQuote(ItemID.Sail)
                    .AddQuote(ItemID.ParrotCracker)
                    .AddQuote(ItemID.BunnyCannon)
                    .AddQuote(ItemID.ExplosiveBunny)
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
                    .AddZoologistQuote(ItemID.BallOfFuseWire)
                    ,

                    [NPCID.Princess] = new NPCQuotes(NPCID.Princess)
                    .WithColor(Main.creativeModeColor * 1.25f)
                    .AddQuote(ItemID.RoyalTiara)
                    .AddQuote(ItemID.RoyalDressTop)
                    .AddQuote(ItemID.RoyalDressBottom)
                    .AddQuote(ItemID.RoyalScepter)
                    .MaleFemaleQuote(ItemID.GlassSlipper)
                    .AddQuote(ItemID.PrinceUniform)
                    .AddQuote(ItemID.PrincePants)
                    .AddQuote(ItemID.PrinceCape)
                    .AddQuote(ItemID.PottedCrystalPlantFern)
                    .AddQuote(ItemID.PottedCrystalPlantSpiral)
                    .AddQuote(ItemID.PottedCrystalPlantTeardrop)
                    .AddQuote(ItemID.PottedCrystalPlantTree)
                    .AddQuote(ItemID.Princess64)
                    .AddQuote(ItemID.PaintingOfALass)
                    .AddQuote(ItemID.DarkSideHallow)
                    .AddQuote(ItemID.BerniePetItem)
                    .AddQuotesWithConditions(ItemID.MusicBoxCredits,
                    (() =>
                    {
                        int guide = NPC.FindFirstNPC(NPCID.Guide);
                        return guide != -1 ? Main.npc[guide].GivenName == "Andrew" : false;
                    }, "_GuideAndrew"))
                    .AddQuote(ItemID.SlimeStaff)
                    .AddQuote(ItemID.HeartLantern)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.FlaskofParty, () => new { PartyGirl = NPC.GetFirstNPCNameOrNull(NPCID.PartyGirl), },
                    (() => NPC.AnyNPCs(NPCID.PartyGirl), "_PartyGirl"))
                    .AddQuote(ItemID.SandstorminaBottle)
                    .AddQuote(ItemID.Terragrim)
                    .AddQuote(ItemID.PirateStaff)
                    .AddQuotesWithConditionsWithSubsitution(ItemID.DiscountCard, () => new { Exporter = NPC.GetFirstNPCNameOrNull(ModContent.NPCType<Exporter>()), },
                    (() => NPC.AnyNPCs(ModContent.NPCType<Exporter>()), "_Exporter"))
                    .AddQuotesWithConditionsWithSubsitution(ItemID.LuckyCoin, () => new { Exporter = NPC.GetFirstNPCNameOrNull(ModContent.NPCType<Exporter>()), },
                    (() => NPC.AnyNPCs(ModContent.NPCType<Exporter>()), "_Exporter"))
                    .AddQuote(ItemID.CoinGun)
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

            if (item.isAShopItem && item.buy && item.tooltipContext == ItemSlot.Context.ShopItem && Main.LocalPlayer.talkNPC != -1)
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
            var chatBubbleFrame = Images.StatusBubble.Value.Frame(horizontalFrames: Images.StatusBubbleFramesX, frameX: 2);
            var chatBubbleScale = baseScale * 0.9f;
            chatBubbleScale.X *= 1.1f;
            var chatBubblePosition = new Vector2(x + chatBubbleFrame.Width / 2f * chatBubbleScale.X, y + chatBubbleFrame.Height / 2f * chatBubbleScale.Y);

            Main.spriteBatch.Draw(Images.StatusBubble.Value, chatBubblePosition + new Vector2(2f) * chatBubbleScale,
                chatBubbleFrame, Color.Black * 0.4f, 0f, chatBubbleFrame.Size() / 2f, chatBubbleScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Images.StatusBubble.Value, chatBubblePosition,
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
            return $"Mods.{mod}.Chat.{AequusText.CreateSearchNameFromNPC(npc)}.ShopQuote.{AequusText.CreateKeyNameFromSearch(ItemID.Search.GetName(item))}";
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
                        if ((period + punctuation[j].Length) >= maxCharacters)
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