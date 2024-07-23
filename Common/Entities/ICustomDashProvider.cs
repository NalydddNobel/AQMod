namespace Aequus.Common.Entities;

public interface ICustomDashProvider {

    float DashSpeed { get; }

    virtual bool ShowShield => false;

    virtual void OnHandledStart(Player player, int direction) { }

    virtual void OnDashVelocityApplied(Player player, int direction) { }

    virtual void OnApplyDash(Player player) { }

    virtual void OnUpdateRampDown(Player player) { }

    virtual void OnUpdateDashDelay(Player player) { }

    virtual void OnPlayerFrame(Player player) {
        if (ShowShield) {
            if (player.velocity.Y != 0f) {
                player.bodyFrame.Y = player.bodyFrame.Height * 6;
            }
        }
    }

    virtual void PreUpdateVisibleAccessories(Player player) {
        if (ShowShield) {
            player.eocDash = 1;
        }
    }

    virtual void PostUpdateVisibleAccessories(Player player) {
        if (ShowShield) {
            player.eocDash = 0;
        }
    }
}
