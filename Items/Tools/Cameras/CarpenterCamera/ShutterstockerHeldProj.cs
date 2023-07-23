using Terraria.ModLoader;

namespace Aequus.Items.Tools.Cameras.CarpenterCamera; 

public class ShutterstockerHeldProj : CameraHeldProj {
    public override int ShootProj => ModContent.ProjectileType<ShutterstockerCameraProj>();
}