using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Players.Dashes;

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


    public virtual void OnHandledStart(Player player, AequusPlayer aequusPlayer, int direction) {
    }

    public virtual void OnDashVelocityApplied(Player player, AequusPlayer aequusPlayer, int direction) {
    }

    public virtual void OnApplyDash(Player player, AequusPlayer aequusPlayer) {
    }

    public virtual void OnUpdateRampDown(Player player, AequusPlayer aequusPlayer) {
    }

    public virtual void OnUpdateDashDelay(Player player, AequusPlayer aequusPlayer) {
    }
}