using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal interface IPostSetupContent : ILoadable {
    void PostSetupContent(Aequus aequus);
}
