using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal interface IAddRecipeGroups : ILoadable {
    void AddRecipeGroups(Aequus aequus);
}
