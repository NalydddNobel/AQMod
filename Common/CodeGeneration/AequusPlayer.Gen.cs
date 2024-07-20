using Aequus.Common.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer : IExpandedBySourceGenerator {
    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
        if (otherPlayer.TryGetModPlayer(out AequusPlayer other)) {
            MatchInfoAccessoriesInner(other);
        }
    }

    public override void ResetInfoAccessories() {
        ResetInfoAccessoriesInner();
    }
}
