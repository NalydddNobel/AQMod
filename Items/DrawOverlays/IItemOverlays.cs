namespace AQMod.Items
{
    public interface IItemOverlays
    {
        IOverlayDrawWorld WorldDraw { get; }
        IOverlayDrawInventory InventoryDraw { get; }
        IOverlayDrawPlayerUse PlayerDraw { get; }
    }
}