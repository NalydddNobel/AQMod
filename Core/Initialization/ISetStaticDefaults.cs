namespace Aequus.Core.Initialization;

internal interface ISetStaticDefaults : ILoadable {
    void SetStaticDefaults(Aequus aequus);
}
