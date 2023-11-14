using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

[Flags]
internal enum GenerationFlags : byte {
    None = 0,
    /// <summary>
    /// If this tile is Not Friendly, allows for a hostile wall Item variant to be generated.
    /// </summary>
    HostileItem = 1 << 0,
    /// <summary>
    /// This generates the base item for the wall. Places the friendly version of the wall.
    /// </summary>
    FriendlyItem = 1 << 1,
    /// <summary>
    /// If this tile is Not Friendly, allows for a friendly wall variant to be generated.
    /// </summary>
    FriendlyWall = 1 << 2,
    All = byte.MaxValue
}

public abstract class AequusModWall : ModWall {
    public abstract bool Friendly { get; }
    internal virtual GenerationFlags ItemGenerationFlags => GenerationFlags.All;

    internal abstract int DustId { get; }
    internal abstract Color MapEntry { get; }
    internal virtual bool HasMapName => false;

    protected virtual void AutoDefaults() {
        var flags = ItemGenerationFlags;
        if (!Friendly) {
            if (flags.HasFlag(GenerationFlags.HostileItem)) {
                Mod.AddContent(new AutoloadedWallItem(this, friendly: false));
            }
            if (flags.HasFlag(GenerationFlags.FriendlyWall)) {
                Mod.AddContent(new AutoloadedWallTile(Texture, HasMapName ? Name : null, DustId, MapEntry, friendly: true));
            }
        }

        if ((Friendly || flags.HasFlag(GenerationFlags.FriendlyWall)) && flags.HasFlag(GenerationFlags.FriendlyItem)) {
            Mod.AddContent(new AutoloadedWallItem(this, friendly: true));
        }
    }

    protected virtual void OnLoad() {
    }

    public sealed override void Load() {
        AutoDefaults();
        OnLoad();
    }

    public override void SetStaticDefaults() {
        Main.wallHouse[Type] = Friendly;
        DustType = DustId;
        AddMapEntry(MapEntry, HasMapName ? this.GetLocalization("DisplayName") : null);
    }

    internal virtual void AddItemRecipes(AutoloadedWallItem modItem) {
    }
}