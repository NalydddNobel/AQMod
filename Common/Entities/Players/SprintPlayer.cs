namespace Aequus.Common.Entities.Players;

public class SprintPlayer : ModPlayer {
    /// <summary>Effects the player's sprint speed from boot accessories.</summary>
    public StatModifier SprintSpeed;
    /// <summary>Effects the player's sprint speed from boot accessories. Only if they are on the ground, however.</summary>
    public StatModifier GroundedSprintSpeed;

    public override void ResetEffects() {
        SprintSpeed = StatModifier.Default;
        GroundedSprintSpeed = StatModifier.Default;
    }

    public override void PostUpdateRunSpeeds() {
        StatModifier modifier = SprintSpeed;
        if (Player.velocity.Y == 0f) {
            modifier = modifier.CombineWith(GroundedSprintSpeed);
        }

        Player.accRunSpeed = modifier.ApplyTo(Player.accRunSpeed);
    }
}
