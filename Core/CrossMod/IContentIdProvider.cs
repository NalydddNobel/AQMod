namespace Aequus.Core.CrossMod;

internal interface IContentIdProvider {
    int GetId();
}

internal readonly record struct ProvideContentId(int Type) : IContentIdProvider {
    public int GetId() {
        return Type;
    }
}

internal readonly record struct ProvideModContentId<T>(string Name, Mod Mod, int BackupVanillaId) : IContentIdProvider where T : IModType {
    public ProvideModContentId(string Name, string Mod, int BackupVanillaId) : this(Name, ModLoader.TryGetMod(Mod, out Mod mod) ? mod : null, BackupVanillaId) { }

    public int GetId() {
        if (Mod?.TryFind(Name, out T value) == true) {
            return ((dynamic)value).Type;
        }

        return BackupVanillaId;
    }
}

internal readonly record struct ProvideInstanceModContentId<T>(T Instance) : IContentIdProvider where T : class {
    public int GetId() {
        return ((dynamic)Instance).Type;
    }
}

internal readonly record struct ProvideGenericTypeModContentId<T>(T Instance) : IContentIdProvider where T : class {
    public int GetId() {
        return ((dynamic)ModContent.GetInstance<T>()).Type;
    }
}
