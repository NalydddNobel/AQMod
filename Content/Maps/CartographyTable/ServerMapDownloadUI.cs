using Aequus.Common.UI;
using Aequus.Common.Utilities;
using System;
using System.Diagnostics;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Aequus.Content.Maps.CartographyTable;
public class ServerMapDownloadUI : UILayer {
    public static ServerMapDownloadUI Instance => ModContent.GetInstance<ServerMapDownloadUI>();

    public override string Layer => AequusUI.InterfaceLayers.MapMinimap_18;

    public float DownloadProgress { get; private set; }
    public float UploadProgress { get; private set; }

    private readonly Stopwatch _netStopwatch = new();

    public LocalizedText? DownloadText { get; private set; }
    public LocalizedText? UploadText { get; private set; }

    public override void SetStaticDefaults() {
        DownloadText = ModContent.GetInstance<CartographyTable>().GetLocalization("DownloadingMap");
        UploadText = ModContent.GetInstance<CartographyTable>().GetLocalization("UploadingMap");
    }

    public void SetDownloadProgress(float value) {
        _netStopwatch.Restart();
        DownloadProgress = value;
    }

    public void SetUploadProgress(float value) {
        _netStopwatch.Restart();
        UploadProgress = value;
    }

    public void EndNet() {
        DownloadProgress = 0f;
        UploadProgress = 0f;
        _netStopwatch.Stop();
    }

    private void DrawSpriteWithDropShadow(SpriteBatch sb, Texture2D texture, Vector2 where, Rectangle frame, Color color, Vector2 origin) {
        sb.Draw(texture, where + new Vector2(2f, 2f), frame, Color.Black * 0.4f, 0f, origin, 1f, SpriteEffects.None, 0f);
        sb.Draw(texture, where, frame, color, 0f, origin, 1f, SpriteEffects.None, 0f);
    }

    private void DrawDownloadBar(Vector2 center, SpriteBatch sb) {
        if (_netStopwatch.ElapsedMilliseconds > 10000) {
            EndNet();
            return;
        }

        float topBarProgress = DownloadProgress;
        float bottomBarProgress = UploadProgress;

        if (topBarProgress <= 0f && bottomBarProgress <= 0f) {
            return;
        }

        Texture2D bar = AequusTextures.CartographyTableDownloadBar.Value;
        Color color = Color.White;
        Rectangle barFrame = new Rectangle(32, 2, 62, 14);
        Vector2 barFillCoords = center - new Vector2(barFrame.Width / 2f - 4, 1f);
        float barFillScale = barFrame.Width - 8f;

        // The bar.
        sb.Draw(AequusTextures.Bloom, center, null, Color.Black * (0.3f + Main.cursorAlpha * 0.3f), 0f, AequusTextures.Bloom.Size() / 2f, new Vector2(1f, 0.7f), SpriteEffects.None, 0f);
        DrawSpriteWithDropShadow(sb, bar, center, barFrame, color, barFrame.Size() / 2f);

        // Throbber for decor.
        int throbberFrameTime = (int)Math.Clamp(Main.GlobalTimeWrappedHourly * 12f % 8f, 0f, 8f);
        Vector2 throbberPosition = center - new Vector2(barFrame.Width / 2f + 10f, 0f);
        Rectangle throbberFrame = new Rectangle(0, 18 * throbberFrameTime, 18, 18);
        DrawSpriteWithDropShadow(sb, bar, throbberPosition, throbberFrame, color, throbberFrame.Size() / 2f);

        Vector2 arrowCoords = center + new Vector2(barFrame.Width / 2f + 6f, 1f);
        Vector2 textCoords = center - new Vector2(0f, 16f);

        var font = FontAssets.MouseText.Value;

        // Draw download sprites.
        if (topBarProgress > 0f) {
            Rectangle dlFrame = new Rectangle(32, 18, 4, 2);
            sb.Draw(bar, barFillCoords + new Vector2(0f, -2f), dlFrame, color, 0f, Vector2.Zero, new Vector2(1f / dlFrame.Width * topBarProgress * barFillScale, 1f), SpriteEffects.None, 0f);

            // Arrow indicating that we are downloading map data.
            if (topBarProgress < 1f) {
                Rectangle dlArrow = new Rectangle(38, 18, 14, 16);
                DrawSpriteWithDropShadow(sb, bar, arrowCoords, dlArrow, color, dlArrow.Size() / 2f);
                arrowCoords.X += 18f;
            }

            if (bottomBarProgress <= 0f) {
                DrawHelper.DrawCenteredText(sb, DownloadText!.Value, textCoords, Colors.AlphaDarken(Color.White), scaleOverride: Vector2.One * 0.66f);
            }
        }

        // Draw upload sprites.
        if (bottomBarProgress > 0f) {
            Rectangle ulFrame = new Rectangle(32, 36, 4, 2);
            sb.Draw(bar, barFillCoords + new Vector2(0f, 2f), ulFrame, color, 0f, Vector2.Zero, new Vector2(1f / ulFrame.Width * bottomBarProgress * barFillScale, 1f), SpriteEffects.None, 0f);

            // Arrow indicating that we are uploading map data.
            if (bottomBarProgress < 1f) {
                Rectangle ulArrow = new Rectangle(38, 36, 14, 16);
                DrawSpriteWithDropShadow(sb, bar, arrowCoords, ulArrow, color, ulArrow.Size() / 2f);
                arrowCoords.X += 18f;
            }

            DrawHelper.DrawCenteredText(sb, UploadText!.Value, textCoords, Colors.AlphaDarken(Color.White), scaleOverride: Vector2.One * 0.66f);
        }
    }

    private void DrawDownloadText(Vector2 center, SpriteBatch sb) {
        if (DownloadProgress > 0f && DownloadProgress < 1f) {
            string dlText = $"{(int)(DownloadProgress * 100f)}%";
            var font = FontAssets.MouseText.Value;
            Vector2 origin = font.MeasureString(dlText) / 2f;
            ChatManager.DrawColorCodedString(sb, font, dlText, center, Colors.AlphaDarken(Color.White), 0f, origin, Vector2.One * 0.76f);
            center.Y += 16f;
        }

        if (UploadProgress > 0f && UploadProgress < 1f) {
            string ulText = $"{(int)(UploadProgress * 100f)}%";
            var font = FontAssets.MouseText.Value;
            Vector2 origin = font.MeasureString(ulText) / 2f;
            ChatManager.DrawColorCodedString(sb, font, ulText, center, Colors.AlphaDarken(Color.White), 0f, origin, Vector2.One * 0.76f);
        }
    }

    public override bool Draw(SpriteBatch spriteBatch) {
        /*
        if (Main.mapFullscreen) {
            DrawDownloadBar(new Vector2(Main.screenWidth / 2f, Main.screenHeight - 40f), spriteBatch);
        }
        else */
        if (Main.mapStyle == 1) {
            Vector2 drawCoordinates = new Vector2(Main.miniMapX, Main.miniMapY + 16f);
            DrawDownloadBar(drawCoordinates, spriteBatch);
            DrawDownloadText(drawCoordinates + new Vector2(0f, 18f), spriteBatch);
        }

        return true;
    }
}
