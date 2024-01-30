using Aequus.Core.Assets;

namespace Aequus;

public sealed partial class AequusTextures : AssetManager<Texture2D> {
    public const System.String TemporaryBuffIcon = "Terraria/Images/Buff_188";
    public const System.String TemporaryDebuffIcon = "Terraria/Images/Buff_164";

    public static System.String Tile(System.Int32 id) {
        return $"Terraria/Images/Tiles_{id}";
    }

    public static System.String Extra(System.Int32 id) {
        return $"Terraria/Images/Extra_{id}";
    }

    public static System.String Item(System.Int32 id) {
        return $"Terraria/Images/Item_{id}";
    }

    public static System.String NPC(System.Int32 id) {
        return $"Terraria/Images/NPC_{id}";
    }

    public static System.String Projectile(System.Int32 id) {
        return $"Terraria/Images/Projectile_{id}";
    }
}