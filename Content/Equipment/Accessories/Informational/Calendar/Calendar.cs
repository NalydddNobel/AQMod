using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Content.Equipment.Accessories.Informational.Calendar;

public class Calendar : ModItem {
    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishermansGuide);
        Item.DefaultToPlaceableTile(ModContent.TileType<CalendarTile>());
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accDayCalendar = true;
    }
}