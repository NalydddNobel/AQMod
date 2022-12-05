using Aequus.NPCs.Friendly.Town.Drones;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons
{
    public class GunnerDroneSlot : DroneSlot
    {
        public override int NPCType => ModContent.NPCType<GunnerDrone>();
    }
}