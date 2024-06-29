global using static Aequus.Core.Utilities.TextureRedirect;
using ReLogic.Content;
using Terraria.GameContent;

namespace Aequus.Core.Utilities;

/// <summary>Lazy shorthand workaround for utilizing fields found in <see cref="TextureAssets"/>.</summary>
public class TextureRedirect {
    public static Asset<Texture2D>[] ProjectileTexture => TextureAssets.Projectile;
    public static Asset<Texture2D>[] ItemTexture => TextureAssets.Item;
    public static Asset<Texture2D>[] NPCTexture => TextureAssets.Npc;
    public static Asset<Texture2D>[] TileTexture => TextureAssets.Tile;

    public static Texture2D GetTileTexture(int i, int j) {
        return GetTileTexture(i, j, Main.tile[i, j]);
    }
    public static Texture2D GetTileTexture(int i, int j, Tile tile) {
        return Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);
    }

    public static Color GetTileLight(int i, int j) {
        return GetTileColor(i, j, Main.tile[i, j]);
    }
    public static Color GetTileColor(int i, int j, Tile tile) {
        if (tile.IsTileFullbright) {
            return Color.White;
        }

        return Lighting.GetColor(i, j);
    }
}
