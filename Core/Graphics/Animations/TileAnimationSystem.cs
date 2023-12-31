using System;
using Terraria.DataStructures;

namespace Aequus.Core.Graphics.Animations;

public sealed class TileAnimationSystem : ModSystem {
    public static TrimmableDictionary<Point16, ITileAnimation> FromTile { get; private set; } = new();

    public static bool TryGet<T>(int x, int y, out T anim) where T : ITileAnimation {
        return TryGet(new(x, y), out anim);
    }

    public static bool TryGet<T>(Point16 xy, out T anim) where T : ITileAnimation {
        if (FromTile.TryGetValue(xy, out var tileAnimation) && tileAnimation is T wantedTileAnimationType) {
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
        if (TryGet(xy, out T anim)) {
            return anim;
        }

        FromTile[xy] = new T();
        return (T)FromTile[xy];
    }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            ClearWorld();
            return;
        }

        lock (FromTile) {
            try {
                foreach (var anim in FromTile) {
                    if (anim.Value?.Update(anim.Key.X, anim.Key.Y) != true) {
                        FromTile.RemoveEnqueue(anim.Key);
                    }
                }
                FromTile.RemoveAllQueued();
            }
            catch (Exception ex) {
                Main.NewText(ex);
                FromTile.Clear();
            }
        }
    }

    public override void ClearWorld() {
        lock (FromTile) {
            FromTile.Clear();
        }
    }
}
