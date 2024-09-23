using Terraria.DataStructures;

namespace Aequus.Common.Entities.Items;

public class CustomItemDrawFrame(Rectangle SourceFrame) : DrawAnimation {
    public CustomItemDrawFrame(int X, int Y, int W, int H) : this(new Rectangle(X, Y, W, H)) { }
    public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1) {
        return SourceFrame;
    }
}
