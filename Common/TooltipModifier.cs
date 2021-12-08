using AQMod.Items.Weapons.Umbrella;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AQMod.Items
{
    public class TooltipModifier : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.type > Main.maxItemTypes)
            {
                if (item.modItem is IUmbrellaDamage)
                {
                    if (line.mod == "Terraria")
                    {
                        string umbrellaText = "";
                        if (line.Name == "Damage")
                        {
                            umbrellaText = Language.GetTextValue("Mods.AQMod.Tooltips.UmbrellaDamage", Main.LocalPlayer.GetWeaponDamage(item));
                        }
                        else if (line.Name == "UseMana")
                        {
                            umbrellaText = Language.GetTextValue("Mods.AQMod.Tooltips.UmbrellaMana", Main.LocalPlayer.GetManaCost(item));
                        }
                        if (umbrellaText != "")
                        {
                            var color = Colors.AlphaDarken(line.overrideColor.GetValueOrDefault(line.color));
                            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, Main.fontMouseText, umbrellaText,
                                new Vector2(line.X, line.Y), color, line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);
                            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, umbrellaText,
                                new Vector2(line.X, line.Y), Color.Black, line.rotation, line.origin, line.baseScale, ignoreColors: false);
                            return false;
                        }
                    }
                }
            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var aQPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
            if (item.type > Main.maxItemTypes)
            {
                if (item.modItem is IManaPerSecond m)
                {
                    int? value = m.ManaPerSecond;
                    int manaCost;
                    if (value != null)
                    {
                        manaCost = value.Value;
                    }
                    else
                    {
                        manaCost = Main.player[Main.myPlayer].GetManaCost(item);
                        int usesPerSecond = 60 / PlayerHooks.TotalUseTime(item.useTime, Main.player[Main.myPlayer], item);
                        if (usesPerSecond != 0)
                        {
                            manaCost *= usesPerSecond;
                        }
                    }
                    foreach (var t in tooltips)
                    {
                        if (t.mod == "Terraria" && t.Name == "UseMana")
                        {
                            t.text = string.Format(Language.GetTextValue("Mods.AQMod.Tooltips.ManaPerSecond"), manaCost);
                        }
                    }
                }
            }
            if (item.pick > 0 && aQPlayer.pickBreak)
            {
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "PickPower")
                    {
                        t.text = item.pick / 2 + Lang.tip[26].Value;
                        t.overrideColor = new Color(128, 128, 128, 255);
                    }
                }
            }
            if (aQPlayer.fidgetSpinner && !item.channel && AQPlayer.CanForceAutoswing(Main.LocalPlayer, item, ignoreChanneled: true))
            {
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "Speed")
                    {
                        AQPlayer.Fidget_Spinner_Force_Autoswing = true;
                        string text = AQUtils.UseTimeAnimationTooltip(PlayerHooks.TotalUseTime(item.useTime, Main.LocalPlayer, item));
                        AQPlayer.Fidget_Spinner_Force_Autoswing = false;
                        if (t.text != text)
                        {
                            t.text = text +
                            " (" + Lang.GetItemName(ModContent.ItemType<Items.Accessories.FidgetSpinner.FidgetSpinner>()).Value + ")";
                            t.overrideColor = new Color(200, 200, 200, 255);
                        }
                    }
                }
            }
        }
    }
}