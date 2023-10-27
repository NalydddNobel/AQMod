using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class Scavenger {
    public delegate void AccessoryUpdate(Scavenger scavenger, bool attacking);

    public static readonly Dictionary<int, AccessoryUpdate> CustomAccessoryUsage = new();

    public float accessoryUseData;

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

                bool playSound = true;
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

    private void LoadAccessoryUsages() {
        CustomAccessoryUsage[ItemID.TsunamiInABottle] = AccessoryUsageBottleJump<TsunamiInABottleJump>();
    }
}