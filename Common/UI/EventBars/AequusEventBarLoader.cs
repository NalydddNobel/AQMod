﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Common.UI.EventBars;

public class AequusEventBarLoader : ILoadable {
    public static byte ActiveBar { get; internal set; } = 255;
    public static bool PlayerSafe { get; internal set; }

    public static readonly List<IEventBar> ProgressBars = new();

    private static float _invasionProgressAlpha = 0f;
    private static float _invasionProgress = 0f;

    public static int Count => ProgressBars.Count;
    public static IEventBar GetProgressBar(int type) {
        return ProgressBars[type];
    }

    internal static void AddBar(IEventBar bar) {
        ProgressBars.Add(bar);
    }

    internal static void Draw() {
        if (ProgressBars == null || Main.invasionProgressAlpha > 0f && _invasionProgressAlpha <= 0f) {
            ActiveBar = 255;
            return;
        }
        Main.invasionProgressAlpha = 0f;
        byte currentInvasionType = 255;
        for (byte i = 0; i < ProgressBars.Count; i++) {
            IEventBar b = ProgressBars[i];
            if (b.IsActive()) {
                currentInvasionType = i;
                ActiveBar = i;
            }
        }
        if (ActiveBar != byte.MaxValue) {
            if (currentInvasionType == byte.MaxValue) {
                if (_invasionProgressAlpha > 0f)
                    _invasionProgressAlpha -= 0.05f;
                else {
                    ActiveBar = 255;
                    return;
                }
            }
            else {
                if (_invasionProgressAlpha < 1f) {
                    _invasionProgressAlpha += 0.05f;
                }
            }

            var bar = ProgressBars[ActiveBar];
            var texture = bar.Icon.Value;
            var eventName = bar.DisplayName.Value;
            var nameBGColor = bar.BackgroundColor;
            float alpha = 0.5f + _invasionProgressAlpha * 0.5f;
            int num11 = (int)(200f * alpha);
            int num12 = (int)(45f * alpha);
            var vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);

            if (!bar.PreDraw(texture, eventName, nameBGColor, alpha)) {
                return;
            }

            Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)vector3.X - num11 / 2, (int)vector3.Y - num12 / 2, num11, num12), new Color(63, 65, 151, 255) * 0.785f);

            _invasionProgress = bar.GetEventProgress();

            string progressText = bar.GetProgressText(_invasionProgress);

            Texture2D colorBar = TextureAssets.ColorBar.Value;
            var font = FontAssets.MouseText.Value;
            var pixel = TextureAssets.MagicPixel.Value;
            Main.spriteBatch.Draw(colorBar, vector3, null, Color.White * _invasionProgressAlpha, 0f, new Vector2(colorBar.Width / 2, 0f), alpha, SpriteEffects.None, 0f);
            float num13 = MathHelper.Clamp(_invasionProgress, 0f, 1f);
            Vector2 textMeasurement = font.MeasureString(progressText);
            float num2 = alpha;
            float textOffsetX = 0f;
            if (textMeasurement.Y > 22f) {
                num2 *= 22f / textMeasurement.Y;
            }

            float num3 = 169f * alpha;
            float num4 = 8f * alpha;
            Vector2 vector5 = vector3 + Vector2.UnitY * num4 + Vector2.UnitX * 1f;
            Utils.DrawBorderString(Main.spriteBatch, progressText, vector5 + new Vector2(0f, -4f), Color.White * _invasionProgressAlpha, num2, 0.5f, 1f);
            vector5 += Vector2.UnitX * (num13 - 0.5f) * num3;
            Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), bar.BackgroundColor * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num3 * num13, num4), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), bar.BackgroundColor with { A = 127 } * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num4), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), Color.Black * _invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num3 * (1f - num13), num4), SpriteEffects.None, 0f);

            Vector2 textOrig = font.MeasureString(eventName);
            float xOff = 120f;
            if (textOrig.X > 200f) {
                xOff += textOrig.X - 200f;
            }
            textOffsetX += 10f;

            Rectangle rectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - xOff, y: Main.screenHeight - 80), (textOrig + new Vector2(texture.Width + 12, 6f)) * alpha);
            Utils.DrawInvBG(Main.spriteBatch, rectangle, nameBGColor);
            Main.spriteBatch.Draw(texture, rectangle.Left() + Vector2.UnitX * alpha * 8f, null, Color.White * _invasionProgressAlpha, 0f, new Vector2(0f, texture.Height / 2), alpha * 0.8f, SpriteEffects.None, 0f);
            Utils.DrawBorderString(Main.spriteBatch, eventName, rectangle.Right() + Vector2.UnitX * alpha * -22f + new Vector2(textOffsetX, 0f), Color.White * _invasionProgressAlpha, alpha * 0.9f, 1f, 0.4f);
        }
        else if (_invasionProgressAlpha <= 0f) {
            ActiveBar = byte.MaxValue;
        }
    }

    void ILoadable.Load(Mod mod) {
    }

    void ILoadable.Unload() {
        ProgressBars?.Clear();
    }
}