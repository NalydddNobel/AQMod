using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Autoloading;

internal interface IPreExtractBestiaryItemDrops : ILoadable {
    void PreExtractBestiaryItemDrops(Aequus aequus, BestiaryDatabase bestiaryDatabase, ItemDropDatabase database);
}