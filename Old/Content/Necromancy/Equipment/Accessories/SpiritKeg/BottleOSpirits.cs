using Aequus.Content.Events.DemonSiege;

namespace Aequus.Old.Content.Necromancy.Equipment.Accessories.SpiritKeg;

[LegacyName("Malediction", "CartilageRing", "NaturesCruelty")]
public class BottleOSpirits : ModItem {
    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ModContent.ItemType<BottleOSpirits>(), Type);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
    }
}