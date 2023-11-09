using Aequus.Core.Generator;
using Aequus.Content.Items.Equipment.Accessories.Informational.Monocle;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool accMonocle;
    [ResetEffects]
    public bool accShimmerMonocle;

    public bool ShowMonocle => accMonocle && ModContent.GetInstance<MonocleBuilderToggle>().CurrentState == 0;
    public bool ShowShimmerMonocle => accShimmerMonocle && ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState == 0;
}