using Aequus.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip;

    public override void Load() {
    }

    public override bool Active() {
        return CalendarTile.Nearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accDayCalendar;
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        Tooltip ??= this.GetLocalization("Tooltip");
        return string.Format(Tooltip.Value, TextHelper.DayOfWeek(TimeTrackerSystem.DayOfTheWeek).Value, TimeTrackerSystem.daysPassed);
    }
}