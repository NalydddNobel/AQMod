using Microsoft.Xna.Framework;
using System.Linq;

namespace Aequus.Content.DedicatedContent;

public interface IDedicatedItem {
    string DedicateeName { get; }
    string DisplayedDedicateeName => DedicateeName;
    Color TextColor { get; }
    public Color FaelingColor => TextColor;

    public static int GetRandomItemId() {
        return Main.rand.Next(Aequus.Instance.GetContent<ModItem>().Where((i) => i is IDedicatedItem).ToArray()).Type;
    }

    public static int GetItemIdFromName(string name) {
        return (Aequus.Instance.GetContent<ModItem>().Where((i) => i is IDedicatedItem dedicatedItem && dedicatedItem.DedicateeName.Equals(name)).FirstOrDefault()?.Type).GetValueOrDefault(0);
    }
}