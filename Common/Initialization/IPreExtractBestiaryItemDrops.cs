using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus;

internal interface IPreExtractBestiaryItemDrops : ILoadable {
    void PreExtractBestiaryItemDrops(BestiaryDatabase bestiaryDatabase, ItemDropDatabase database);
}