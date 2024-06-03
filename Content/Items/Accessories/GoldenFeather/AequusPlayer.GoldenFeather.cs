using Aequus.Content.Items.Accessories.GoldenFeather;
using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public Item accGoldenFeather;

    private void UpdateGoldenFeather(Player teammate, AequusPlayer teammateAequusPlayer) {
        if (teammateAequusPlayer.accGoldenFeather?.ModItem is GoldenFeather goldenFeather) {
            Player.AddBuff(goldenFeather.BuffType, 16, quiet: true);
        }
    }
}