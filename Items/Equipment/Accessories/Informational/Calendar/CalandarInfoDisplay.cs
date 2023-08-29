using Aequus.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Informational.Calendar;

public class CalandarInfoDisplay : InfoDisplay {
    public override bool Active() {
        return CalendarTile.Nearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accDayCalendar;
    }

    public override string DisplayValue(ref Color displayColor) {
        return Language.GetTextValue("Mods.Aequus.Misc.DayOfTheWeek." + TimeTrackerSystem.DayOfTheWeek.ToString());
    }
}
