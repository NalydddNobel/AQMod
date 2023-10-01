using Aequus.Common.Players.Attributes;
using Aequus.Content.Items.Equipment.Accessories.Restoration.GoldenFeather;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public Item accGoldenFeather;

    private void PostUpdateEquips_TeamEffects_GoldenFeather(Player teammate, AequusPlayer teammateAequusPlayer) {
        if (teammateAequusPlayer.accGoldenFeather?.ModItem is GoldenFeather goldenFeather) {
            Player.AddBuff(goldenFeather.BuffType, 16, quiet: true);
        }
    }
}