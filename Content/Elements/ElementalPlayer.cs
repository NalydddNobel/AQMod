using Aequus.Content.Elements;
using System.Collections.Generic;

namespace Aequus.Content.Elements;

public class ElementalPlayer : ModPlayer {
    public HashSet<Element> visibleElements = [];

    public override void ResetEffects() {
        visibleElements.Clear();
    }
}
