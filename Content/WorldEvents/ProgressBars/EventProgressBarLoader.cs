using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;

namespace AQMod.Content.WorldEvents.ProgressBars
{
    public static class EventProgressBarLoader
    {
        public static byte ActiveBar { get; internal set; } = 255;
        public static bool PlayerSafe { get; internal set; }
        public static bool PlayerSafe_GaleStreams { get; internal set; }

        private static int NextIndex;
        private static EventProgressBar[] _progressBars;

        private static float _invasionProgressAlpha = 0f;
        private static float _invasionProgress = 0f;

        public static int Count => NextIndex;
        public static EventProgressBar GetProgressBar(int type)
        {
            return _progressBars[type];
        }

        internal static void AddBar(EventProgressBar bar)
        {
            if (_progressBars == null)
            {
                NextIndex = 0;
                _progressBars = new EventProgressBar[1];
            }
            else
            {
                Array.Resize(ref _progressBars, NextIndex + 1);
            }
            _progressBars[NextIndex] = bar;
            NextIndex++;
        }

        internal static void Draw()
        {
            if (Main.invasionProgressAlpha > 0f && _invasionProgressAlpha <= 0f)
            {
                ActiveBar = 255;
                return;
            }
            Main.invasionProgressAlpha = 0f;
            byte currentInvasionType = 255;
            for (byte i = 0; i < _progressBars.Length; i++)
            {
                EventProgressBar b = _progressBars[i];
                if (b.IsActive())
                {
                    currentInvasionType = i;
                    ActiveBar = i;
                }
            }
            if (ActiveBar != byte.MaxValue)
            {
                if (currentInvasionType == byte.MaxValue)
                {
                    if (_invasionProgressAlpha > 0f)
                    {
                        _invasionProgressAlpha -= 0.05f;
                    }
                    else
                    {
                        ActiveBar = 255;
                        return;
                    }
                }
                else
                {
                    if (_invasionProgressAlpha < 1f)
                        _invasionProgressAlpha += 0.05f;
                }

                var bar = _progressBars[ActiveBar];
                var texture = bar.IconTexture;
                var eventName = bar.EventName;
                var nameBGColor = bar.NameBGColor;
                float alpha = 0.5f + _invasionProgressAlpha * 0.5f;
                int num11 = (int)(200f * alpha);
                int num12 = (int)(45f * alpha);
                var vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);

                if (!bar.PreDraw(texture, eventName, nameBGColor, alpha))
                {
                    return;
                }

                Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)vector3.X - num11 / 2, (int)vector3.Y - num12 / 2, num11, num12), new Color(63, 65, 151, 255) * 0.785f);

                _invasionProgress = bar.EventProgress;

                string progressText = (int)(_invasionProgress * 100f) + "%";
                progressText = bar.ModifyProgressText(Language.GetTextValue("Game.WaveCleared", progressText));

                Texture2D colorBar = Main.colorBarTexture;
                Main.spriteBatch.Draw(colorBar, vector3, null, Color.White * _invasionProgressAlpha, 0f, new Vector2(colorBar.Width / 2, 0f), alpha, SpriteEffects.None, 0f);
                float num13 = MathHelper.Clamp(_invasionProgress, 0f, 1f);
                Vector2 textMeasurement = Main.fontMouseText.MeasureString(progressText);
                float num2 = alpha;
                if (textMeasurement.Y > 22f)
                    num2 *= 22f / textMeasurement.Y;
                float num3 = 169f * alpha;
                float num4 = 8f * alpha;
                Vector2 vector5 = vector3 + Vector2.UnitY * num4 + Vector2.UnitX * 1f;
                Utils.DrawBorderString(Main.spriteBatch, progressText, vector5 + new Vector2(0f, -4f), Color.White * _invasionProgressAlpha, num2, 0.5f, 1f);
                vector5 += Vector2.UnitX * (num13 - 0.5f) * num3;
                Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 241, 51) * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num3 * num13, num4), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 165, 0, 127) * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num4), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Main.magicPixel, vector5, new Rectangle(0, 0, 1, 1), Color.Black * _invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num3 * (1f - num13), num4), SpriteEffects.None, 0f);

                Vector2 textOrig = Main.fontMouseText.MeasureString(eventName);
                float xOff = 120f;
                if (textOrig.X > 200f)
                    xOff += textOrig.X - 200f;
                Rectangle rectangle = Utils.CenteredRectangle(new Vector2(Main.screenWidth - xOff, y: Main.screenHeight - 80), (textOrig + new Vector2(texture.Width + 12, 6f)) * alpha);
                Utils.DrawInvBG(Main.spriteBatch, rectangle, nameBGColor);
                Main.spriteBatch.Draw(texture, rectangle.Left() + Vector2.UnitX * alpha * 8f, null, Color.White * _invasionProgressAlpha, 0f, new Vector2(0f, texture.Height / 2), alpha * 0.8f, SpriteEffects.None, 0f);
                Utils.DrawBorderString(Main.spriteBatch, eventName, rectangle.Right() + Vector2.UnitX * alpha * -22f, Color.White * _invasionProgressAlpha, alpha * 0.9f, 1f, 0.4f);
            }
            else if (_invasionProgressAlpha <= 0f)
            {
                ActiveBar = byte.MaxValue;
            }
        }

        internal static void Unload()
        {
            _progressBars = null;
            NextIndex = 0;
        }
    }
}