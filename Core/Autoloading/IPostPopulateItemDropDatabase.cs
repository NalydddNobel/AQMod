using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Autoloading;

internal interface IPostPopulateItemDropDatabase : ILoadable {
    void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database);
}
