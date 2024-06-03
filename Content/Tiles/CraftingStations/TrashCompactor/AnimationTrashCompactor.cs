using Aequus.Core.Graphics.Animations;
using System;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class AnimationTrashCompactor : ITileAnimation {
    public Vector2 Shake;
    public float ShakeTime;
    public int Frame;
    public int FrameTime;

    public bool Update(int x, int y) {
        if (Frame != 0 || FrameTime != 0) {
            ModContent.GetInstance<TrashCompactor>().AnimateTile(ref Frame, ref FrameTime);
        }

        if (ShakeTime > 0f) {
            ShakeTime *= 0.97f;
            ShakeTime -= 0.1f;
            Shake = Main.rand.NextVector2Square(-ShakeTime, ShakeTime) * 0.25f;
            Shake.Y = Math.Abs(Shake.Y);
        }
        else {
            Shake = Vector2.Zero;
        }

        var tile = Framing.GetTileSafely(x, y);
        return tile.TileType == ModContent.TileType<TrashCompactor>() && tile.TileFrameX == 0 && tile.TileFrameY == 0 && (ShakeTime > 0f || Frame != 0 || FrameTime != 0);
    }
}