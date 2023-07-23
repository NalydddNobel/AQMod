using Terraria.ModLoader;

namespace Aequus.Items.Tools.Cameras.MapCamera;

public class PixelCameraHeldProj : CameraHeldProj {
    public override int ShootProj => ModContent.ProjectileType<PixelCameraProj>();
}