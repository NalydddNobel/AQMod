namespace Aequus.Old.Content.Items.Potions;

public class PotionSystem : ModSystem {
    public override void ModifyLightingBrightness(ref float scale) {
        if (Main.LocalPlayer.TryGetModPlayer(out PotionsPlayer potionPlayer) && potionPlayer.empoweredPotionId == BuffID.NightOwl) {
            // Pray this isn't buggy...
            scale = 1.06f;
        }
    }
}
