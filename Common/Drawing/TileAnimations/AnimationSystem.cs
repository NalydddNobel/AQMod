using Aequus.Common.Structures.Collections;
using System;
using Terraria.DataStructures;

namespace Aequus.Common.Drawing.TileAnimations;

public sealed class AnimationSystem : ModSystem {
    public static DictionaryRemoveQueue<Point16, ITileAnimation> TileAnimations { get; private set; } = new();

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
        return GetValueOrAddDefault<T>(new(x, y));
    }

    public static T GetValueOrAddDefault<T>(Point16 xy) where T : ITileAnimation, new() {
        lock (TileAnimations) {
            if (TryGet(xy, out T anim)) {
                return anim;
            }

            var value = new T();
            TileAnimations[xy] = value;
            return value;
        }
    }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            ClearWorld();
            return;
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
