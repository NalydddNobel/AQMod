namespace Aequus.Content.Potions.Food.TaintedSeafood;

public class FoodPoisoningGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemID.Sets.IsFood[entity.type];
    }

    public override bool CanUseItem(Item item, Player player) {
        return !ItemID.Sets.IsFood[item.type] || !player.GetModPlayer<FoodPoisonedPlayer>().foodPoisoned;
    }
}
