using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Content.Players;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Effects;
using AQMod.Items.Accessories.FidgetSpinner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace AQMod.Items
{
    public sealed class TooltipText : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
            {
                if (item.modItem is IDedicatedItem dedicatedItem)
                {
                    tooltips.Add(new TooltipLine(mod, "DedicatedItem", Language.GetTextValue("Mods.AQMod.Tooltips.DedicatedItem")) { overrideColor = dedicatedItem.Color });
                }
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
                if (ModContent.GetInstance<AQConfigClient>().DemonSiegeUpgradeTooltip && DemonSiegeEvent.GetUpgrade(item) != null)
                {
                    int index = FindVanillaTooltipLineIndex(tooltips, "Material");
                    tooltips.Insert(index, new TooltipLine(mod, "DemonSiegeUpgrade", Language.GetTextValue("Mods.AQMod.Tooltips.DemonSiegeUpgrade")) { overrideColor = new Color(255, 222, 222), });
                }
                if (ModContent.GetInstance<AQConfigClient>().HookBarbBlacklistTooltip && item.shoot > ProjectileID.None && AQProjectile.Sets.HookBarbBlacklist.Contains(item.shoot))
                {
                    int index = FindVanillaTooltipLineIndex(tooltips, "Material");
                    tooltips.Insert(index, new TooltipLine(mod, "HookBarbBlacklist", Language.GetTextValue("Mods.AQMod.Tooltips.HookBarbBlacklist")) { overrideColor = new Color(255, 255, 255), });
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

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "AQMod" && line.Name == "DedicatedItem")
            {
                DrawDedicatedItemText(line);
                return false;
            }
            return true;
        }

        public static void DrawDedicatedItemText(DrawableTooltipLine line)
        {
            DrawDedicatedItemText(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale, line.overrideColor.GetValueOrDefault(line.color));
        }

        public static void DrawDedicatedItemText(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            color = Colors.AlphaDarken(color);
            color.A = 0;
            float xOff = (float)Math.Sin(Main.GlobalTime * 15f) + 1f;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color * 0.8f, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color * 0.8f, rotation, origin, baseScale);
        }

        public static void DrawNarrizuulText(DrawableTooltipLine line)
        {
            DrawNarrizuulText(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale, line.overrideColor.GetValueOrDefault(line.color));
        }

        public static void DrawNarrizuulText(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) // since you can rename items.
            {
                return;
            }
            var font = Main.fontMouseText;
            var size = font.MeasureString(text);
            var center = size / 2f;
            var transparentColor = color * 0.4f;
            transparentColor.A = 0;
            var texture = AQTextures.Lights[LightTex.Spotlight12x66];
            var spotlightOrigin = texture.Size() / 2f;
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightScale = new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime * 4f) * 0.145f, center.Y * 0.15f);

            // black BG
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, Color.Black * 0.3f, rotation,
            spotlightOrigin, new Vector2(size.X / texture.Width * 2f, center.Y / texture.Height * 2.5f), SpriteEffects.None, 0f);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black,
                rotation, origin, baseScale);

            GeneralEffectsManager.SetRand(Main.LocalPlayer.name.GetHashCode());
            // particles
            var particleTexture = AQTextures.Lights[LightTex.Spotlight15x15];
            var particleOrigin = particleTexture.Size() / 2f;
            int amt = (int)GeneralEffectsManager.Rand((int)size.X / 3, (int)size.X);
            for (int i = 0; i < amt; i++)
            {
                float lifeTime = (GeneralEffectsManager.Rand(20f) + Main.GlobalTime * 2f) % 20f;
                int baseParticleX = (int)GeneralEffectsManager.Rand(4, (int)size.X - 4);
                int particleX = baseParticleX + (int)AQUtils.Wave(lifeTime + Main.GlobalTime * GeneralEffectsManager.Rand(2f, 5f), -GeneralEffectsManager.Rand(3f, 10f), GeneralEffectsManager.Rand(3f, 10f));
                int particleY = (int)GeneralEffectsManager.Rand(10);
                float scale = GeneralEffectsManager.Rand(0.2f, 0.4f);
                if (baseParticleX > 14 && baseParticleX < size.X - 14 && GeneralEffectsManager.RandChance(6))
                {
                    scale *= 2f;
                }
                var clr = color;
                if (lifeTime < 0.3f)
                {
                    clr *= lifeTime / 0.3f;
                }
                if (lifeTime < 5f)
                {
                    if (lifeTime > MathHelper.PiOver2)
                    {
                        float timeMult = (lifeTime - MathHelper.PiOver2) / MathHelper.PiOver2;
                        scale -= timeMult * 0.4f;
                        if (scale < 0f)
                        {
                            continue;
                        }
                        int colorMinusAmount = (int)(timeMult * 255f);
                        clr.R = (byte)Math.Max(clr.R - colorMinusAmount, 0);
                        clr.G = (byte)Math.Max(clr.G - colorMinusAmount, 0);
                        clr.B = (byte)Math.Max(clr.B - colorMinusAmount, 0);
                        clr.A = (byte)Math.Max(clr.A - colorMinusAmount, 0);
                        if (clr.R == 0 && clr.G == 0 && clr.B == 0 && clr.A == 0)
                        {
                            continue;
                        }
                    }
                    if (scale > 0.4f)
                    {
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr * 2f, 0f, particleOrigin, scale * 0.5f, SpriteEffects.None, 0f);
                    }
                    Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr, 0f, particleOrigin, scale, SpriteEffects.None, 0f);
                }
            }

            // light effect
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.35f, spotlightRotation,
               spotlightOrigin, spotlightScale * 2f, SpriteEffects.None, 0f);

            // colored text
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y), color,
                rotation, origin, baseScale);

            // glowy effect on text
            float wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            for (int i = 1; i <= 2; i++)
            {
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x + wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x - wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
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

        internal static int FindVanillaTooltipLineIndex(List<TooltipLine> tooltips, string tooltipName)
        {
            switch (tooltipName)
            {
                case "Damage":
                    {
                        for (int i = tooltips.Count - 1; i >= 0; i--)
                        {
                            TooltipLine t = tooltips[i];
                            if (t.mod != "Terraria")
                                continue;
                            switch (t.Name)
                            {
                                case "Favorite":
                                case "FavoriteDesc":
                                case "Social":
                                case "SocialDesc":
                                case "Damage":
                                    return i + 1;
                            }
                        }
                    }
                    break;

                case "Material":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "WandConsumes":
                            case "Quest":
                            case "Vanity":
                            case "Defense":
                            case "PickPower":
                            case "AxePower":
                            case "HammerPower":
                            case "TileBoost":
                            case "HealLife":
                            case "HealMana":
                            case "UseMana":
                            case "Placeable":
                            case "Ammo":
                            case "Consumable":
                            case "Material":
                                return i + 1;
                        }
                    }
                    break;

                case "Tooltip#":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "WandConsumes":
                            case "Quest":
                            case "Vanity":
                            case "Defense":
                            case "PickPower":
                            case "AxePower":
                            case "HammerPower":
                            case "TileBoost":
                            case "HealLife":
                            case "HealMana":
                            case "UseMana":
                            case "Placeable":
                            case "Ammo":
                            case "Consumable":
                            case "Material":
                            case "Tooltip0":
                                return i + 1;
                        }
                    }
                    break;

                case "Equipable":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "Tooltip0":
                                return i + 1;
                        }
                    }
                    break;

                case "Expert":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "WandConsumes":
                            case "Quest":
                            case "Vanity":
                            case "Defense":
                            case "PickPower":
                            case "AxePower":
                            case "HammerPower":
                            case "TileBoost":
                            case "HealLife":
                            case "HealMana":
                            case "UseMana":
                            case "Placeable":
                            case "Ammo":
                            case "Consumable":
                            case "Material":
                            case "Tooltip0":
                            case "EtherianManaWarning":
                            case "WellFedExpert":
                            case "BuffTime":
                            case "OneDropLogo":
                            case "PrefixDamage":
                            case "PrefixSpeed":
                            case "PrefixCritChance":
                            case "PrefixUseMana":
                            case "PrefixSize":
                            case "PrefixShootSpeed":
                            case "PrefixKnockback":
                            case "PrefixAccDefense":
                            case "PrefixAccMaxMana":
                            case "PrefixAccCritChance":
                            case "PrefixAccDamage":
                            case "PrefixAccMoveSpeed":
                            case "PrefixAccMeleeSpeed":
                            case "SetBonus":
                            case "Expert":
                                return i + 1;
                        }
                    }
                    break;
            }
            return 1;
        }
    }
}