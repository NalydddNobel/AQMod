namespace Aequus.Core.Initialization;

internal interface IPostSetupContent : ILoad {
    void PostSetupContent(Aequus aequus);
}
