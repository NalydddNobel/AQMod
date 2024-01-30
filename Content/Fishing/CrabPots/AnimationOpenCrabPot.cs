using Aequus.Core.Graphics.Animations;
using Terraria.Audio;

namespace Aequus.Content.Fishing.CrabPots;

public class AnimationOpenCrabPot : ITileAnimation {
    public System.Int32 Frame;
    public System.Int32 FrameCount;
    public System.Int32 RealFrame;

    public System.Boolean Update(System.Int32 x, System.Int32 y) {
        if (FrameCount == 0 && Frame == 0) {
            SoundEngine.PlaySound(SoundID.DoorOpen, new Vector2(x, y).ToWorldCoordinates());
        }
        if (++FrameCount > 3) {
            FrameCount = 0;
            Frame++;
            RealFrame = Frame;
            if (Frame >= BaseCrabPot.FramesCount) {
                RealFrame = BaseCrabPot.FramesCount - (Frame - BaseCrabPot.FramesCount + 1);
            }
        }
        return RealFrame > -1;
    }
}