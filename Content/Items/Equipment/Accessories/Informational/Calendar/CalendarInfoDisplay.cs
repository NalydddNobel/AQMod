using Aequus.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip;

    public override void Load() {
    }

    public override bool Active() {
        return CalendarTile.Nearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accDayCalendar;
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)/* tModPorter Suggestion: Set displayColor to InactiveInfoTextColor if your display value is "zero"/shows no valuable information */ {
        Tooltip ??= this.GetLocalization("Tooltip");
        string dayOfTheWeekText = Language.GetTextValue("Mods.Aequus.Misc.DayOfTheWeek." + TimeTrackerSystem.DayOfTheWeek.ToString());
        return string.Format(Tooltip.Value, dayOfTheWeekText, TimeTrackerSystem.daysPassed);
    }
}