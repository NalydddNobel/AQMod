namespace Aequus.Old.Content.Equipment.Accessories.WarHorn;

public class WarHornCooldown : ModBuff {
    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffSets.NurseCannotRemoveDebuff[Type] = true;
    }
}