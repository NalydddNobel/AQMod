using AequusRemake.Core.Graphics.Animations;
using Terraria.Audio;

namespace AequusRemake.Systems.Fishing.CrabPots;

public class AnimationOpenCrabPot : ITileAnimation {
    public int Frame;
    public int FrameCount;
    public int RealFrame;

    public bool Update(int x, int y) {
        if (FrameCount == 0 && Frame == 0) {
            SoundEngine.PlaySound(SoundID.DoorOpen, new Vector2(x, y).ToWorldCoordinates());
        }
        if (++FrameCount > 3) {
            FrameCount = 0;
            Frame++;
            RealFrame = Frame;
            if (Frame >= UnifiedCrabPot.FramesCount) {
                RealFrame = UnifiedCrabPot.FramesCount - (Frame - UnifiedCrabPot.FramesCount + 1);
            }
        }
        return RealFrame > -1;
    }
}