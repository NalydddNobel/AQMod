namespace Aequu2.Core.Entities.Items.Components;

public interface IPickItemMovementAction {
    public const int RESULT_NONE = -1;
    /// <summary>Returning this result will attempt to place the item into the slot.</summary>
    public const int RESULT_SIMPLE_SWAP = 0;
    /// <summary>Returning this result will attempt to place the item into the slot, if it is an equipable and follows conditions depending on the context.</summary>
    public const int RESULT_EQUIPMENT_SWAP = 1;
    /// <summary>Returning this result will attempt to place the item into the slot, if it is a Dye.</summary>
    public const int RESULT_DYE_SWAP = 2;
    /// <summary>Returning this result will attempt to purchase the item.</summary>
    public const int RESULT_BUY_ITEM = 3;
    /// <summary>Returning this result will attempt to sell the item.</summary>
    public const int RESULT_SELL_ITEM = 4;
    /// <summary>Returning this result will attempt dupe the item from the item slot, stacking it onto the mouse item.</summary>
    public const int RESULT_JOURNEY_DUPE = 5;

    void OverrideItemMovementAction(ref int result, Item[] inventory, int context, int slot, Item checkItem);
}
