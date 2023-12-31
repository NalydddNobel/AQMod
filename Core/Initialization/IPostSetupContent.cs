namespace Aequus.Core.Initialization;

internal interface IPostSetupContent : ILoadable {
    void PostSetupContent(Aequus aequus);
}
