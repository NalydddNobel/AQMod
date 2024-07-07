using AequusRemake.Core.Systems;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Accessories.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip;

    public override void SetStaticDefaults() {
        Tooltip = this.GetLocalization("Tooltip");
    }

    public override bool Active() {
        return CalendarTile.Nearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoDayCalendar;
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        return string.Format(Tooltip.Value, TimeSystem.GetWeekText(TimeSystem.DayOfTheWeek).Value, TimeSystem.DaysPassed);
    }
}