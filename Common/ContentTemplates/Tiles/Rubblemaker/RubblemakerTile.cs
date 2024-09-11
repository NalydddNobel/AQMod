using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;

namespace Aequus.Common.ContentTemplates.Tiles.Rubblemaker;

internal abstract class RubblemakerTile(string? NameOverride, bool Natural) : ModTile {
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

    public readonly bool Natural = Natural;

    protected RubblemakerTile() : this(null, Natural: true) {
    }

    public override string Name => NameOverride ?? base.Name;
    public override string Texture => Natural ? base.Texture : base.Texture[..^("Rubblemaker".Length)];

    public RubblemakerTile? CreateRubblemakerCopy() {
        return Activator.CreateInstance(GetType(), Name + "Rubblemaker", false) as RubblemakerTile;
    }

    public sealed override void Load() {
        if (Natural) {
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

        if (!Natural) {
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