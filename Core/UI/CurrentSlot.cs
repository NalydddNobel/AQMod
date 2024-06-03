namespace Aequus.Core.UI;

public sealed class CurrentSlot {
    public static CurrentSlot Instance { get; private set; } = new();

    private readonly Item[] _singleItemArray = new Item[1];

    public int Context { get; private set; }
    public int Slot { get; private set; }
    public Item[] Inventory { get; private set; }
    public Vector2 Position { get; private set; }
    public Color LightColor { get; private set; }

    public bool IsDirty { get; private set; }

    public bool Update(int Context, int Slot, Item Item, Vector2 Position, Color LightColor) {
        _singleItemArray[0] = Item;
        return Update(Context, Slot, _singleItemArray, Position, LightColor);
    }

    public bool Update(int Context, int Slot, Item[] Inventory, Vector2 Position, Color LightColor) {
        if (!IsDirty && Context == this.Context && Slot == this.Slot) {
            return false;
        }

        this.Context = Context;
        this.Slot = Slot;
        this.Inventory = Inventory;
        this.Position = Position;
        this.LightColor = LightColor;
        IsDirty = false;

        return true;
    }

    public void Dirty() {
        IsDirty = true;
    }
}