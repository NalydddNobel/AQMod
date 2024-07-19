namespace Aequus.Common.Structures.ID;

internal readonly record struct ModId<T>(string Name, Mod Mod, int BackupVanillaId) : IProvideId where T : IModType {
    public ModId(string Name, string Mod, int BackupVanillaId) : this(Name, ModLoader.TryGetMod(Mod, out Mod mod) ? mod : null, BackupVanillaId) { }

    public int GetId() {
        if (Mod?.TryFind(Name, out T value) == true) {
            return ((dynamic)value).Type;
        }

        return BackupVanillaId;
    }
}
