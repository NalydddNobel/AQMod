using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.UI.EventProgressBars
{
    public class LegacyEventProgressBarLoader : ILoadable
    {
        public static byte ActiveBar { get; internal set; } = 255;
        public static bool PlayerSafe { get; internal set; }

        private static int NextIndex;
        private static ILegacyEventProgressBar[] _progressBars;

        private static float _invasionProgressAlpha = 0f;
        private static float _invasionProgress = 0f;

        public static int Count => NextIndex;
        public static ILegacyEventProgressBar GetProgressBar(int type)
        {
            return _progressBars[type];
        }

        internal static void AddBar(ILegacyEventProgressBar bar)
        {
            if (_progressBars == null)
            {
                NextIndex = 0;
                _progressBars = new ILegacyEventProgressBar[1];
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
            if (_progressBars == null || (Main.invasionProgressAlpha > 0f && _invasionProgressAlpha <= 0f))
            {
                ActiveBar = 255;
                return;
            }
            Main.invasionProgressAlpha = 0f;
            byte currentInvasionType = 255;
            for (byte i = 0; i < _progressBars.Length; i++)
            {
                ILegacyEventProgressBar b = _progressBars[i];
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
                        _invasionProgressAlpha -= 0.05f;
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
                var texture = ModContent.Request<Texture2D>(bar.Icon, AssetRequestMode.ImmediateLoad).Value;
                var eventName = Language.GetTextValue(bar.EventKey);
                var nameBGColor = bar.BackgroundColor;
                float alpha = 0.5f + _invasionProgressAlpha * 0.5f;
                int num11 = (int)(200f * alpha);
                int num12 = (int)(45f * alpha);
                var vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);

                if (!bar.PreDraw(texture, eventName, nameBGColor, alpha))
                    return;

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
                if (textMeasurement.Y > 22f)
                    num2 *= 22f / textMeasurement.Y;
                float num3 = 169f * alpha;
                float num4 = 8f * alpha;
                Vector2 vector5 = vector3 + Vector2.UnitY * num4 + Vector2.UnitX * 1f;
                Utils.DrawBorderString(Main.spriteBatch, progressText, vector5 + new Vector2(0f, -4f), Color.White * _invasionProgressAlpha, num2, 0.5f, 1f);
                vector5 += Vector2.UnitX * (num13 - 0.5f) * num3;
                Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 241, 51) * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num3 * num13, num4), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 165, 0, 127) * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num4), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), Color.Black * _invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num3 * (1f - num13), num4), SpriteEffects.None, 0f);

                Vector2 textOrig = font.MeasureString(eventName);
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

        void ILoadable.Load(Mod mod)
        {
            if (_progressBars == null)
                _progressBars = Array.Empty<ILegacyEventProgressBar>();
        }

        void ILoadable.Unload()
        {
            _progressBars = null;
            NextIndex = 0;
        }
    }
}