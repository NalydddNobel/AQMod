using Aequus.Buffs;
using Aequus.Buffs.Misc.Empowered;
using Aequus.Common.GlobalItems;
using Aequus.Common.ModPlayers;
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

        public static Dictionary<int, ItemDedication> Dedicated { get; private set; }
        public static readonly Dictionary<int, List<ITooltipModifier>> TooltipModifiers = new();

        public static void AddTooltipModifier(int itemType, ITooltipModifier modifier) {
            if (TooltipModifiers.TryGetValue(itemType, out var l)) {
                l.Add(modifier);
            }
            else {
                TooltipModifiers[itemType] = new() { modifier };
            }
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
            AddTooltipModifier(ItemID.Aglet, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.WormScarf, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.BandofStarpower, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MagicCuffs, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.CelestialCuffs, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ManaRegenerationBand, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.NaturesGift, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ManaFlower, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MagnetFlower, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ManaCloak, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ArcaneFlower, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MagicQuiver, new TooltipModifierEmpoweredStat() { ignoreLines = new[] { 1, } });
            AddTooltipModifier(ItemID.MoltenQuiver, new TooltipModifierEmpoweredStat() { ignoreLines = new[] { 1, } });
            AddTooltipModifier(ItemID.StalkersQuiver, new TooltipModifierEmpoweredStat() { ignoreLines = new[] { 1, } });
            AddTooltipModifier(ItemID.SharkToothNecklace, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.StingerNecklace, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.WarriorEmblem, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.RangerEmblem, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SorcererEmblem, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SummonerEmblem, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AvengerEmblem, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.DestroyerEmblem, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.LavaCharm, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MoltenCharm, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.LavaWaders, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AncientChisel, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AnglerEarring, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.Toolbox, new TooltipModifierEmpoweredStat());

            AddTooltipModifier(ItemID.JunglePants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.NecroGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MeteorLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MoltenGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.CobaltLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MythrilGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AdamantiteLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.HallowedGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.FrostLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.CrimsonGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AncientCobaltLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ChlorophyteGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.TikiPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.PalladiumLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.OrichalcumLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.TitaniumLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.TurtleLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SpectrePants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ShroomiteLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SpookyLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.BeetleLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.BeeGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SpiderGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.VortexLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.NebulaLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SolarFlareLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.FossilPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.StardustLeggings, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AncientBattleArmorPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SquireGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.SquireAltPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.HuntressPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.HuntressAltPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MonkPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.MonkAltPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ApprenticeTrousers, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.ApprenticeAltPants, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.AncientHallowedGreaves, new TooltipModifierEmpoweredStat());
            AddTooltipModifier(ItemID.CrystalNinjaLeggings, new TooltipModifierEmpoweredStat());

            AddTooltipModifier(ItemID.SunStone, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.MoonStone, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CelestialStone, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CelestialShell, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.MoonShell, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.MoonCharm, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.NeptunesShell, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.ShinyRedBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BalloonHorseshoeFart, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BalloonHorseshoeHoney, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BalloonHorseshoeSharkron, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BalloonPufferfish, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlizzardinaBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlueHorseshoeBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BundleofBalloons, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CloudinaBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.FartInABalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.HoneyBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.SandstorminaBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.SharkronBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.WhiteHorseshoeBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.YellowHorseshoeBalloon, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.TackleBox, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.ObsidianRose, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CrossNecklace, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.StarVeil, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CoinRing, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.GoldRing, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.GreedyRing, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.DiscountCard, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.LuckyCoin, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PaladinsShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.FrozenShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.AnkhShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CobaltShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.EoCShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.HeroShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.ObsidianShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.SquireShield, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PortableCementMixer, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.YoyoBag, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlackString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlueString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BrownString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CyanString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.GreenString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.LimeString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.OrangeString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PinkString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PurpleString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.RainbowString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.RedString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.SkyBlueString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.TealString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.VioletString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.WhiteString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.YellowString, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.YoYoGlove, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlackCounterweight, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlueCounterweight, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.GreenCounterweight, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PurpleCounterweight, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.RedCounterweight, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.YellowCounterweight, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PhilosophersStone, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.TreasureMagnet, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.AncientChisel, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CelestialMagnet, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.MagmaStone, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.Flipper, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.IceSkates, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PanicNecklace, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.FloatingTube, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.CloudinaBottle, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.TsunamiInABottle, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BlizzardinaBottle, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.SandstorminaBottle, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.FartinaJar, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.PortableStool, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BoneHelm, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.RoyalGel, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.HiveBackpack, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.StarCloak, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BeeCloak, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.AnkhCharm, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.GravityGlobe, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.SporeSac, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.ShinyStone, new TooltipModifierNoInteractions());
            AddTooltipModifier(ItemID.BrainOfConfusion, new TooltipModifierNoInteractions());
            //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
        }

        public void Unload_Tooltips()
        {
            TooltipModifiers.Clear();
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

        internal void Tooltip_DefenseStack(Item item, List<TooltipLine> tooltips) {

            if (equipEmpowerment == null) {
                return;
            }

            var color = equipEmpowerment.bonusColor ?? Color.White;

            if (equipEmpowerment.HasFlag(EquipEmpowermentParameters.Defense) && item.defense > 0)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Defense")
                    {
                        var text = tooltips[i].Text.Split(' ');
                        string number = text[0];
                        if (int.TryParse(number, out int numberValue))
                        {
                            text[0] = TextHelper.ColorCommand((numberValue * (equipEmpowerment.addedStacks + 1)).ToString(), color, alphaPulse: true);
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

                Tooltip_NameTag(item, tooltips);
                Tooltip_SummonerStaffUpgrade(item, tooltips, player, aequus);
                Tooltip_WeirdHints(item, tooltips);
                Tooltip_BuffConflicts(item, tooltips);
                Tooltip_PickBreak(item, tooltips);
                Tooltip_DefenseStack(item, tooltips);
                Tooltip_DefenseChange(item, tooltips);
                Tooltip_Price(item, tooltips, player, aequus);
                Tooltip_DedicatedItem(item, tooltips);
                ModifyTooltips_Prefixes(item, tooltips);
                if (TooltipModifiers.TryGetValue(item.type, out var statTooltips)) {

                    foreach (var ttModifier in statTooltips) {
                        ttModifier.ModifyTooltips(item, tooltips);
                    }
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
    public class TooltipModifierEmpoweredStat : ITooltipModifier {

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
            var empowerment = item.Aequus().equipEmpowerment;

            if (empowerment == null || !empowerment.HasFlag(EquipEmpowermentParameters.Abilities)) {
                return;
            }

            var color = empowerment.bonusColor ?? Color.White;

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
                            string newNumberText = (result * (1+empowerment.addedStacks)).ToString();
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
    public class TooltipModifierNoInteractions : ITooltipModifier {
        public void ModifyTooltips(Item item, List<TooltipLine> tooltips) {

            var empowerment = item.Aequus().equipEmpowerment;
            if (empowerment == null || !empowerment.HasFlag(EquipEmpowermentParameters.Abilities)) {
                return;
            }

            int index = tooltips.GetIndex("Tooltip");
            tooltips.Insert(index, new(
                Aequus.Instance, 
                "NoDoubledStats",
                TextHelper.GetTextValue("ItemTooltip.NoDoubledStats")) { 
                OverrideColor = empowerment.bonusColor ?? Color.White 
            });
        }
    }
}