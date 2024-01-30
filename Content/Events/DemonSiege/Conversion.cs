namespace Aequus.Content.Events.DemonSiege;

public struct Conversion {
    public readonly System.Int32 OriginalItem;
    public readonly System.Int32 NewItem;
    public EventTier Progression;
    public System.Boolean Hide;
    public System.Boolean DisableDecraft;

    public Conversion(System.Int32 oldItem, System.Int32 newItem, EventTier progression) {
        OriginalItem = oldItem;
        NewItem = newItem;
        Progression = progression;
        Hide = false;
        DisableDecraft = false;
    }

    public Item Convert(Item original) {
        original.Transform(NewItem);
        return original;
    }
}