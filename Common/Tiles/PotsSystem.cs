using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public class PotsSystem : ModSystem {
    public record PotLootPreview(Texture2D Texture, Rectangle? Frame, int Stack, bool Dangerous) {
        public float Opacity;
    }

    public static Dictionary<Point, PotLootPreview> LootPreviews { get; private set; } = new();

    public static Queue<Point> RemoveQueue { get; private set; } = new();

    public override void PreUpdateEntities() {
        lock (LootPreviews) {
            foreach (var preview in LootPreviews) {
                if (!InPotSightRange(Main.LocalPlayer, preview.Key)) {
                    preview.Value.Opacity -= 0.04f;
                    if (preview.Value.Opacity <= 0f) {
                        RemoveQueue.Enqueue(preview.Key);
                    }
                }
                else if (preview.Value.Opacity < 1f) {
                    preview.Value.Opacity += 0.1f;
                    if (preview.Value.Opacity > 1f) {
                        preview.Value.Opacity = 1f;
                    }
                }
            }

            while (RemoveQueue.TryDequeue(out var removePoint)) {
                LootPreviews.Remove(removePoint);
            }            
        }
    }

    private void DrawPreview(Point tileCoordinates, PotLootPreview preview) {
        float scale = preview.Opacity;
        var frame = preview.Frame ?? preview.Texture.Bounds;
        int largestSide = Math.Max(frame.Width, frame.Height);
        if (largestSide > 24f) {
            scale *= 24f / largestSide;
        }
        var seed = Helper.TileSeed(tileCoordinates) % 100f;
        scale *= Helper.Oscillate(Main.GlobalTimeWrappedHourly * 10f + seed, 0.9f, 1f);
        var drawCoordinates = new Vector2(tileCoordinates.X * 16f + 16f, tileCoordinates.Y * 16f + 20f) - Main.screenPosition + new Vector2(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 3f + seed * 0.9f, -1f, 1f), Helper.Oscillate(Main.GlobalTimeWrappedHourly * 1.2f + seed * 0.8f, -2f, 2f));
        float rotation = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 4.2f, -0.1f, 0.1f);
        MiscWorldInterfaceElements.Draw(AequusTextures.BloomStrong, drawCoordinates, null, Color.Black * 0.75f * preview.Opacity, 0f, AequusTextures.BloomStrong.Size() / 2f, 0.4f, SpriteEffects.None, 0f);
        MiscWorldInterfaceElements.Draw(preview.Texture, drawCoordinates + new Vector2(2f) * scale, frame, Color.Black * 0.33f * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        MiscWorldInterfaceElements.Draw(preview.Texture, drawCoordinates, frame, Color.White * 0.66f * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        if (preview.Stack > 1) {
            MiscWorldInterfaceElements.DrawColorCodedString(FontAssets.MouseText.Value, preview.Stack.ToString(), drawCoordinates + new Vector2(-12f, -2f), Color.White * 0.66f * preview.Opacity, 0f, Vector2.Zero, Vector2.One * 0.66f);
        }
    }

    public override void PostDrawTiles() {
        lock (LootPreviews) {
            foreach (var preview in LootPreviews) {
                DrawPreview(preview.Key, preview.Value);
            }
        }
    }

    public static bool InPotSightRange(Player player, Point potCoordinates) {
        return InPotSightRange(player.Center, new Vector2(potCoordinates.X * 16f + 16f, potCoordinates.Y * 16f + 16f));
    }

    public static bool InPotSightRange(Vector2 pos1, Vector2 pos2) {
        return Vector2.Distance(pos1, pos2) < 360f;
    }
}