namespace Aequus.Content.Items.Accessories.Informational.Calendar;

public class CalendarSystem : ModSystem {
    public static bool IsCalendarNearby { get; internal set; }

    public override void ResetNearbyTileEffects() {
        IsCalendarNearby = false;
    }
}
