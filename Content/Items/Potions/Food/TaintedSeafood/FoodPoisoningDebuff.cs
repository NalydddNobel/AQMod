using Aequus.Core.Components.Buffs;

namespace Aequus.Content.Items.Potions.Food.TaintedSeafood;

public class FoodPoisoningDebuff : ModBuff, ICheckQuickBuff {
    bool ICheckQuickBuff.CheckQuickBuff(Player player) {
        return false;
    }

    public override void SetStaticDefaults() {
        BuffSets.IsFedState[Type] = true;
        BuffSets.LongerExpertDebuff[Type] = false;
        BuffSets.NurseCannotRemoveDebuff[Type] = true;
        Main.debuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.GetModPlayer<FoodPoisonedPlayer>().foodPoisoned = true;
    }
}
