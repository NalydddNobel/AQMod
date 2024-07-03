using Aequu2.Content.Items.Accessories.GoldenFeather;
using Aequu2.Core.CodeGeneration;

namespace Aequu2;

public partial class AequusPlayer {
    [ResetEffects]
    public Item accGoldenFeather;

    private void UpdateGoldenFeather(Player teammate, AequusPlayer teammateAequu2Player) {
        if (teammateAequu2Player.accGoldenFeather?.ModItem is GoldenFeather goldenFeather) {
            Player.AddBuff(goldenFeather.BuffType, 16, quiet: true);
        }
    }
}