namespace Aequus.Content.CursorDyes.Items;
[LegacyName("DemonicCursorDye")]
public class DemonCursor : CursorDyeBase {
    public override void SetDefaults() {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 1);
    }

    public override ICursorDye InitalizeDye() {
        return new TextureChangeCursor($"{Texture}/DemonCursor");
    }
}