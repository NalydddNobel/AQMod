using System.Collections.Generic;

namespace AequusRemake.Systems.Elements;

public class ElementalPlayer : ModPlayer {
    public HashSet<Element> visibleElements = [];

    public override void ResetEffects() {
        visibleElements.Clear();
    }
}
