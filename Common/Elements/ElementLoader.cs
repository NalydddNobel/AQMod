using System.Collections.Generic;

namespace Aequus.Common.Elements;

public partial class ElementLoader : ModSystem {
    public static readonly List<Element> Elements = [];

    public static int ElementCount => Elements.Count;

    public static Element GetElement(int Type) {
        return Elements.IndexInRange(Type) ? Elements[Type] : null;
    }

    public static Element RegisterElement(Element element) {
        element.Type = Elements.Count;
        Elements.Add(element);
        return element;
    }

    public override void PostSetupRecipes() {
        foreach (Element e in Elements) {
            e.OnPostSetupRecipes();
        }
    }

    public override void Unload() {
        Elements.Clear();
    }
}