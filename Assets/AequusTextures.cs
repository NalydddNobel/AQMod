using Aequus.Common;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusTextures : ILoadable {
    public void Load(Mod mod) {
    }

    public void Unload() {
        foreach (var f in GetType().GetFields(BindingFlags.Public | BindingFlags.Static)) {
            if (f.FieldType == typeof(TextureAsset)) {
                ((TextureAsset)f.GetValue(this))?.Unload();
            }
        }
    }

    #region Get Paths
    public const string TemporaryBuffIcon = "Terraria/Images/Buff_188";
    public const string TemporaryDebuffIcon = "Terraria/Images/Buff_164";

    public static string Extra(int id) {
        return $"Terraria/Images/Extra_{id}";
    }

    public static string NPC(int id) {
        return $"Terraria/Images/NPC_{id}";
    }
    #endregion
}