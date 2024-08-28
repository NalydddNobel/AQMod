using Terraria.Localization;

namespace Aequus.Common.ContentTemplates.Armor;

internal interface ISetbonusProvider : ILocalizedModType {
    LocalizedText? SetbonusText { get; set; }

    bool IsArmorSet(Item Head, Item Body, Item Legs);
    void UpdateArmorSet(Item Item, Player Player);
}