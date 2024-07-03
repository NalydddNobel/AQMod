using Aequu2.Core.CodeGeneration;

namespace Aequu2.Old.Content.Necromancy;

[Gen.Aequu2Player_Field<int>("ghostSlots")]
[Gen.Aequu2Player_Field<int>("ghostSlotsOld")]
[Gen.Aequu2Player_ResetField<int>("ghostSlotsMax")]
[Gen.Aequu2Player_ResetField<StatModifier>("ghostLifespan")]
public class NecromancySystem {
    public static int DefaultLifespan => 3600;

    [Gen.Aequu2Player_ResetEffects]
    internal static void OnResetEffects(Aequu2Player Aequu2Player) {
        Aequu2Player.ghostSlotsOld = Aequu2Player.ghostSlots;
        Aequu2Player.ghostSlots = 0;
    }
}
