using Aequus.Buffs;
using Aequus.Buffs.Misc.Empowered;
using Aequus.Common.GlobalItems;
using Aequus.Content.ItemRarities;
using Aequus.Content.Town.ExporterNPC;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public static Color HintColor => new Color(225, 100, 255, 255);
        public static Color CrownOfBloodColor => new(255, 128, 140, 255);
        public static Color NormalColor => new(140, 255, 128, 255);


        public static Dictionary<int, ItemDedication> Dedicated { get; private set; }
        public static Dictionary<int, ITooltipModifier> TooltipModifiers { get; private set; }


        public Color GetMultipliedStatColor(Item item, int stacks)
        {
            if (crownOfBloodUsed)
            {
                return CrownOfBloodColor;
            }
            return NormalColor;
        }

        public static void GenericMultipliedStatTooltip(Item item, List<TooltipLine> tt, int stacks, Color color)
        {
            foreach (var t in tt)
            {
                if (t.Name.StartsWith("Tooltip"))
                {
                    string numberExtract = string.Empty;
                    for (int i = 0; i < t.Text.Length + 1; i++)
                    {
                        if (i < t.Text.Length && char.IsNumber(t.Text[i]))
                        {
                            char current = t.Text[i];
                            numberExtract += current;
                        }
                        else if (numberExtract != string.Empty)
                        {
                            if (!int.TryParse(numberExtract, out int result))
                            {
                                numberExtract = string.Empty;
                                continue;
                            }

                            int numberStart = i - numberExtract.Length;
                            string preNumberText = t.Text.Substring(0, numberStart);
                            string postNumberText = "";
                            if (i < t.Text.Length)
                            {
                                postNumberText = t.Text.Substring(numberStart + numberExtract.Length);
                            }
                            string newNumberText = (result * stacks).ToString();
                            newNumberText = TextHelper.ColorCommand(newNumberText, color, alphaPulse: true);
                            i = numberStart + newNumberText.Length;
                            t.Text = preNumberText + newNumberText + postNumberText;
                            numberExtract = string.Empty;
                        }
                    }
                }
            }
        }

        public void Load_Tooltips()
        {
            Dedicated = new Dictionary<int, ItemDedication>();
            TooltipModifiers = new()
            {
                [ItemID.Aglet] = new EmpoweredStatTooltipModifer(),
                [ItemID.WormScarf] = new EmpoweredStatTooltipModifer(),
                [ItemID.BandofStarpower] = new EmpoweredStatTooltipModifer(),
                [ItemID.MagicCuffs] = new EmpoweredStatTooltipModifer(),
                [ItemID.CelestialCuffs] = new EmpoweredStatTooltipModifer(),
                [ItemID.ManaRegenerationBand] = new EmpoweredStatTooltipModifer(),
                [ItemID.NaturesGift] = new EmpoweredStatTooltipModifer(),
                [ItemID.ManaFlower] = new EmpoweredStatTooltipModifer(),
                [ItemID.MagnetFlower] = new EmpoweredStatTooltipModifer(),
                [ItemID.ManaCloak] = new EmpoweredStatTooltipModifer(),
                [ItemID.ArcaneFlower] = new EmpoweredStatTooltipModifer(),
                [ItemID.MagicQuiver] = new EmpoweredStatTooltipModifer() { ignoreLines = new[] { 1, } },
                [ItemID.MoltenQuiver] = new EmpoweredStatTooltipModifer() { ignoreLines = new[] { 1, } },
                [ItemID.StalkersQuiver] = new EmpoweredStatTooltipModifer() { ignoreLines = new[] { 1, } },
                [ItemID.SharkToothNecklace] = new EmpoweredStatTooltipModifer(),
                [ItemID.StingerNecklace] = new EmpoweredStatTooltipModifer(),
                [ItemID.WarriorEmblem] = new EmpoweredStatTooltipModifer(),
                [ItemID.RangerEmblem] = new EmpoweredStatTooltipModifer(),
                [ItemID.SorcererEmblem] = new EmpoweredStatTooltipModifer(),
                [ItemID.SummonerEmblem] = new EmpoweredStatTooltipModifer(),
                [ItemID.AvengerEmblem] = new EmpoweredStatTooltipModifer(),
                [ItemID.DestroyerEmblem] = new EmpoweredStatTooltipModifer(),
                [ItemID.LavaCharm] = new EmpoweredStatTooltipModifer(),
                [ItemID.MoltenCharm] = new EmpoweredStatTooltipModifer(),
                [ItemID.LavaWaders] = new EmpoweredStatTooltipModifer(),
                [ItemID.AncientChisel] = new EmpoweredStatTooltipModifer(),
                [ItemID.AnglerEarring] = new EmpoweredStatTooltipModifer(),
                [ItemID.Toolbox] = new EmpoweredStatTooltipModifer(),

                [ItemID.JunglePants] = new EmpoweredStatTooltipModifer(),
                [ItemID.NecroGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.MeteorLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.MoltenGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.CobaltLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.MythrilGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.AdamantiteLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.HallowedGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.FrostLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.CrimsonGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.AncientCobaltLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.ChlorophyteGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.TikiPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.PalladiumLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.OrichalcumLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.TitaniumLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.TurtleLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.SpectrePants] = new EmpoweredStatTooltipModifer(),
                [ItemID.ShroomiteLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.SpookyLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.BeetleLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.BeeGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.SpiderGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.VortexLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.NebulaLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.SolarFlareLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.FossilPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.StardustLeggings] = new EmpoweredStatTooltipModifer(),
                [ItemID.AncientBattleArmorPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.SquireGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.SquireAltPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.HuntressPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.HuntressAltPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.MonkPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.MonkAltPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.ApprenticeTrousers] = new EmpoweredStatTooltipModifer(),
                [ItemID.ApprenticeAltPants] = new EmpoweredStatTooltipModifer(),
                [ItemID.AncientHallowedGreaves] = new EmpoweredStatTooltipModifer(),
                [ItemID.CrystalNinjaLeggings] = new EmpoweredStatTooltipModifer(),

                [ItemID.SunStone] = new NoInteractionsTooltipModifier(),
                [ItemID.MoonStone] = new NoInteractionsTooltipModifier(),
                [ItemID.CelestialStone] = new NoInteractionsTooltipModifier(),
                [ItemID.CelestialShell] = new NoInteractionsTooltipModifier(),
                [ItemID.MoonShell] = new NoInteractionsTooltipModifier(),
                [ItemID.MoonCharm] = new NoInteractionsTooltipModifier(),
                [ItemID.NeptunesShell] = new NoInteractionsTooltipModifier(),
                [ItemID.ShinyRedBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.BalloonHorseshoeFart] = new NoInteractionsTooltipModifier(),
                [ItemID.BalloonHorseshoeHoney] = new NoInteractionsTooltipModifier(),
                [ItemID.BalloonHorseshoeSharkron] = new NoInteractionsTooltipModifier(),
                [ItemID.BalloonPufferfish] = new NoInteractionsTooltipModifier(),
                [ItemID.BlizzardinaBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.BlueHorseshoeBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.BundleofBalloons] = new NoInteractionsTooltipModifier(),
                [ItemID.CloudinaBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.FartInABalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.HoneyBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.SandstorminaBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.SharkronBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.WhiteHorseshoeBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.YellowHorseshoeBalloon] = new NoInteractionsTooltipModifier(),
                [ItemID.TackleBox] = new NoInteractionsTooltipModifier(),
                [ItemID.ObsidianRose] = new NoInteractionsTooltipModifier(),
                [ItemID.CrossNecklace] = new NoInteractionsTooltipModifier(),
                [ItemID.StarVeil] = new NoInteractionsTooltipModifier(),
                [ItemID.CoinRing] = new NoInteractionsTooltipModifier(),
                [ItemID.GoldRing] = new NoInteractionsTooltipModifier(),
                [ItemID.GreedyRing] = new NoInteractionsTooltipModifier(),
                [ItemID.DiscountCard] = new NoInteractionsTooltipModifier(),
                [ItemID.LuckyCoin] = new NoInteractionsTooltipModifier(),
                [ItemID.PaladinsShield] = new NoInteractionsTooltipModifier(),
                [ItemID.FrozenShield] = new NoInteractionsTooltipModifier(),
                [ItemID.AnkhShield] = new NoInteractionsTooltipModifier(),
                [ItemID.CobaltShield] = new NoInteractionsTooltipModifier(),
                [ItemID.EoCShield] = new NoInteractionsTooltipModifier(),
                [ItemID.HeroShield] = new NoInteractionsTooltipModifier(),
                [ItemID.ObsidianShield] = new NoInteractionsTooltipModifier(),
                [ItemID.SquireShield] = new NoInteractionsTooltipModifier(),
                [ItemID.PortableCementMixer] = new NoInteractionsTooltipModifier(),
                [ItemID.YoyoBag] = new NoInteractionsTooltipModifier(),
                [ItemID.BlackString] = new NoInteractionsTooltipModifier(),
                [ItemID.BlueString] = new NoInteractionsTooltipModifier(),
                [ItemID.BrownString] = new NoInteractionsTooltipModifier(),
                [ItemID.CyanString] = new NoInteractionsTooltipModifier(),
                [ItemID.GreenString] = new NoInteractionsTooltipModifier(),
                [ItemID.LimeString] = new NoInteractionsTooltipModifier(),
                [ItemID.OrangeString] = new NoInteractionsTooltipModifier(),
                [ItemID.PinkString] = new NoInteractionsTooltipModifier(),
                [ItemID.PurpleString] = new NoInteractionsTooltipModifier(),
                [ItemID.RainbowString] = new NoInteractionsTooltipModifier(),
                [ItemID.RedString] = new NoInteractionsTooltipModifier(),
                [ItemID.SkyBlueString] = new NoInteractionsTooltipModifier(),
                [ItemID.TealString] = new NoInteractionsTooltipModifier(),
                [ItemID.VioletString] = new NoInteractionsTooltipModifier(),
                [ItemID.WhiteString] = new NoInteractionsTooltipModifier(),
                [ItemID.YellowString] = new NoInteractionsTooltipModifier(),
                [ItemID.YoYoGlove] = new NoInteractionsTooltipModifier(),
                [ItemID.BlackCounterweight] = new NoInteractionsTooltipModifier(),
                [ItemID.BlueCounterweight] = new NoInteractionsTooltipModifier(),
                [ItemID.GreenCounterweight] = new NoInteractionsTooltipModifier(),
                [ItemID.PurpleCounterweight] = new NoInteractionsTooltipModifier(),
                [ItemID.RedCounterweight] = new NoInteractionsTooltipModifier(),
                [ItemID.YellowCounterweight] = new NoInteractionsTooltipModifier(),
                [ItemID.PhilosophersStone] = new NoInteractionsTooltipModifier(),
                [ItemID.TreasureMagnet] = new NoInteractionsTooltipModifier(),
                [ItemID.AncientChisel] = new NoInteractionsTooltipModifier(),
                [ItemID.CelestialMagnet] = new NoInteractionsTooltipModifier(),
                [ItemID.MagmaStone] = new NoInteractionsTooltipModifier(),
                [ItemID.Flipper] = new NoInteractionsTooltipModifier(),
                [ItemID.IceSkates] = new NoInteractionsTooltipModifier(),
                [ItemID.PanicNecklace] = new NoInteractionsTooltipModifier(),
                [ItemID.FloatingTube] = new NoInteractionsTooltipModifier(),
                [ItemID.CloudinaBottle] = new NoInteractionsTooltipModifier(),
                [ItemID.TsunamiInABottle] = new NoInteractionsTooltipModifier(),
                [ItemID.BlizzardinaBottle] = new NoInteractionsTooltipModifier(),
                [ItemID.SandstorminaBottle] = new NoInteractionsTooltipModifier(),
                [ItemID.FartinaJar] = new NoInteractionsTooltipModifier(),
                [ItemID.PortableStool] = new NoInteractionsTooltipModifier(),
                [ItemID.BoneHelm] = new NoInteractionsTooltipModifier(),
                [ItemID.RoyalGel] = new NoInteractionsTooltipModifier(),
                [ItemID.HiveBackpack] = new NoInteractionsTooltipModifier(),
                [ItemID.StarCloak] = new NoInteractionsTooltipModifier(),
                [ItemID.BeeCloak] = new NoInteractionsTooltipModifier(),
                [ItemID.AnkhCharm] = new NoInteractionsTooltipModifier(),
                [ItemID.GravityGlobe] = new NoInteractionsTooltipModifier(),
                [ItemID.SporeSac] = new NoInteractionsTooltipModifier(),
                [ItemID.ShinyStone] = new NoInteractionsTooltipModifier(),
                [ItemID.BrainOfConfusion] = new NoInteractionsTooltipModifier(),
            };
            //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
        }

        public void Unload_Tooltips()
        {
            Dedicated?.Clear();
            Dedicated = null;
        }

        internal void Tooltip_DedicatedItem(Item item, List<TooltipLine> tooltips)
        {
            if (Dedicated.TryGetValue(item.type, out var dedication))
            {
                tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new TooltipLine(Mod, "DedicatedItem", TextHelper.GetTextValue("ItemTooltip.Common.DedicatedItem")) { OverrideColor = dedication.color() });
            }
        }

        internal void Tooltip_SummonerStaffUpgrade(Item item, List<TooltipLine> tooltips, Player player, AequusPlayer aequus)
        {
            if (aequus.moroSummonerFruit && SummonStaff.Contains(item.type))
            {
                tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "UseMana");
            }
        }

        internal void Tooltip_ExporterDoubloons(Item item, List<TooltipLine> tooltips, NPC chatNPC)
        {
            if (chatNPC.type == ModContent.NPCType<Exporter>())
                ModifyPriceTooltip(item, tooltips, "Chat.Exporter");
        }

        internal void Tooltip_Price(Item item, List<TooltipLine> tooltips, Player player, AequusPlayer aequus)
        {
            if (Main.npcShop > 0)
            {
                if (player.talkNPC != -1 && item.isAShopItem && item.buy && item.tooltipContext == ItemSlot.Context.ShopItem)
                {
                    Tooltip_ExporterDoubloons(item, tooltips, Main.npc[player.talkNPC]);
                }
            }
            else if (aequus.accPriceMonocle)
            {
                if (item.value >= 0 && !item.IsACoin && tooltips.Find((t) => t.Name == "Price" || t.Name == "SpecialPrice") == null)
                {
                    AddPriceTooltip(player, item, tooltips);
                }
            }
        }

        internal void Tooltip_WeirdHints(Item item, List<TooltipLine> tooltips)
        {
            if (LegendaryFishIDs.Contains(item.type))
            {
                if (NPC.AnyNPCs(NPCID.Angler))
                    tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), new TooltipLine(Mod, "AnglerHint", TextHelper.GetTextValue("ItemTooltip.Misc.AnglerHint")) { OverrideColor = HintColor, });
                tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Quest");
            }
        }

        internal void Tooltip_BuffConflicts(Item item, List<TooltipLine> tooltips)
        {
            int originalBuffType = EmpoweredBuffBase.GetDepoweredBuff(item.buffType);
            if (originalBuffType > 0 && AequusBuff.PotionConflicts.TryGetValue(originalBuffType, out var l) && l != null && l.Count > 0)
            {
                string text = "";
                if (l.Count == 1)
                {
                    text = TextHelper.GetTextValueWith("ItemTooltip.Common.NewPotionsBalancing", new { PotionName = Lang.GetBuffName(l[0]), });
                }
                else
                {
                    for (int i = 0; i < l.Count - 1; i++)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            text += ", ";
                        }
                        text += Lang.GetBuffName(l[i]);
                    }
                    text = TextHelper.GetTextValueWith("ItemTooltip.Common.NewPotionsBalancing2", new { PotionName = text, PotionName2 = Lang.GetBuffName(l[^1]), });
                }
                tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), new TooltipLine(Mod, "PotionConflict", text));
            }
        }

        internal void Tooltip_PickBreak(Item item, List<TooltipLine> tooltips)
        {
            if (item.pick > 0)
            {
                float pickDamage = Math.Max(Main.LocalPlayer.Aequus().pickTileDamage, 0f);
                if (item.pick != (int)(item.pick * pickDamage))
                {
                    foreach (var t in tooltips)
                    {
                        if (t.Mod == "Terraria" && t.Name == "PickPower")
                        {
                            string sign = "-";
                            var color = new Color(190, 120, 120, 255);
                            if (pickDamage > 1f)
                            {
                                sign = "+";
                                color = new Color(120, 190, 120, 255);
                            }
                            t.Text = $"{item.pick}{TextHelper.ColorCommand($"({sign}{(int)Math.Abs(item.pick * (1f - pickDamage))})", color, alphaPulse: true)}{Language.GetTextValue("LegacyTooltip.26")}";
                        }
                    }
                }
            }
        }

        internal void Tooltip_DefenseChange(Item item, List<TooltipLine> tooltips)
        {
            if (defenseChange != 0)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Defense")
                    {
                        if (defenseChange == -item.defense)
                        {
                            tooltips.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (defenseChange <= -item.defense)
                        {
                            tooltips[i].Text = $"-{tooltips[i].Text}";
                            break;
                        }
                        var text = tooltips[i].Text.Split(' ');
                        text[0] += defenseChange > 0 ?
                            TextHelper.ColorCommand($"(+{defenseChange})", TextHelper.PrefixGood, alphaPulse: true) :
                            TextHelper.ColorCommand($"({defenseChange})", TextHelper.PrefixBad, alphaPulse: true);
                        tooltips[i].Text = string.Join(' ', text);
                        break;
                    }
                }
            }
        }

        internal void Tooltip_AccStack(Item item, List<TooltipLine> tooltips) {

            var color = item.Aequus().GetMultipliedStatColor(item, accStacks);

            if (accStacks <= 1) {
                return;
            }

            if (item.defense > 0)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Defense")
                    {
                        var text = tooltips[i].Text.Split(' ');
                        string number = text[0];
                        if (int.TryParse(number, out int numberValue))
                        {
                            text[0] = TextHelper.ColorCommand((numberValue * accStacks).ToString(), color, alphaPulse: true);
                            tooltips[i].Text = string.Join(' ', text);
                        }
                        break;
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
            {
                var player = Main.LocalPlayer;
                var aequus = player.Aequus();

                Load_Tooltips();
                Tooltip_NameTag(item, tooltips);
                Tooltip_SummonerStaffUpgrade(item, tooltips, player, aequus);
                Tooltip_WeirdHints(item, tooltips);
                Tooltip_BuffConflicts(item, tooltips);
                Tooltip_PickBreak(item, tooltips);
                Tooltip_AccStack(item, tooltips);
                Tooltip_DefenseChange(item, tooltips);
                Tooltip_Price(item, tooltips, player, aequus);
                Tooltip_DedicatedItem(item, tooltips);
                ModifyTooltips_Prefixes(item, tooltips);
                if (TooltipModifiers.TryGetValue(item.type, out var statTooltips)) {

                    statTooltips.ModifyTooltips(item, tooltips);
                }
            }
            catch
            {
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus")
            {
                if (line.Name.StartsWith("Fake"))
                {
                    return false;
                }
                if (line.Name == "DedicatedItem")
                {
                    DrawDedicatedTooltip(line);
                    return false;
                }
            }
            else if (line.Mod == "Terraria")
            {
                if (line.Name == "ItemName" && item.rare >= ItemRarityID.Count && RarityLoader.GetRarity(item.rare) is IDrawRarity drawRare)
                {
                    drawRare.DrawTooltipLine(line);
                    return false;
                }
            }
            return true;
        }

        #region Static Methods
        private static void TestLootBagTooltip(Item item, List<TooltipLine> tooltips)
        {
            var dropTable = Helper.GetListOfDrops(Main.ItemDropsDB.GetRulesForItemID(item.type));

            for (int i = 0; i < dropTable.Count; i++)
            {
                tooltips.Add(new TooltipLine(Aequus.Instance, $"Drop{i}", dropTable[i]));
            }
        }
        private static void DebugEnemyDrops(int npcID, List<TooltipLine> tooltips)
        {
            var dropTable = Helper.GetListOfDrops(Main.ItemDropsDB.GetRulesForNPCID(npcID, includeGlobalDrops: false));

            for (int i = 0; i < dropTable.Count; i++)
            {
                tooltips.Add(new TooltipLine(Aequus.Instance, $"Drop{i}", dropTable[i]));
            }
        }
        public TooltipLine GetPriceTooltipLine(Player player, Item item)
        {
            player.GetItemExpectedPrice(item, out var calcForSelling, out var calcForBuying);
            int value = item.isAShopItem || item.buyOnce ? calcForBuying : calcForSelling;
            if (item.shopSpecialCurrency != -1)
            {
                string[] text = new string[1];
                int line = 0;
                CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, text, ref line, value);
                return new TooltipLine(Mod, "SpecialPrice", text[0]) { OverrideColor = Color.White, };
            }
            else if (value > 0)
            {
                string text = "";
                long platinum = 0;
                long gold = 0;
                long silver = 0;
                long copper = 0;
                long itemValue = value * item.stack;
                if (!item.buy)
                {
                    itemValue = value / 5;
                    if (itemValue < 1)
                    {
                        itemValue = 1;
                    }
                    long num3 = itemValue;
                    itemValue *= item.stack;
                    int amount = Main.shopSellbackHelper.GetAmount(item);
                    if (amount > 0)
                    {
                        itemValue += (-num3 + calcForBuying) * Math.Min(amount, item.stack);
                    }
                }
                if (itemValue < 1)
                {
                    itemValue = 1;
                }
                if (itemValue >= 1000000)
                {
                    platinum = itemValue / 1000000;
                    itemValue -= platinum * 1000000;
                }
                if (itemValue >= 10000)
                {
                    gold = itemValue / 10000;
                    itemValue -= gold * 10000;
                }
                if (itemValue >= 100)
                {
                    silver = itemValue / 100;
                    itemValue -= silver * 100;
                }
                if (itemValue >= 1)
                {
                    copper = itemValue;
                }

                if (platinum > 0)
                {
                    text = text + platinum + " " + Lang.inter[15].Value + " ";
                }
                if (gold > 0)
                {
                    text = text + gold + " " + Lang.inter[16].Value + " ";
                }
                if (silver > 0)
                {
                    text = text + silver + " " + Lang.inter[17].Value + " ";
                }
                if (copper > 0)
                {
                    text = text + copper + " " + Lang.inter[18].Value + " ";
                }

                var t = new TooltipLine(Mod, "Price", Lang.tip[item.buy ? 50 : 49].Value + " " + text);

                if (platinum > 0)
                {
                    t.OverrideColor = Colors.CoinPlatinum;
                }
                else if (gold > 0)
                {
                    t.OverrideColor = Colors.CoinGold;
                }
                else if (silver > 0)
                {
                    t.OverrideColor = Colors.CoinSilver;
                }
                else if (copper > 0)
                {
                    t.OverrideColor = Colors.CoinCopper;
                }
                return t;
            }
            else if (item.type != ItemID.DefenderMedal)
            {
                return new TooltipLine(Mod, "Price", Lang.tip[51].Value) { OverrideColor = new Color(120, 120, 120, 255) };
            }
            return null;
        }
        public void AddPriceTooltip(Player player, Item item, List<TooltipLine> tooltips)
        {
            var tt = GetPriceTooltipLine(player, item);
            if (tt != null)
            {
                tooltips.Add(tt);
            }
        }
        public bool ModifyPriceTooltip(Item item, List<TooltipLine> lines, string key)
        {
            var t = lines.Find("Price");
            if (t != null)
            {
                t.Text = t.Text.Replace(Lang.inter[15].Value, TextHelper.GetTextValue(key + ".ShopPrice.Platinum"));
                t.Text = t.Text.Replace(Lang.inter[16].Value, TextHelper.GetTextValue(key + ".ShopPrice.Gold"));
                t.Text = t.Text.Replace(Lang.inter[17].Value, TextHelper.GetTextValue(key + ".ShopPrice.Silver"));
                t.Text = t.Text.Replace(Lang.inter[18].Value, TextHelper.GetTextValue(key + ".ShopPrice.Copper"));
            }
            return false;
        }
        public void FitTooltipBackground(List<TooltipLine> lines, int width, int height, int index = -1, string firstBoxName = "Fake")
        {
            var font = FontAssets.MouseText.Value;
            var measurement = font.MeasureString(Helper.AirCharacter.ToString());
            string t = "";
            var stringSize = Vector2.Zero;
            for (int i = 0; i < width; i++)
            {
                t += Helper.AirCharacter;
                stringSize = ChatManager.GetStringSize(font, t, Vector2.One);
                if (stringSize.X > width)
                {
                    break;
                }
            }

            if (index == -1)
            {
                index = lines.Count - 1;
            }

            int linesY = Math.Max((int)(height / stringSize.Y), 1);
            for (int i = 0; i < linesY; i++)
            {
                lines.Insert(index, new TooltipLine(Mod, "Fake_" + i, t));
            }
            lines.Insert(index, new TooltipLine(Mod, firstBoxName, t));
        }

        internal static void PercentageModifier(float value, string key, List<TooltipLine> tooltips, bool good)
        {
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, key, TextHelper.GetTextValue("Prefixes." + key, (value > 0f ? "+" : "") + (int)(value * 100f) + "%"))
            { IsModifier = true, IsModifierBad = !good, });
        }
        internal static void PercentageModifier(int num, int originalNum, string key, List<TooltipLine> tooltips, bool higherIsGood = false)
        {
            if (num == originalNum)
            {
                return;
            }

            float value = num / (float)originalNum;
            if (value < 1f)
            {
                value = 1f - value;
            }
            else
            {
                value--;
            }
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, key, TextHelper.GetTextValue("Prefixes." + key, (num > originalNum ? "+" : "-") + (int)(value * 100f) + "%"))
            { IsModifier = true, IsModifierBad = num < originalNum ? higherIsGood : !higherIsGood, });
        }
        #endregion

        #region Dedicated Tooltip Drawing
        public static void DrawDedicatedTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            float brightness = Main.mouseTextColor / 255f;
            float brightnessProgress = (Main.mouseTextColor - 190f) / (byte.MaxValue - 190f);
            color = Colors.AlphaDarken(color);
            color.A = 0;
            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, coords, new Color(0, 0, 0, 255), rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y) + f.ToRotationVector2() * (brightness / 2f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.8f, rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
            {
                var coords = new Vector2(x, y) + (f + Main.GlobalTimeWrappedHourly).ToRotationVector2() * (brightnessProgress * 3f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.2f, rotation, origin, baseScale);
            }
        }
        public static void DrawDedicatedTooltip(string text, int x, int y, Color color)
        {
            DrawDedicatedTooltip(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
        }
        public static void DrawDedicatedTooltip(DrawableTooltipLine line)
        {
            DrawDedicatedTooltip(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
        }
        #endregion
    }

    public interface ITooltipModifier {
        void ModifyTooltips(Item item, List<TooltipLine> tooltips);
    }
    public class EmpoweredStatTooltipModifer : ITooltipModifier {

        public int[] ignoreLines;

        public virtual bool CanAdjustTooltip(Item item, List<TooltipLine> tooltips, TooltipLine currentTooltip) {
            if (currentTooltip.Name.StartsWith("Tooltip")) {
                if (ignoreLines == null) {
                    return true;
                }

                int compare = -1;
                // Tooltip is 7 characters long, so we want all characters after "Tooltip" to check for a number
                string subString = currentTooltip.Name[7..];
                if (int.TryParse(subString, out int result)) {
                    compare = result;
                }
                return !ignoreLines.ContainsAny(num => num == compare);
            }
            return false;
        }

        public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            int stacks = item.Aequus().accStacks;

            if (stacks <= 1) {
                return;
            }

            var color = item.Aequus().GetMultipliedStatColor(item, stacks);

            foreach (var t in tooltips) {
                if (CanAdjustTooltip(item, tooltips, t)) {
                    string numberExtract = string.Empty;
                    for (int i = 0; i < t.Text.Length + 1; i++) {
                        if (i < t.Text.Length && char.IsNumber(t.Text[i])) {
                            char current = t.Text[i];
                            numberExtract += current;
                        }
                        else if (numberExtract != string.Empty) {
                            if (!int.TryParse(numberExtract, out int result)) {
                                numberExtract = string.Empty;
                                continue;
                            }

                            int numberStart = i - numberExtract.Length;
                            string preNumberText = t.Text.Substring(0, numberStart);
                            string postNumberText = "";
                            if (i < t.Text.Length) {
                                postNumberText = t.Text.Substring(numberStart + numberExtract.Length);
                            }
                            string newNumberText = (result * stacks).ToString();
                            newNumberText = TextHelper.ColorCommand(newNumberText, color, alphaPulse: true);
                            i = numberStart + newNumberText.Length;
                            t.Text = preNumberText + newNumberText + postNumberText;
                            numberExtract = string.Empty;
                        }
                    }
                }
            }
        }
    }
    public class NoInteractionsTooltipModifier : ITooltipModifier {
        public void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            int index = tooltips.GetIndex("Tooltip");
            var aequus = Main.LocalPlayer.Aequus();

            if (aequus.empoweredLegs && item.legSlot > 0) {
                tooltips.Insert(index, new(Aequus.Instance, "NoDoubledStats",
                    TextHelper.GetTextValue("ItemTooltip.NoDoubledStats")) { OverrideColor = AequusItem.NormalColor });
            }
            if (aequus.accBloodCrownSlot > 2 && item.accessory) {
                tooltips.Insert(index, new(Aequus.Instance, "NoCrownOfBlood",
                    TextHelper.GetTextValue("ItemTooltip.CrownOfBlood.NoDoubledStats")) { OverrideColor = AequusItem.CrownOfBloodColor });
            }
        }
    }
}