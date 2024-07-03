namespace Aequu2;

public partial class AequusPlayer {
    public override void SetControls() {
        SetControlsInner();
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
        if (otherPlayer.TryGetModPlayer(out AequusPlayer other)) {
            MatchInfoAccessoriesInner(other);
        }
    }

    public override void ResetInfoAccessories() {
        ResetInfoAccessoriesInner();
    }
}
