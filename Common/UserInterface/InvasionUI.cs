using AQMod.Assets;
using AQMod.Common.Utilities;
using AQMod.Content.WorldEvents.DemonicEvent;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;

namespace AQMod.Common.UserInterface
{
    internal static class InvasionUI
    {
        public const byte GlimmerEventInvasion = 0;
        public const byte DemonSiegeInvasion = 1;
        public static byte InvasionType { get; set; } = 255;

        public static float invasionProgressAlpha = 0;
        public static int lastInvasionUIIndex = 0;
        public static float invasionProgressMax = 0f;
        public static float invasionProgress = 0f;

        public static void Apply()
        {
            if (Main.invasionProgressAlpha > 0f && invasionProgressAlpha <= 0f)
                return;
            Main.invasionProgressAlpha = 0f;
            byte currentInvasionType = 255;
            if (DemonSiege.CloseEnoughToDemonSiege(Main.player[Main.myPlayer]))
            {
                currentInvasionType = DemonSiegeInvasion;
                InvasionType = DemonSiegeInvasion;
            }
            else if (AQMod.CosmicEvent.CanShowInvasionProgress())
            {
                currentInvasionType = GlimmerEventInvasion;
                InvasionType = GlimmerEventInvasion;
            }
            else if (invasionProgressAlpha <= 0f)
            {
                InvasionType = byte.MaxValue;
            }
            if (InvasionType != byte.MaxValue)
            {
                if (currentInvasionType == byte.MaxValue)
                {
                    if (invasionProgressAlpha > 0f)
                        invasionProgressAlpha -= 0.05f;
                }
                else
                {
                    if (invasionProgressAlpha < 1f)
                        invasionProgressAlpha += 0.05f;
                }
                var texture = default(Texture2D);
                var text = default(string);
                var color = default(Color);
                int uiStyle = 0;
                switch (InvasionType)
                {
                    case GlimmerEventInvasion:
                    {
                        invasionProgress = 1f - (float)AQMod.CosmicEvent.GetTileDistance(Main.LocalPlayer) / GlimmerEvent.MaxDistance;
                        uiStyle = 0;
                        texture = TextureCache.GlimmerEventEventIcon.Value;
                        text = AQText.GlimmerEvent().Value;
                        color = new Color(120, 20, 110, 128);
                    }
                    break;

                    case DemonSiegeInvasion:
                    {
                        if (DemonSiege.Upgrade.upgradeTime != 0)
                            invasionProgress = 1f - DemonSiege.UpgradeTime / (float)DemonSiege.Upgrade.upgradeTime;
                        uiStyle = 0;
                        texture = TextureCache.DemonSiegeEventIcon.Value;
                        text = AQText.ModText("Common.DemonSiege").Value;
                        color = new Color(120, 70 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10), 20, 128);
                    }
                    break;
                }
                float alpha = 0.5f + invasionProgressAlpha * 0.5f;
                switch (uiStyle)
                {
                    default:
                    {
                        int num11 = (int)(200f * alpha);
                        int num12 = (int)(45f * alpha);
                        var vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)vector3.X - num11 / 2, (int)vector3.Y - num12 / 2, num11, num12), new Color(63, 65, 151, 255) * 0.785f);

                        string eventNameAndProgressText = (int)(invasionProgress * 100f) + "%";
                        //text3 += " (" + InvasionType + ")";
                        eventNameAndProgressText = Language.GetTextValue("Game.WaveCleared", eventNameAndProgressText);

                        if (InvasionType == DemonSiegeInvasion)
                        {
                            string value = AQText.ModText("Common.TimeLeft").Value;
                            eventNameAndProgressText = string.Format(value, AQUtils.TimeText3(DemonSiege.UpgradeTime));
                        }

                        Texture2D colorBar = Main.colorBarTexture;
                        Main.spriteBatch.Draw(colorBar, vector3, null, Color.White * invasionProgressAlpha, 0f, new Vector2(colorBar.Width / 2, 0f), alpha, SpriteEffects.None, 0f);
                        float num13 = MathHelper.Clamp(invasionProgress, 0f, 1f);
                        Vector2 textMeasurement = Main.fontMouseText.MeasureString(eventNameAndProgressText);
                        float num2 = alpha;
                        if (textMeasurement.Y > 22f)
                            num2 *= 22f / textMeasurement.Y;
                        float num3 = 169f * alpha;
                        float num4 = 8f * alpha;
                        Vector2 vector5 = vector3 + Vector2.UnitY * num4 + Vector2.UnitX * 1f;
                        Utils.DrawBorderString(Main.spriteBatch, eventNameAndProgressText, vector5 + new Vector2(0f, -4f), Color.White * invasionProgressAlpha, num2, 0.5f, 1f);
                        vector5 += Vector2.UnitX * (num13 - 0.5f) * num3;
                        Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 241, 51) * invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num3 * num13, num4), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 165, 0, 127) * invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num4), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle(0, 0, 1, 1), Color.Black * invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num3 * (1f - num13), num4), SpriteEffects.None, 0f);
                    }
                    break;
                }
                Vector2 textOrig = Main.fontMouseText.MeasureString(text);
                float xOff = 120f;
                if (textOrig.X > 200f)
                    xOff += textOrig.X - 200f;
                Rectangle rectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - xOff, y: Main.screenHeight - 80), (textOrig + new Vector2(texture.Width + 12, 6f)) * alpha);
                Utils.DrawInvBG(Main.spriteBatch, rectangle, color);
                Main.spriteBatch.Draw(texture, rectangle.Left() + Vector2.UnitX * alpha * 8f, null, Color.White * invasionProgressAlpha, 0f, new Vector2(0f, texture.Height / 2), alpha * 0.8f, SpriteEffects.None, 0f);
                Utils.DrawBorderString(Main.spriteBatch, text, rectangle.Right() + Vector2.UnitX * alpha * -22f, Color.White * invasionProgressAlpha, alpha * 0.9f, 1f, 0.4f);
            }
        }
    }
}