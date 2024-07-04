namespace AequusRemake.Core.Structures.ID;

internal readonly record struct TypeId<T>(T Instance) : IProvideId where T : class {
    public int GetId() {
        return ((dynamic)ModContent.GetInstance<T>()).Type;
    }
}
