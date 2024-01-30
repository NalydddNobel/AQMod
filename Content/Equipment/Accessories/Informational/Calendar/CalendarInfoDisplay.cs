using Aequus.Common.Systems;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip;

    public override void Load() {
    }

    public override System.Boolean Active() {
        return CalendarTile.Nearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accDayCalendar;
    }

    public override System.String DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        Tooltip ??= this.GetLocalization("Tooltip");
        return System.String.Format(Tooltip.Value, ExtendLanguage.DayOfWeek(TimeTrackerSystem.DayOfTheWeek).Value, TimeTrackerSystem.daysPassed);
    }
}