﻿using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class Scavenger {
    public delegate void AccessoryUpdate(Scavenger scavenger, Boolean attacking);

    public static readonly Dictionary<Int32, AccessoryUpdate> CustomAccessoryUsage = new();

    public Single accessoryUseData;
    public Single acceleration;
    public Single runSpeedCap;

    public static AccessoryUpdate AccessoryUsageBottleJump<T>() where T : ExtraJump {
        var extraJump = ModContent.GetInstance<T>();
        return (s, attacking) => {
            var npc = s.NPC;
            if (npc.velocity.Y == 0f) {
                if (s.accessoryUseData != 0f) {
                    s.accessoryUseData = 0f;
                    npc.netUpdate = true;
                }
                return;
            }
            if (s.accessoryUseData == 0f && npc.velocity.Y > 1f) {
                s.accessoryUseData = 1f;
                npc.velocity.Y = -8f;
                npc.netUpdate = true;

                Boolean playSound = true;
                extraJump.OnStarted(s.playerDummy, ref playSound);
                if (playSound) {
                    SoundEngine.PlaySound(SoundID.DoubleJump, npc.Center);
                }

            }
            if (s.accessoryUseData != 0f && npc.velocity.Y < 0f) {
                extraJump.ShowVisuals(s.playerDummy);
            }
        };
    }

    public static AccessoryUpdate AccessoryUsageBoots(Int32 dustID) {
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
            for (Int32 i = 0; i < 4; i++) {
                var d = Dust.NewDustDirect(new Vector2(npc.position.X - 4f, npc.position.Y), npc.width + 8, npc.height, dustID, (0f - npc.velocity.X) * 0.5f, npc.velocity.Y * 0.5f, 100, Scale: 1.5f + Main.rand.Next(-5, 3) * 0.1f);
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