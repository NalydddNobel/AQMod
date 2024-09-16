using Terraria.GameContent.ItemDropRules;

namespace Aequus;

internal interface IPostPopulateItemDropDatabase : ILoadable {
    void PostPopulateItemDropDatabase(ItemDropDatabase database);
}
