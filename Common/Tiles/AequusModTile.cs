using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public abstract class AequusModTile : ModTile {
    protected virtual void AutoDefaults() {
        Mod.AddContent(new AutoloadedTileItem(this));
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