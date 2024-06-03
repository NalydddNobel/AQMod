using Aequus.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;

namespace Aequus.Common.Tiles.Rubblemaker;

internal abstract class RubblemakerTile : AutoloadedInstancedModTile {
    public static ushort GetId<T>() where T : RubblemakerTile {
        return _registeredDict[typeof(T)].Item1.Type;
    }
    public static ushort GetRubblemakerId<T>() where T : RubblemakerTile {
        return _registeredDict[typeof(T)].Item2.Type;
    }
    private static readonly Dictionary<Type, (RubblemakerTile, RubblemakerTile)> _registeredDict = new();

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
            // Manually register old names for rubblemaker tile variants
            ModTypeLookup<ModTile>.RegisterLegacyNames(copy, LegacyNameAttribute.GetLegacyNamesOfType(GetType()).Select((s) => s + "Rubblemaker").ToArray());

            _registeredDict[GetType()] = (this, copy);
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

    public override void Unload() {
        _registeredDict.Clear();
    }
}