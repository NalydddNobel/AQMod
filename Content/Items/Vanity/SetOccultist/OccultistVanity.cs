using Aequus.Common.ContentTemplates;
using Aequus.Common.ContentTemplates.Armor;

namespace Aequus.Content.Items.Vanity.SetOccultist;

public class OccultistVanity : UnifiedVanitySet {
    public ModItem? Hood { get; private set; }
    public ModItem? Robes { get; private set; }

    public override void Load() {
        Hood = this.AddContent(new InstancedArmorPiece(this, nameof(Hood), new(
            EquipTypes: [EquipType.Head],
            Price: Item.buyPrice(gold: 4, silver: 50),
            Vanity: true
        )));
        Robes = this.AddContent(new InstancedArmorPiece(this, nameof(Robes), new(
            EquipTypes: [EquipType.Body, EquipType.Legs],
            Price: Item.buyPrice(gold: 4, silver: 50),
            Vanity: true
        )));
    }
}
