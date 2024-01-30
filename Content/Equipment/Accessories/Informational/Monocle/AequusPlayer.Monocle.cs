using Aequus.Content.Equipment.Accessories.Informational.Monocle;
using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public System.Boolean accMonocle;
    [ResetEffects]
    public System.Boolean accShimmerMonocle;

    public System.Boolean ShowMonocle => accMonocle && ModContent.GetInstance<MonocleBuilderToggle>().CurrentState == 0;
    public System.Boolean ShowShimmerMonocle => accShimmerMonocle && ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState == 0;
}