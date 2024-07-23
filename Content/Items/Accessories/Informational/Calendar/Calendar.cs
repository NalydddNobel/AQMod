using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Items.Accessories.Informational.Calendar;

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
}