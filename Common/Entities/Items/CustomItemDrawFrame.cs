using Terraria.DataStructures;

namespace Aequus.Common.Entities.Items;

public class CustomItemDrawFrame(Rectangle SourceFrame) : DrawAnimation {
    public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1) {
        return SourceFrame;
    }
}
