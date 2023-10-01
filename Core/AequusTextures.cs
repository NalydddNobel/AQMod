using Aequus.Common;
using Aequus.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus;

public partial class AequusTextures : AssetLoader<TextureAsset, Texture2D> {
    #region Get Paths
    public const string TemporaryBuffIcon = "Terraria/Images/Buff_188";
    public const string TemporaryDebuffIcon = "Terraria/Images/Buff_164";

    public static string Tile(int id) {
        return $"Terraria/Images/Tiles_{id}";
    }

    public static string Extra(int id) {
        return $"Terraria/Images/Extra_{id}";
    }

    public static string Item(int id) {
        return $"Terraria/Images/Item_{id}";
    }

    public static string NPC(int id) {
        return $"Terraria/Images/NPC_{id}";
    }

    public static string Projectile(int id) {
        return $"Terraria/Images/Projectile_{id}";
    }
    #endregion
}