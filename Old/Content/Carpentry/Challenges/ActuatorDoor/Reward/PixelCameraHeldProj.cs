using Aequus.Old.Common.Carpentry;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward;

namespace Aequus.Items.Tools.Cameras.MapCamera;

public class PixelCameraHeldProj : CameraHeldProj {
    public override int ShootProj => ModContent.ProjectileType<PixelCameraProj>();
}