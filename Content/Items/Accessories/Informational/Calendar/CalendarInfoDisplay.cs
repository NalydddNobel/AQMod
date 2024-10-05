using Aequus.Common.Utilities;
using Aequus.Content.Systems.Seasons;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip = LocalizedText.Empty;

    public override void SetStaticDefaults() {
        Tooltip = this.GetLocalization("Tooltip");
    }

    public override bool Active() {
        return Calendar.IsActive(Main.LocalPlayer);
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        TimeSystem time = Instance<TimeSystem>();
#if SEASONS
        if (Main.GameUpdateCount % 220 > 120) {
            return time.SeasonText.GetOrDefault(time.Season, ALanguage.UnknownText).Value;
        }
#endif

        LocalizedText weekText = time.WeekText.GetOrDefault(time.DayOfTheWeek, ALanguage.UnknownText);
        return string.Format(Tooltip.Value, weekText.Value, time.DaysPassed + 1);
    }
}