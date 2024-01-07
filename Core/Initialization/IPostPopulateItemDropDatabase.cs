using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Initialization;

internal interface IPostPopulateItemDropDatabase : ILoadable {
    void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database);
}
