using Aequus.Core.UI;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.UI;

namespace Aequus.Old.Common.EventBars;

// Replica of vanilla's invasion progress bars.
public class AequusEventBarLoader : UILayer {
    private static System.Byte _activeBarReal;
    public static System.Byte ActiveBar { get; internal set; } = 255;
    public static System.Boolean PlayerSafe { get; internal set; }

    private static readonly List<IEventBar> _progressBars = new();

    private static System.Single _invasionProgressAlpha = 0f;
    private static System.Single _invasionProgress = 0f;

    public static System.Int32 Count => _progressBars.Count;

    public override void OnPreUpdatePlayers() {
        _activeBarReal = System.Byte.MaxValue;
        for (System.Byte i = 0; i < _progressBars.Count; i++) {
            IEventBar b = _progressBars[i];
            if (b.IsActive()) {
                _activeBarReal = i;
                ActiveBar = i;
                Activate();
            }
        }
    }

    protected override System.Boolean DrawSelf() {
        if (_progressBars == null || (Main.invasionProgressAlpha > 0f && _invasionProgressAlpha <= 0f)) {
            ActiveBar = System.Byte.MaxValue;
            Deactivate();
            return true;
        }
        Main.invasionProgressAlpha = 0f;
        if (ActiveBar != System.Byte.MaxValue) {
            if (_activeBarReal == System.Byte.MaxValue) {
                if (_invasionProgressAlpha > 0f) {
                    _invasionProgressAlpha -= 0.05f;
                }
                else {
                    ActiveBar = System.Byte.MaxValue;
                    Deactivate();
                    return true;
                }
            }
            else {
                if (_invasionProgressAlpha < 1f) {
                    _invasionProgressAlpha += 0.05f;
                }
            }

            var bar = _progressBars[ActiveBar];
            var texture = bar.Icon.Value;
            var eventName = bar.DisplayName.Value;
            var nameBGColor = bar.BackgroundColor;
            System.Single alpha = 0.5f + _invasionProgressAlpha * 0.5f;
            System.Int32 num11 = (System.Int32)(200f * alpha);
            System.Int32 num12 = (System.Int32)(45f * alpha);
            var vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);

            if (!bar.PreDraw(texture, eventName, nameBGColor, alpha)) {
                return true;
            }

            Utils.DrawInvBG(Main.spriteBatch, new Rectangle((System.Int32)vector3.X - num11 / 2, (System.Int32)vector3.Y - num12 / 2, num11, num12), new Color(63, 65, 151, 255) * 0.785f);

            _invasionProgress = bar.GetEventProgress();

            System.String progressText = bar.GetProgressText(_invasionProgress);

            Texture2D colorBar = TextureAssets.ColorBar.Value;
            var font = FontAssets.MouseText.Value;
            var pixel = TextureAssets.MagicPixel.Value;
            Main.spriteBatch.Draw(colorBar, vector3, null, Color.White * _invasionProgressAlpha, 0f, new Vector2(colorBar.Width / 2, 0f), alpha, SpriteEffects.None, 0f);
            System.Single num13 = MathHelper.Clamp(_invasionProgress, 0f, 1f);
            Vector2 textMeasurement = font.MeasureString(progressText);
            System.Single num2 = alpha;
            System.Single textOffsetX = 0f;
            if (textMeasurement.Y > 22f) {
                num2 *= 22f / textMeasurement.Y;
            }

            System.Single num3 = 169f * alpha;
            System.Single num4 = 8f * alpha;
            Vector2 vector5 = vector3 + Vector2.UnitY * num4 + Vector2.UnitX * 1f;
            Utils.DrawBorderString(Main.spriteBatch, progressText, vector5 + new Vector2(0f, -4f), Color.White * _invasionProgressAlpha, num2, 0.5f, 1f);
            vector5 += Vector2.UnitX * (num13 - 0.5f) * num3;
            Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), bar.BackgroundColor * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num3 * num13, num4), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), bar.BackgroundColor with { A = 127 } * _invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num4), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(pixel, vector5, new Rectangle(0, 0, 1, 1), Color.Black * _invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num3 * (1f - num13), num4), SpriteEffects.None, 0f);

            Vector2 textOrig = font.MeasureString(eventName);
            System.Single xOff = 120f;
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
            ActiveBar = System.Byte.MaxValue;
            Deactivate();
        }
        return true;
    }

    public static IEventBar GetProgressBar(System.Int32 type) {
        return _progressBars[type];
    }

    internal static void AddBar(IEventBar bar) {
        _progressBars.Add(bar);
    }

    public override void OnUnload() {
        _progressBars.Clear();
    }

    public AequusEventBarLoader() : base("EventBars", InterfaceLayerNames.InvasionProgressBars_17, InterfaceScaleType.UI) { }
}