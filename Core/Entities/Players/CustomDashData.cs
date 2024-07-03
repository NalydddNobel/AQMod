namespace Aequu2.Core.Entities.Players;

public abstract class CustomDashData : ModType {
    protected sealed override void Register() {
        ModTypeLookup<CustomDashData>.Register(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    public abstract float DashSpeed { get; }
    public float DashHaltSpeed => 12f;
    /// <summary>
    /// Multiplier used to reduce velocity when above the dash's halting speed.
    /// </summary>
    public float DashHaltSpeedMultiplier => 0.992f;

    public int DashDelay => 20;

    public virtual bool ShowShield => false;

    public virtual void OnHandledStart(Player player, AequusPlayer Aequu2Player, int direction) {
    }

    public virtual void OnDashVelocityApplied(Player player, AequusPlayer Aequu2Player, int direction) {
    }

    public virtual void OnApplyDash(Player player, AequusPlayer Aequu2Player) {
    }

    public virtual void OnUpdateRampDown(Player player, AequusPlayer Aequu2Player) {
    }

    public virtual void OnUpdateDashDelay(Player player, AequusPlayer Aequu2Player) {
    }

    public virtual void OnPlayerFrame(Player player, AequusPlayer Aequu2Player) {
        if (ShowShield) {
            if (player.velocity.Y != 0f) {
                player.bodyFrame.Y = player.bodyFrame.Height * 6;
            }
        }
    }

    public virtual void PreUpdateVisibleAccessories(Player player, AequusPlayer Aequu2Player) {
        if (ShowShield) {
            player.eocDash = 1;
        }
    }

    public virtual void PostUpdateVisibleAccessories(Player player, AequusPlayer Aequu2Player) {
        if (ShowShield) {
            player.eocDash = 0;
        }
    }
}