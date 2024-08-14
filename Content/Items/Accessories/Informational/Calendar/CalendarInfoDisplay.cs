using Aequus.Systems;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip = LocalizedText.Empty;

    public override void SetStaticDefaults() {
        Tooltip = this.GetLocalization("Tooltip");
    }

    public override bool Active() {
        return CalendarSystem.IsCalendarNearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoDayCalendar;
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        return string.Format(Tooltip.Value, TimeSystem.GetWeekText(TimeSystem.DayOfTheWeek).Value, TimeSystem.DaysPassed);
    }
}