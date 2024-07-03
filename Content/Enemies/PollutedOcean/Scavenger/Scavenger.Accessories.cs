using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequu2.Content.Enemies.PollutedOcean.Scavenger;

public partial class Scavenger {
    public delegate void AccessoryUpdate(Scavenger scavenger, bool attacking);

    public static readonly Dictionary<int, AccessoryUpdate> CustomAccessoryUsage = new();

    public float accessoryUseData;
    public float acceleration;
    public float runSpeedCap;

    public static AccessoryUpdate AccessoryUsageBottleJump<T>() where T : ExtraJump {
        var extraJump = ModContent.GetInstance<T>();
        return (s, attacking) => {
            var npc = s.NPC;
            if (npc.HasValidTarget) {
                return;
            }

            Player target = Main.player[npc.target];

            if (npc.velocity.Y == 0f) {
                if (s.accessoryUseData != 0f) {
                    s.accessoryUseData = 0f;
                    npc.netUpdate = true;
                }
                return;
            }

            if (s.accessoryUseData == 0f && npc.velocity.Y > 1f && npc.Bottom.Y > target.Bottom.Y) {
                s.accessoryUseData = 1f;
                npc.velocity.Y = -8f;
                npc.netUpdate = true;

                bool playSound = true;
                extraJump.OnStarted(s.playerDummy, ref playSound);
                if (playSound && Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height)) {
                    SoundEngine.PlaySound(SoundID.DoubleJump, npc.Center);
                }

            }
            if (s.accessoryUseData != 0f && npc.velocity.Y < 0f) {
                extraJump.ShowVisuals(s.playerDummy);
            }
        };
    }

    public static AccessoryUpdate AccessoryUsageBoots(int dustID) {
        return (s, attacking) => {
            var npc = s.NPC;
            if (npc.direction != Math.Sign(npc.velocity.X)) {
                npc.velocity.X += s.playerDummy.runSlowdown * npc.direction;
                return;
            }
            if (Math.Abs(npc.velocity.X) > s.SpeedCap) {
                npc.velocity.X += s.playerDummy.runSlowdown * -npc.direction;
            }
            if (npc.velocity.Y != 0f || Math.Abs(npc.velocity.X) < s.SpeedCap - s.runSpeedCap) {
                return;
            }
            s.accessoryUseData++;
            if (s.accessoryUseData % 10 == 0) {
                SoundEngine.PlaySound(SoundID.Run, npc.Center);
            }
            for (int i = 0; i < 4; i++) {
                var d = Terraria.Dust.NewDustDirect(new Vector2(npc.position.X - 4f, npc.position.Y), npc.width + 8, npc.height, dustID, (0f - npc.velocity.X) * 0.5f, npc.velocity.Y * 0.5f, 100, Scale: 1.5f + Main.rand.Next(-5, 3) * 0.1f);
                d.noGravity = true;
                d.velocity *= 0.2f;
            }
        };
    }

    private void SetupAccessoryUsages() {
        CustomAccessoryUsage[ItemID.TsunamiInABottle] = AccessoryUsageBottleJump<TsunamiInABottleJump>();
        CustomAccessoryUsage[ItemID.SailfishBoots] = AccessoryUsageBoots(DustID.SailfishBoots);
    }
}