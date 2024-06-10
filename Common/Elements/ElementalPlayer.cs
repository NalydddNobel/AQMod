using System.Collections.Generic;

namespace Aequus.Common.Elements;

public class ElementalPlayer : ModPlayer {
    public HashSet<Element> visibleElements = [];

    public override void ResetEffects() {
        visibleElements.Clear();
    }
}
