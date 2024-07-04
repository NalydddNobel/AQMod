using AequusRemake.Core.Entities.Players;

namespace AequusRemake;

public partial class AequusPlayer {
    public CustomDashData DashData { get; private set; }

    public void SetDashData<T>() where T : CustomDashData {
        DashData = ModContent.GetInstance<T>();
    }

    internal void DoCommonDashHandle(Player player, out int dir, out bool dashing, CustomDashData dashData) {
        dir = 0;
        dashing = false;
        if (player.dashTime > 0) {
            player.dashTime--;
        }
        if (player.dashTime < 0) {
            player.dashTime++;
        }
        if (player.controlRight && player.releaseRight) {
            if (player.dashTime > 0) {
                dir = 1;
                dashing = true;
                player.dashTime = 0;
                player.timeSinceLastDashStarted = 0;
                dashData.OnHandledStart(player, this, dir);
            }
            else {
                player.dashTime = 15;
            }
        }
        else if (player.controlLeft && player.releaseLeft) {
            if (player.dashTime < 0) {
                dir = -1;
                dashing = true;
                player.dashTime = 0;
                player.timeSinceLastDashStarted = 0;
                dashData.OnHandledStart(player, this, dir);
            }
            else {
                player.dashTime = -15;
            }
        }
    }
}