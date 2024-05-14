using Aequus.Core.CodeGeneration;

namespace Aequus.Old.Content.Necromancy;

[PlayerGen.Field<int>("ghostSlots")]
[PlayerGen.Field<int>("ghostSlotsOld")]
[PlayerGen.ResetField<int>("ghostSlotsMax")]
[PlayerGen.ResetField<StatModifier>("ghostLifespan")]
public class NecromancySystem {
    public static int DefaultLifespan => 3600;

    [PlayerGen.ResetEffects]
    internal static void OnResetEffects(AequusPlayer aequusPlayer) {
        aequusPlayer.ghostSlotsOld = aequusPlayer.ghostSlots;
        aequusPlayer.ghostSlots = 0;
    }
}
