namespace Aequus.Common.Wires;

public class WiringSystem : ModSystem {
    public static System.Single MechCooldownMultiplier { get; set; }

    public override void Load() {
        On_Wiring.CheckMech += On_Wiring_CheckMech;
    }

    private static System.Boolean On_Wiring_CheckMech(On_Wiring.orig_CheckMech orig, System.Int32 i, System.Int32 j, System.Int32 time) {
        return orig(i, j, (System.Int32)(time * MechCooldownMultiplier));
    }

    public override void ClearWorld() {
        MechCooldownMultiplier = 1f;
    }

    public override void PostUpdateEverything() {
        MechCooldownMultiplier = 1f;
    }
}