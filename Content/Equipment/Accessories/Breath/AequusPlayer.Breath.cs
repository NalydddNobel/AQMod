using Aequus.Content.Graphics.Particles;
using Aequus.Core.CodeGeneration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria.GameContent.Achievements;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public int accRestoreBreathOnKill;

    private void RestoreBreathOnBrokenTile(int X, int Y) {
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

    private void IL_Player_PickTile(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchPropertySetter(typeof(AchievementsHelper), nameof(AchievementsHelper.CurrentlyMining)))) {
            Mod.Logger.Error("Could not find mining AchievementsHelper.CurrentlyMining setter in Player.PickTile."); return;
        }

        c.Emit(OpCodes.Ldarg_0); // Player
        c.Emit(OpCodes.Ldarg_1); // X
        c.Emit(OpCodes.Ldarg_2); // Y
        c.Emit(OpCodes.Ldarg_3); // Pickaxe Power
        c.EmitDelegate((Player player, int X, int Y, int PickPower) => {
            this.accGifterRing = "";
            player.GetModPlayer<AequusPlayer>().RestoreBreathOnBrokenTile(X, Y);
        });
    }
}