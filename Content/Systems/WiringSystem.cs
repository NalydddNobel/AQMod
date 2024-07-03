namespace Aequu2.Content.Systems;

public class WiringSystem : ModSystem {
    public static float MechCooldownMultiplier { get; set; }

    public override void Load() {
        On_Wiring.CheckMech += On_Wiring_CheckMech;
    }

    private static bool On_Wiring_CheckMech(On_Wiring.orig_CheckMech orig, int i, int j, int time) {
        return orig(i, j, (int)(time * MechCooldownMultiplier));
    }

    public override void ClearWorld() {
        MechCooldownMultiplier = 1f;
    }

    public override void PostUpdateEverything() {
        MechCooldownMultiplier = 1f;
    }
}