namespace Aequus.Core.Initialization;

internal interface IOnModLoad : ILoad {
    void OnModLoad(Aequus aequus);
}
