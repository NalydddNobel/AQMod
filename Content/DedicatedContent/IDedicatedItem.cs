using System.Linq;

namespace Aequus.Content.DedicatedContent;

public interface IDedicatedItem {
    System.String DedicateeName { get; }
    System.String DisplayedDedicateeName => DedicateeName;
    Color TextColor { get; }
    public Color FaelingColor => TextColor;

    public static System.Int32 GetRandomItemId() {
        return Main.rand.Next(Aequus.Instance.GetContent<ModItem>().Where((i) => i is IDedicatedItem).ToArray()).Type;
    }

    public static System.Int32 GetItemIdFromName(System.String name) {
        return (Aequus.Instance.GetContent<ModItem>().Where((i) => i is IDedicatedItem dedicatedItem && dedicatedItem.DedicateeName.Equals(name)).FirstOrDefault()?.Type).GetValueOrDefault(0);
    }
}