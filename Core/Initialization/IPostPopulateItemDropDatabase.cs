using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Initialization;

internal interface IPostPopulateItemDropDatabase : ILoad {
    void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database);
}
