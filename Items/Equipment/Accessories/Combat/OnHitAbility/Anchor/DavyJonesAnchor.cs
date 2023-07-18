using Aequus.Common.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.Anchor;

public class DavyJonesAnchor : ModItem, IDavyJonesAnchor {
    public virtual int AnchorSpawnChance => 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.ChanceFracPercent(AnchorSpawnChance));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemDefaults.RarityCrabCrevice;
        Item.value = ItemDefaults.ValueCrabCrevice;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.Aequus().accDavyJonesAnchor.SetAccessory(Item, this);
    }
}