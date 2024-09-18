using Terraria.Localization;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.Anchor;

public class DavyJonesAnchor : ModItem, IDavyJonesAnchor {
    public virtual int AnchorSpawnChance => 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.ChanceFracPercent(AnchorSpawnChance));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.Flipper);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.Aequus().accDavyJonesAnchor.SetAccessory(Item, this);
    }
}