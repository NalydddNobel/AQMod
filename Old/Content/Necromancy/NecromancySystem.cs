using Aequus.Core.CodeGeneration;

namespace Aequus.Old.Content.Necromancy;

[Gen.AequusPlayer_Field<int>("ghostSlots")]
[Gen.AequusPlayer_Field<int>("ghostSlotsOld")]
[Gen.AequusPlayer_ResetField<int>("ghostSlotsMax")]
[Gen.AequusPlayer_ResetField<StatModifier>("ghostLifespan")]
public class NecromancySystem {
    public static int DefaultLifespan => 3600;

    [Gen.AequusPlayer_ResetEffects]
    internal static void OnResetEffects(AequusPlayer aequusPlayer) {
        aequusPlayer.ghostSlotsOld = aequusPlayer.ghostSlots;
        aequusPlayer.ghostSlots = 0;
    }
}
