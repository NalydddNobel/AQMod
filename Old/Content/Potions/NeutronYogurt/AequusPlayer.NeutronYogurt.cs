using Aequus.Core.CodeGeneration;
using System;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects(1f)]
    public float buffNeutronYogurt;

    public void UpdateNeutronYogurt() {
        if (buffNeutronYogurt <= 1f || (Player.slowFall && !Player.controlDown)) {
            return;
        }

        int velocityYDirection = Math.Sign(Player.velocity.Y);
        int gravityDirection = Math.Sign(Player.gravDir);
        if (velocityYDirection != gravityDirection) {
            return;
        }

        Player.gravity *= buffNeutronYogurt;
        if (Player.grappling[0] != -1 || (Player.mount.Active && (Player.mount.CanFly() || (Player.mount._data.swimSpeed > 0f && Player.wet)))) {
            return;
        }

        float fallSpeed = Math.Abs(Player.velocity.Y);
        if (fallSpeed < Player.maxFallSpeed - 0.01f) {
            float yOffset = gravityDirection == 1 ? Player.height : 0;
            float fallProgress = fallSpeed / Player.maxFallSpeed;
            float wavePattern = fallProgress * MathHelper.TwoPi;
            for (int i = 0; i < 2; i++) {
                float waveOffset = MathF.Sin(wavePattern + MathHelper.Pi * i);
                var d = Dust.NewDustPerfect(new Vector2(Player.position.X + Player.width / 2f + Player.width * waveOffset, Player.position.Y + yOffset), DustID.CrystalSerpent_Pink, Scale: fallProgress * 0.5f + 0.7f);
                d.noGravity = true;
                d.noLight = true;
                d.velocity *= 0.1f;
                d.velocity.X += Player.velocity.X;
            }
        }
    }
}