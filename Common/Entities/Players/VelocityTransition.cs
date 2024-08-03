namespace Aequus.Common.Entities.Players;
public class VelocityTransition : ModPlayer {
    public Vector2 Value;

    public override void PostUpdateMiscEffects() {
        if ((Value - Player.velocity).Length() < 0.01f) {
            Value = Player.velocity;
        }
        Value = Vector2.Lerp(Value, Player.velocity, 0.25f);
    }
}
