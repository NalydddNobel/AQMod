using Aequus.Common.Utilities.Extensions;
using Aequus.Common.Assets;
using System.Runtime.CompilerServices;

namespace Aequus.Common.Utilities.Extensions;

public static class RequestCacheExtensions {
    public static int Width(this RequestCache<Texture2D> asset) {
        return asset.Value.Width;
    }

    public static int Height(this RequestCache<Texture2D> asset) {
        return asset.Value.Height;
    }

    public static Rectangle Bounds(this RequestCache<Texture2D> asset) {
        return asset.Value.Bounds;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetCenteredFrameOrigin(this RequestCache<Texture2D> asset, int horizontalFrames = 1, int verticalFrames = 1) {
        var size = asset.Size();
        return new Vector2(size.X / horizontalFrames / 2f, size.Y / verticalFrames / 2f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Size(this RequestCache<Texture2D> asset) {
        return asset.Value.Size();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Frame(this RequestCache<Texture2D> asset, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0) {
        var value = asset.Value;
        int num = value.Width / horizontalFrames;
        int num2 = value.Height / verticalFrames;
        return new Rectangle(num * frameX, num2 * frameY, num + sizeOffsetX, num2 + sizeOffsetY);
    }
}