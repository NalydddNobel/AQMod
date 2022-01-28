using AQMod.Content.Players;
using AQMod.Items.Accessories.FidgetSpinner;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public sealed class TooltipText : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
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
                            t.text = item.pick / 2 + Language.GetTextValue("LegacyTooltip.26");
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
                            string text = UseTimeAnimationTooltip(PlayerHooks.TotalUseTime(item.useTime, Main.LocalPlayer, item));
                            AQPlayer.Fidget_Spinner_Force_Autoswing = false;
                            if (t.text != text)
                            {
                                t.text = text +
                                " (" + Lang.GetItemName(ModContent.ItemType<FidgetSpinner>()).Value + ")";
                                t.overrideColor = new Color(200, 200, 200, 255);
                            }
                        }
                    }
                }
                if (!PlayerStorage.hoverStorage.IsAir)
                {
                    var span = PlayerStorage.hoverStorage.TimeSinceStored;
                    int seconds = (int)span.TotalSeconds;
                    int minutes = seconds / 60;
                    int hours = minutes / 60;
                    int days = hours / 24;
                    int months = days / 30;
                    string text;
                    if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                    {
                        text = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)
                            ? Language.GetTextValue("Mods.AQMod.HermitCrab.Second" + (seconds == 1 ? "" : "s") + "Ago", seconds)
                            : Language.GetTextValue("Mods.AQMod.HermitCrab.StoredAt", PlayerStorage.hoverStorage.WhenStored);
                    }
                    else if (months > 0)
                    {
                        text = Language.GetTextValue("Mods.AQMod.HermitCrab.Month" + (months == 1 ? "" : "s") + "Ago", months);
                    }
                    else if (days > 0)
                    {
                        text = Language.GetTextValue("Mods.AQMod.HermitCrab.Day" + (days == 1 ? "" : "s") + "Ago", days);
                    }
                    else if (hours > 0)
                    {
                        text = Language.GetTextValue("Mods.AQMod.HermitCrab.Hour" + (hours == 1 ? "" : "s") + "Ago", hours);
                    }
                    else if (minutes > 0)
                    {
                        text = Language.GetTextValue("Mods.AQMod.HermitCrab.Minute" + (minutes == 1 ? "" : "s") + "Ago", minutes);
                    }
                    else
                    {
                        text = Language.GetTextValue("Mods.AQMod.HermitCrab.Second" + (seconds == 1 ? "" : "s") + "Ago", seconds);
                    }
                    tooltips.Add(new TooltipLine(mod, "StorageDate", text));
                    PlayerStorage.hoverStorage = new PlayerStorage.HermitCrabStorage();
                }
            }
            catch
            {
            }
        }

        internal static string UseTimeAnimationTooltip(float useAnimation)
        {
            if (useAnimation <= 8)
            {
                return Language.GetTextValue("LegacyTooltip.6");
            }
            else if (useAnimation <= 20)
            {
                return Language.GetTextValue("LegacyTooltip.7");
            }
            else if (useAnimation <= 25)
            {
                return Language.GetTextValue("LegacyTooltip.8");
            }
            else if (useAnimation <= 30)
            {
                return Language.GetTextValue("LegacyTooltip.9");
            }
            else if (useAnimation <= 35)
            {
                return Language.GetTextValue("LegacyTooltip.10");
            }
            else if (useAnimation <= 45)
            {
                return Language.GetTextValue("LegacyTooltip.11");
            }
            else if (useAnimation <= 55)
            {
                return Language.GetTextValue("LegacyTooltip.12");
            }
            return Language.GetTextValue("LegacyTooltip.13");
        }

        internal static string KnockbackItemTooltip(float knockback)
        {
            if (knockback == 0f)
            {
                return Language.GetTextValue("LegacyTooltip.14");
            }
            else if (knockback <= 1.5)
            {
                return Language.GetTextValue("LegacyTooltip.15");
            }
            else if (knockback <= 3f)
            {
                return Language.GetTextValue("LegacyTooltip.16");
            }
            else if (knockback <= 4f)
            {
                return Language.GetTextValue("LegacyTooltip.17");
            }
            else if (knockback <= 6f)
            {
                return Language.GetTextValue("LegacyTooltip.18");
            }
            else if (knockback <= 7f)
            {
                return Language.GetTextValue("LegacyTooltip.19");
            }
            else if (knockback <= 9f)
            {
                return Language.GetTextValue("LegacyTooltip.20");
            }
            else if (knockback <= 11f)
            {
                return Language.GetTextValue("LegacyTooltip.21");
            }
            return Language.GetTextValue("LegacyTooltip.22");
        }
    }
}