using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;

namespace AequusRemake.Systems;

/// <summary>Allows painting of arbitrary textures.</summary>
internal class PaintSystem : ModSystem {
    public static readonly Dictionary<TextureVariantKey, TextureRenderTargetHolder> Renderers = new();
    public static readonly List<TilePaintSystemV2.ARenderTargetHolder> Requests = new();

    public override void Load() {
        On_TilePaintSystemV2.PrepareAllRequests += TilePaintSystemV2_PrepareAllRequests;
    }

    private static void TilePaintSystemV2_PrepareAllRequests(On_TilePaintSystemV2.orig_PrepareAllRequests orig, TilePaintSystemV2 self) {
        orig(self);
        if (Requests.Count != 0) {
            for (int i = 0; i < Requests.Count; i++) {
                Requests[i].Prepare();
            }
            Requests.Clear();
        }
    }

    public override void Unload() {
        Main.QueueMainThreadAction(ClearTargets);
    }

    public static void Reset() {
        Main.QueueMainThreadAction(ClearTargets);
    }

    public static void RequestTile(ref TextureVariantKey lookupKey) {
        if (!Renderers.TryGetValue(lookupKey, out var value)) {
            value = new TextureRenderTargetHolder {
                Key = lookupKey
            };
            Renderers.Add(lookupKey, value);
        }
        if (!value.IsReady) {
            Requests.Add(value);
        }
    }

    public static Texture2D TryGetPaintedTexture(TextureVariantKey lookupKey) {
        if (Renderers.TryGetValue(lookupKey, out var value) && value.IsReady) {
            return value.Target;
        }

        RequestTile(ref lookupKey);
        return mod.Assets.Request<Texture2D>(lookupKey.Texture, AssetRequestMode.ImmediateLoad).Value;
    }

    public static Texture2D TryGetPaintedTexture(Tile tile, string texture) {
        return TryGetPaintedTexture(new(texture, tile.TileType, 0, tile.TileColor));
    }

    public static Texture2D TryGetPaintedTexture(int i, int j, string texture) {
        return TryGetPaintedTexture(Main.tile[i, j], texture);
    }

    public static Texture2D TryGetPaintedTexture(Point p, string texture) {
        return TryGetPaintedTexture(p.X, p.Y, texture);
    }

    private static void ClearTargets() {
        foreach (TextureRenderTargetHolder value in Renderers.Values) {
            value.Clear();
        }

        Renderers.Clear();
    }
}

public record struct TextureVariantKey(string Texture, int TileType, int TileStyle, byte PaintColor) {
    public readonly bool Equals(TextureVariantKey other) {
        return Texture == other.Texture && PaintColor == other.PaintColor;
    }

    public override readonly int GetHashCode() {
        return Texture.GetHashCode() + PaintColor;
    }
}

public class TextureRenderTargetHolder : TilePaintSystemV2.ARenderTargetHolder {
    public TextureVariantKey Key;

    public override void Prepare() {
        PrepareTextureIfNecessary(mod.Assets.Request<Texture2D>(Key.Texture, AssetRequestMode.ImmediateLoad).Value);
    }

    public override void PrepareShader() {
        PrepareShader(Key.PaintColor, TreePaintSystemData.GetTileSettings(Key.TileType, Key.TileStyle));
    }
}