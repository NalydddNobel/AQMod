using Terraria.Localization;

namespace Aequus.Core.Debugging;

internal class DebugItem : ModItem {
    private static int _debugItemsRegistered;
    private Color _color;

    protected override bool CloneNewInstances => true;

    public override string Texture => AequusTextures.StunEffect.Path;

    public override LocalizedText DisplayName => Language.GetText(PrettyPrintName());
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DEBUG_MODE;
    }

    public override void Load() {
        _debugItemsRegistered++;
    }

    public override void SetStaticDefaults() {
        int colorIndex = 0;
        for (int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
            if (ItemLoader.GetItem(i) is DebugItem) {
                colorIndex++;
            }
            if (ItemLoader.GetItem(i)?.Equals(this) == true) {
                _color = Main.hslToRgb(new Vector3(colorIndex / (float)_debugItemsRegistered, 1f, 0.5f));
                break;
            }
        }
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Orange;
        Item.color = _color;
    }
}
