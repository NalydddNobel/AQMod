using Aequus.Content.Graphics.Particles;
using Aequus.Core.CodeGeneration;
using System;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public int accRestoreBreathOnKill;

    internal void RestoreBreathOnBrokenTile(int X, int Y) {
        if (accRestoreBreathOnKill <= 0 || Player.breath >= Player.breathMax) {
            return;
        }

        Player.breath += Math.Max(Player.breathMax / 15, 1) * accRestoreBreathOnKill;
        if (Player.breath > Player.breathMax) {
            Player.breath = Player.breathMax;
        }

        if (Main.netMode != NetmodeID.Server) {
            Vector2 tileCenter = new Vector2(X, Y).ToWorldCoordinates();
            foreach (var particle in ModContent.GetInstance<UnderwaterBubbleParticles>().NewMultipleReduced(12, 3)) {
                particle.Location = tileCenter;
                particle.Frame = (byte)Main.rand.Next(2);
                particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.3f);
                particle.Velocity.Y = -Math.Abs(particle.Velocity.Y);
                particle.UpLift = (1f - particle.Velocity.X) * 0.002f;
                particle.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }

            var bigBubble = ModContent.GetInstance<UnderwaterBubbleParticles>().New();
            bigBubble.Location = tileCenter;
            bigBubble.Frame = (byte)Main.rand.Next(3, 6);
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.005f;
        }
    }
}