using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal interface IPostPopulateItemDropDatabase : ILoadable {
    void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database);
}
