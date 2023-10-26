using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Wires;

public class WiringSystem : ModSystem {
    public static float MechCooldownMultiplier;

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