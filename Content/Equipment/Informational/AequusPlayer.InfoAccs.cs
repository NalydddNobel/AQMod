using Aequus.Content.Equipment.Informational.Monocle;

namespace Aequus;

public partial class AequusPlayer {
    public bool accInfoDayCalendar;

    public bool accInfoDebuffDPS;

    public bool accInfoMoneyMonocle;
    public bool accInfoShimmerMonocle;

    public bool ShowMoneyMonocle => accInfoMoneyMonocle && ModContent.GetInstance<MonocleBuilderToggle>().CurrentState == 0;
    public bool ShowShimmerMonocle => accInfoShimmerMonocle && ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState == 0;

    public override void ResetInfoAccessories() {
        accInfoDayCalendar = false;
        accInfoDebuffDPS = false;
        accInfoMoneyMonocle = false;
        accInfoShimmerMonocle = false;
#if !DEBUG
        accInfoQuestFish = false;
#endif
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
        if (otherPlayer.TryGetModPlayer(out AequusPlayer otherAequusPlayer)) {
            InheritInfoAccs(otherAequusPlayer);
        }
    }

    private void InheritInfoAccs(AequusPlayer other) {
        // TODO -- Automate this?
        accInfoMoneyMonocle |= other.accInfoMoneyMonocle;
        accInfoShimmerMonocle |= other.accInfoShimmerMonocle;
        accInfoDayCalendar |= other.accInfoDayCalendar;
        accInfoDebuffDPS |= other.accInfoDebuffDPS;
#if !DEBUG
        accInfoQuestFish |= other.accInfoQuestFish;
#endif
    }
}