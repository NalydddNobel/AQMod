using Aequu2.Content.Items.Accessories.ValentinesRing;

namespace Aequu2;

public partial class AequusPlayer {
    public void UpdateGiftRing() {
        if (string.IsNullOrEmpty(accGifterRing) || accGifterRing == Player.name) {
            return;
        }

        for (int i = 0; i < Main.maxPlayers; i++) {
            // Two players cannot have the same name in a multiplayer server.
            if (Main.player[i].active && Main.player[i].name == accGifterRing) {
                Player.AddBuff(ModContent.BuffType<ValentineRingBuff>(), 8, quiet: true);
                Main.player[i].AddBuff(ModContent.BuffType<ValentineRingBuff>(), 8, quiet: true);
                break;
            }
        }
    }
}