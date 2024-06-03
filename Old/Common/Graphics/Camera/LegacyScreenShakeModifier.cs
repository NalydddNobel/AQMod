using Terraria.Graphics.CameraModifiers;

namespace Aequus.Old.Common.Graphics.Camera;

public class LegacyScreenShakeModifier : ICameraModifier {
    public string UniqueIdentity { get; private set; }

    public bool Finished => _intensity <= 1f;

    private readonly float MultiplyPerTick;

    private float _intensity;

    public LegacyScreenShakeModifier(float intensity, float multiplyPerTick) {
        _intensity = intensity;
        MultiplyPerTick = multiplyPerTick;
    }

    public void Update(ref CameraInfo cameraPosition) {
        cameraPosition.CameraPosition += (new Vector2(Main.rand.NextFloat(-_intensity, _intensity), Main.rand.NextFloat(-_intensity, _intensity)) * 0.5f).Floor();
        _intensity *= MultiplyPerTick;
    }
}
