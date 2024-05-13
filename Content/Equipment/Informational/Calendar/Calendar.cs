using Aequus.Core.CodeGeneration;

namespace Aequus.Content.Equipment.Informational.Calendar;

[InfoPlayerField("accInfoDayCalendar", "bool")]
public class Calendar : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishermansGuide);
        Item.DefaultToPlaceableTile(ModContent.TileType<CalendarTile>());
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoDayCalendar = true;
    }
}