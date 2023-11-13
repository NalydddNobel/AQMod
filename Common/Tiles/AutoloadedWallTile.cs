using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

[Autoload(false)]
internal class AutoloadedWallTile : ModWall {
    private readonly string TexturePath;
    private readonly string DisplayNameKey;
    private readonly int DustId;
    private readonly Color? MapEntry;
    private readonly bool Friendly;

    private AutoloadedWallTile() {
    }

    public AutoloadedWallTile(string texturePath, string displayNameKey = null, int dustId = 0, Color? mapEntry = null, bool friendly = false) {
        TexturePath = texturePath;
        DisplayNameKey = displayNameKey;
        DustId = dustId;
        MapEntry = mapEntry;
        Friendly = friendly;
    }

    public override string Texture => TexturePath;

    public override void SetStaticDefaults() {
        Main.wallHouse[Type] = Friendly;
        DustType = DustId;
        if (MapEntry.HasValue) {
            AddMapEntry(MapEntry.Value, DisplayNameKey != null ? Language.GetText($"Mods.{Mod.Name}.{LocalizationCategory}.{DisplayNameKey}.DisplayName") : null);
        }
    }
}