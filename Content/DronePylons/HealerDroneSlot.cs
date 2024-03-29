﻿using Aequus.Content.DronePylons.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons {
    public class HealerDroneSlot : DroneSlot
    {
        public override int NPCType => ModContent.NPCType<HealerDrone>();

        public override void OnHardUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            var droneManager = GetDronePoint();
            if (!droneManager.isActive)
            {
                var townNPCs = droneManager.NearbyTownNPCs;
                if (townNPCs.Count > 0)
                {
                    while (townNPCs.Count > 0)
                    {
                        int index = Main.rand.Next(townNPCs.Count);
                        var n = townNPCs[index];
                        if (n.life >= n.lifeMax)
                        {
                            townNPCs.RemoveAt(index);
                            continue;
                        }

                        n.life += 20 * droneManager.hardUpdates;
                        if (n.life > n.lifeMax)
                        {
                            n.life = n.lifeMax;
                        }
                        n.netUpdate = true;
                        break;
                    }
                }
            }
        }
    }
}