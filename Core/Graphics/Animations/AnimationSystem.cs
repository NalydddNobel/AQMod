using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Core.Graphics.Animations;

public sealed class AnimationSystem : ModSystem {
    public static TrimmableDictionary<Point16, ITileAnimation> TileAnimations { get; private set; } = new();

    public static readonly Vector2[] FlameOffsets = new Vector2[7];

    public static bool TryGet<T>(int x, int y, out T anim) where T : ITileAnimation {
        return TryGet(new(x, y), out anim);
    }

    public static bool TryGet<T>(Point16 xy, out T anim) where T : ITileAnimation {
        if (TileAnimations.TryGetValue(xy, out var tileAnimation) && tileAnimation is T wantedTileAnimationType) {
            anim = wantedTileAnimationType;
            return true;
        }
        anim = default;
        return false;
    }

    public static T GetValueOrAddDefault<T>(int x, int y) where T : ITileAnimation, new() {
        return GetValueOrAddDefault<T>(new Point16(x, y));
    }

    public static T GetValueOrAddDefault<T>(Point xy) where T : ITileAnimation, new() {
        return GetValueOrAddDefault<T>(new Point16(xy.X, xy.Y));
    }

    public static T GetValueOrAddDefault<T>(Point16 xy) where T : ITileAnimation, new() {
        if (TryGet(xy, out T anim)) {
            return anim;
        }

        TileAnimations[xy] = new T();
        return (T)TileAnimations[xy];
    }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            ClearWorld();
            return;
        }

        for (int i = 0; i < FlameOffsets.Length; i++) {
            FlameOffsets[i] = Main.rand.NextVector2Square(-i, i);
        }

        lock (TileAnimations) {
            try {
                foreach (var anim in TileAnimations) {
                    if (anim.Value?.Update(anim.Key.X, anim.Key.Y) != true) {
                        TileAnimations.RemoveEnqueue(anim.Key);
                    }
                }
                TileAnimations.RemoveAllQueued();
            }
            catch (Exception ex) {
                Main.NewText(ex);
                TileAnimations.Clear();
            }
        }
    }

    public override void ClearWorld() {
        lock (TileAnimations) {
            TileAnimations.Clear();
        }
    }
}
