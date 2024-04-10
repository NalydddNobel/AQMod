using Aequus.Content.Equipment.Informational.Monocle;
using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool accInfoDayCalendar;

    [ResetEffects]
    public bool accInfoDPSMeterDebuff;

    [ResetEffects]
    public bool accInfoMoneyMonocle;
    [ResetEffects]
    public bool accInfoShimmerMonocle;

    public bool ShowMoneyMonocle => accInfoMoneyMonocle && ModContent.GetInstance<MonocleBuilderToggle>().CurrentState == 0;
    public bool ShowShimmerMonocle => accInfoShimmerMonocle && ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState == 0;
}