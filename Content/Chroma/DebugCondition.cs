using ReLogic.Peripherals.RGB;

namespace Aequus.Content.Chroma;
public class DebugCondition : ChromaCondition {
    public override bool IsActive() {
        return Helper.DebugKeyPressed;
    }
}