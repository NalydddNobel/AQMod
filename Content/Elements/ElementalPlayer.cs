using Aequu2.Content.Elements;
using System.Collections.Generic;

namespace Aequu2.Content.Elements;

public class ElementalPlayer : ModPlayer {
    public HashSet<Element> visibleElements = [];

    public override void ResetEffects() {
        visibleElements.Clear();
    }
}
