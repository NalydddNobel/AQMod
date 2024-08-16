using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Content.Items.Accessories.Informational.Calendar;

[Gen.AequusPlayer_InfoField("accInfoDayCalendar")]
public class Calendar : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishermansGuide);
        Item.DefaultToPlaceableTile(ModContent.TileType<CalendarTile>());
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoDayCalendar = true;
    }

    public static bool IsActive(Player player) {
        return CalendarSystem.IsCalendarNearby || player.GetModPlayer<AequusPlayer>().accInfoDayCalendar;
    }

    public static bool IsInfoVisible(Player player) {
        return IsActive(player) && !player.hideInfo[ModContent.GetInstance<CalendarInfoDisplay>().Type];
    }
}