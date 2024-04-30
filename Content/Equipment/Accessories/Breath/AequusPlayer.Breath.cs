using Aequus.Content.Equipment.Accessories.Breath;
using Aequus.Content.Graphics.Particles;
using Aequus.Core.CodeGeneration;
using System;

namespace Aequus;

public partial class AequusPlayer {
    public static readonly Color CombatText_RestoreBreath = new Color(100, 150, 150);

    public Item accBreathRestore;
    [ResetEffects]
    public int accBreathRestoreStacks;

    internal void RestoreBreathOnBrokenTile(int X, int Y) {
        if (accBreathRestoreStacks <= 0 || Player.breath >= Player.breathMax) {
            return;
        }

        int restoreBreath = Math.Max((int)(Player.breathMax * BreathConserver.RestoreBreathMaxOnTileBreak), 1) * accBreathRestoreStacks;
        Player.HealBreath(restoreBreath);

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

    internal void RestoreBreathOnKillNPC(in KillInfo info) {
        if (accBreathRestoreStacks <= 0) {
            return;
        }

        Player targetPlayer = null;
        int lowestBreath = int.MaxValue;
        foreach (Player nearby in Player.FindNearbyPlayersOnTeam(1000f)) {
            if (nearby.breath < lowestBreath && nearby.breath < nearby.breathMax) {
                lowestBreath = nearby.breath;
                targetPlayer = nearby;
            }
        }

        if (targetPlayer == null) {
            return;
        }

        if (Main.myPlayer == Player.whoAmI) {
            Projectile.NewProjectile(Player.GetSource_Accessory(accBreathRestore), info.Center, Vector2.UnitY, ModContent.ProjectileType<BreathConserverProj>(), 0, 0f, Owner: Player.whoAmI, ai0: targetPlayer.whoAmI);
        }

        if (Main.netMode != NetmodeID.Server) {
            NPC npc = ContentSamples.NpcsByNetId[info.Type];
            int bubbleCount = Math.Clamp((int)(npc.Size.Length() / 4f), 12, 80);
            Vector2 bubbleSpawnSize = npc.Size / 4f;
            foreach (var particle in ModContent.GetInstance<UnderwaterBubbleParticles>().NewMultipleReduced(bubbleCount, bubbleCount / 4)) {
                particle.Location = info.Center + Main.rand.NextVector2Unit() * bubbleSpawnSize;
                particle.Frame = (byte)Main.rand.Next(2);
                particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.3f);
                particle.Velocity.Y = -Math.Abs(particle.Velocity.Y);
                particle.UpLift = (1f - particle.Velocity.X) * 0.002f;
                particle.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }

            var bigBubble = ModContent.GetInstance<UnderwaterBubbleParticles>().New();
            bigBubble.Location = info.Center;
            bigBubble.Frame = (byte)Main.rand.Next(3, 6);
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.005f;
        }
    }
}