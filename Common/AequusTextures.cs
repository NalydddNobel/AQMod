using ReLogic.Content;

namespace Aequus;

public sealed partial class AequusTextures {
    public const string TemporaryBuffIcon = "Terraria/Images/Buff_188";
    public const string TemporaryDebuffIcon = "Terraria/Images/Buff_164";

    public static Asset<Texture2D>[] LensFlares { get; private set; }

    #region Frame Counts
    public const int FogFrameCount = 8;
    #endregion

    #region Texture Paths
    public static string Buff(int id) {
        return $"Terraria/Images/Buff_{id}";
    }

    public static string Tile(int id) {
        return $"Terraria/Images/Tiles_{id}";
    }

    public static string Tree_Branches(int id) {
        return $"Terraria/Images/Tree_Branches_{id}";
    }

    public static string Tree_Tops(int id) {
        return $"Terraria/Images/Tree_Tops_{id}";
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