using Aequus.Content.Equipment.Accessories.Informational.Monocle;
using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool accMonocle;
    [ResetEffects]
    public bool accShimmerMonocle;

    public bool ShowMonocle => accMonocle && ModContent.GetInstance<MonocleBuilderToggle>().CurrentState == 0;
    public bool ShowShimmerMonocle => accShimmerMonocle && ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState == 0;
}