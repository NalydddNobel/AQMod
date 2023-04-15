using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    public class SlotMachineSystem : ModSystem {
        public static float Time;
        public static float TimeSpeed;

        public static List<int> DefaultPotions { get; private set; }

        public override void Load() {
            DefaultPotions = new List<int>()
            {
                ItemID.ShinePotion,
                ItemID.NightOwlPotion,
                ItemID.SwiftnessPotion,
                ItemID.ArcheryPotion,
                ItemID.GillsPotion,
                ItemID.HunterPotion,
                ItemID.MiningPotion,
                ItemID.TrapsightPotion,
                ItemID.RegenerationPotion,
            };
        }

        public override void Unload() {
            DefaultPotions?.Clear();
            DefaultPotions = null;
        }

        public override void UpdateUI(GameTime gameTime) {
            Time += (TimeSpeed + 1f) / 60f;
            if (TimeSpeed > 0f) {
                TimeSpeed -= 0.015f;
                if (TimeSpeed > 1f) {
                    TimeSpeed *= 0.99f;
                }
                else if (TimeSpeed < 0f) {
                    TimeSpeed = 0f;
                }
            }
        }
    }
}