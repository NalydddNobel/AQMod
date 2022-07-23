using Aequus.Projectiles.Misc.Drones;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons
{
    public class GunnerDroneType : DroneType
    {
        public override int ProjectileType => ModContent.ProjectileType<GunnerDrone>();
    }
}