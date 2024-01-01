using System;
using Terraria.GameContent;

namespace Aequus.Common.Tiles.Rubblemaker;

internal abstract class RubblemakerTile : AutoloadedInstanceableModTile {
    public abstract int UseItem { get; }

    public abstract int[] Styles { get; }

    public abstract FlexibleTileWand Size { get; }

    protected readonly bool _natural;

    protected RubblemakerTile() : this(null, null, natural: true) {
    }

    protected RubblemakerTile(string? name, string? texture, bool natural) : base(name, texture) {
        _natural = natural;
    }

    public RubblemakerTile CreateRubblemakerCopy() {
        return (RubblemakerTile)Activator.CreateInstance(GetType(), Name + "Rubblemaker", Texture, false);
    }

    public sealed override void Load() {
        if (_natural) {
            var copy = CreateRubblemakerCopy();
            Mod.AddContent(copy);
            Load(copy);
        }
    }

    public virtual void Load(RubblemakerTile rubblemakerCopy) {
    }

    public sealed override void SetStaticDefaults() {
        SafeSetStaticDefaults();

        if (!_natural) {
            int item = UseItem;
            Size.AddVariations(item, Type, Styles);
            RegisterItemDrop(item);
        }
    }

    public virtual void SafeSetStaticDefaults() {
    }
}