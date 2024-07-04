using AequusRemake.Content.Items.Accessories.GoldenFeather;
using AequusRemake.Core.CodeGeneration;

namespace AequusRemake;

public partial class AequusPlayer {
    [ResetEffects]
    public Item accGoldenFeather;

    private void UpdateGoldenFeather(Player teammate, AequusPlayer teammateAequusRemakePlayer) {
        if (teammateAequusRemakePlayer.accGoldenFeather?.ModItem is GoldenFeather goldenFeather) {
            Player.AddBuff(goldenFeather.BuffType, 16, quiet: true);
        }
    }
}