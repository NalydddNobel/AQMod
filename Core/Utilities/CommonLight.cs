namespace Aequu2.Core.Utilities;

public class CommonLight {
    public static readonly Vector3 ShadowOrb = new Vector3(0.6f, 0.1f, 1f);
    public static readonly Vector3 CrimsonHeart = new Vector3(1f, 0.11f, 0.33f);
    public static readonly Vector3 BlueFairy = new Vector3(0.45f, 0.75f, 1f);
    public static readonly Vector3 PinkFairy = new Vector3(1f, 0.45f, 0.75f);
    public static readonly Vector3 GreenFairy = new Vector3(0.45f, 1f, 0.75f);
    public static readonly Vector3 Flickerwick = new Vector3(0.3f, 0.5f, 1f);
    public static readonly Vector3 WispInABottle = new Vector3(0.5f, 0.9f, 1f);
    public static readonly Vector3 SuspiciousLookingTentacle = new Vector3(0.5f, 0.9f, 1f);
    public static readonly Vector3 PumpkinScentedCandle = new Vector3(1f, 0.7f, 0.05f);
    public static readonly Vector3 ToyGolem = new Vector3(1f, 0.61f, 0.16f);
    public static readonly Vector3 FairyPrincess = new Vector3(1f, 0.6f, 1f);

    public static Vector3 OmegaStarite { get; set; } = new Vector3(0.1f, 0.6f, 1f);

    public const float ShadowOrbBrightness = 0.9f;
    public const float FairyBrightness = 0.9f;
    public const float FlickerwickBrightness = 1f;
    public const float WispInABottleBrightness = 1.5f;
    public const float SuspiciousLookingTentacleBrightness = 2f;
    public const float PumpkinScentedCandleBrightness = 1.5f;
    public const float ToyGolemBrightness = 1.5f;
    public const float FairyPrincessBrightness = 1.5f;

    public static float OmegaStariteBrightness { get; set; } = 1f;
}
