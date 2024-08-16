using Aequus.Content.Villagers.SkyMerchant;
using Terraria.Map;
using Terraria.UI;

namespace Aequus.Content.Items.Accessories.Informational.Calendar;

public class CalendarMapDisplay : ModMapLayer {
    public override void Draw(ref MapOverlayDrawContext context, ref string text) {
        // The Sky Merchant only appears during the day.
        // Also only visible if the player has the Calendar accessory.
        if (!Main.dayTime || !Calendar.IsInfoVisible(Main.LocalPlayer) || NPC.AnyNPCs(ModContent.NPCType<SkyMerchant>())) {
            return;
        }

        Vector2 drawCoordinates = new Vector2(SkyMerchantSystem.SkyMerchantX, -10f);
        var result = context.Draw(AequusTextures.CalendarMapDisplay, drawCoordinates, Alignment.Center);
        if (result.IsMouseOver) {
            text = ModContent.GetInstance<SkyMerchant>().DisplayName.Value;
        }
    }
}
