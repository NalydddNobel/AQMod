namespace AequusRemake.Core.Structures.ID;

internal readonly record struct InstanceId<T>(T Instance) : IProvideId where T : class {
    public int GetId() {
        return ((dynamic)Instance).Type;
    }
}
