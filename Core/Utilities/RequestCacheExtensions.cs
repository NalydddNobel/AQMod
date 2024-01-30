using Aequus.Core.Assets;
using System.Runtime.CompilerServices;

namespace Aequus.Core.Utilities;

public static class RequestCacheExtensions {
    public static System.Int32 Width(this RequestCache<Texture2D> asset) => asset.Value.Width;
    public static System.Int32 Height(this RequestCache<Texture2D> asset) => asset.Value.Height;
    public static Rectangle Bounds(this RequestCache<Texture2D> asset) => asset.Value.Bounds;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetCenteredFrameOrigin(this RequestCache<Texture2D> asset, System.Int32 horizontalFrames = 1, System.Int32 verticalFrames = 1) {
        var size = asset.Size();
        return new Vector2(size.X / horizontalFrames / 2f, size.Y / verticalFrames / 2f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Size(this RequestCache<Texture2D> asset) {
        return asset.Value.Size();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Frame(this RequestCache<Texture2D> asset, System.Int32 horizontalFrames = 1, System.Int32 verticalFrames = 1, System.Int32 frameX = 0, System.Int32 frameY = 0, System.Int32 sizeOffsetX = 0, System.Int32 sizeOffsetY = 0) {
        var value = asset.Value;
        System.Int32 num = value.Width / horizontalFrames;
        System.Int32 num2 = value.Height / verticalFrames;
        return new Rectangle(num * frameX, num2 * frameY, num + sizeOffsetX, num2 + sizeOffsetY);
    }
}