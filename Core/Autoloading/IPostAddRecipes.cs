using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal interface IPostAddRecipes : ILoadable {
    void PostAddRecipes(Aequus aequus);
}
