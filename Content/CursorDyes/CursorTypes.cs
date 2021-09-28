namespace AQMod.Content.CursorDyes
{
    public enum CursorType : byte
    {
        Cursor = 0,
        SmartCursor = 1,
        EyeglassCursor = 2,
        FavoriteCursor = 3,
        CameraModePinCursorPurple = 4,
        CameraModePinCursorGold = 5,
        TrashCursor = 6,
        QuickPutIntoInventory = 7,
        QuickRemoveFromChestCursor = 8,
        QuickPlaceIntoChestCursor = 9,
        SellItemCursor = 10,
        CursorOutline = 11,
        SmartCursorOutline = 12,
        Count // gamepad cursors are not in this because gamepad support is... non-existent                           
    }
}