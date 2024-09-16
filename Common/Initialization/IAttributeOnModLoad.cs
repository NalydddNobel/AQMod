namespace Aequus.Common.Initialization;

/// <summary>Apply to an <see cref="System.Attribute"/> in order to have them define <see cref="OnModLoad(Mod, ILoadable)"/> to run on <see cref="ILoadable"/> types when the mod loads.</summary>
public interface IAttributeOnModLoad {
    void OnModLoad(Mod mod, ILoadable Content);
}
