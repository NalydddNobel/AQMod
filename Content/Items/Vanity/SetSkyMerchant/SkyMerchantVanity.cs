using Aequus.Common.ContentTemplates;
using Aequus.Common.ContentTemplates.Armor;

namespace Aequus.Content.Items.Vanity.SetSkyMerchant;
internal class SkyMerchantVanity : UnifiedVanitySet {
    public ModItem? Hat { get; private set; }

    public override void Load() {
        Hat = this.AddContent(new InstancedArmorPiece(this, nameof(Hat), new(
            EquipTypes: [EquipType.Head],
            Price: Item.buyPrice(gold: 3),
            Vanity: true
        )));
    }

    public override void SetStaticDefaults() {
        ArmorIDs.Head.Sets.DrawHatHair[Hat!.Item.headSlot] = true;
    }
}
