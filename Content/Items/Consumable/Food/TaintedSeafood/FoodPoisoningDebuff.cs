using Aequus.Common.Entities.Buffs;

namespace Aequus.Content.Items.Consumable.Food.TaintedSeafood;

public class FoodPoisoningDebuff : ModBuff, ICheckQuickBuff {
    bool ICheckQuickBuff.CheckQuickBuff(Player player) {
        return false;
    }

    public override void SetStaticDefaults() {
        BuffID.Sets.IsFedState[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        Main.debuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.GetModPlayer<FoodPoisonedPlayer>().foodPoisoned = true;
    }
}
