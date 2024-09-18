namespace Aequus.Content.CursorDyes.Items;
public class XmasCursor : CursorDyeBase {
    public override void SetDefaults() {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 1);
    }

    public override ICursorDye InitalizeDye() {
        return new TextureChangeCursor($"{Texture}/Cursor", new Vector2(0f, -2f));
    }
}