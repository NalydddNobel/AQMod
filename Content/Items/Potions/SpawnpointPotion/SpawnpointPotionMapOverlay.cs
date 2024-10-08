using Terraria.Map;
using Terraria.UI;

namespace Aequus.Content.Items.Potions.SpawnpointPotion;

public class SpawnpointPotionMapOverlay : ModMapLayer {
    public override void Draw(ref MapOverlayDrawContext context, ref string text) {
        if (!Main.LocalPlayer.TryGetModPlayer(out SpawnpointPotionPlayer beaconPlayer) || !beaconPlayer.HasValidBeaconSpawnpoint) {
            return;
        }

        if (context.Draw(AequusTextures.SpawnpointPotionMapIcon, beaconPlayer.beaconPos.ToVector2(), Alignment.Center).IsMouseOver) {
            text = Instance<SpawnpointPotion>().GetLocalizedValue("MapIcon");
        }
    }
}
