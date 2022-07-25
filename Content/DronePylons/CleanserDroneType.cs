using Aequus.Projectiles.Misc.Drones;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons
{
    public class CleanserDroneType : DroneType
    {
        public override int ProjectileType => ModContent.ProjectileType<CleanserDrone>();
    }
}