using Aequus.Common;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusTextures : ILoadable {
    public void Load(Mod mod) {
    }

    public void Unload() {
        foreach (var f in GetType().GetFields()) {
            ((TextureAsset)f.GetValue(this))?.Unload();
        }
    }

    #region Get Paths
    public const string TemporaryBuffIcon = "Terraria/Images/Buff_188";
    public const string TemporaryDebuffIcon = "Terraria/Images/Buff_164";

    public static string Extra(int id) {
        return $"Terraria/Images/Extra_{id}";
    }
    #endregion
}