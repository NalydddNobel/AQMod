using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public abstract class AequusModTile : ModTile {
    private AutoloadedTileItem _item;
    public ModItem Item => _item;

    protected virtual void AutoDefaults() {
        _item = new AutoloadedTileItem(this);
        Mod.AddContent(_item);
    }

    protected virtual void OnLoad() {
    }

    public sealed override void Load() {
        AutoDefaults();
        OnLoad();
    }

    internal virtual void AddItemRecipes(AutoloadedTileItem modItem) {
    }
}