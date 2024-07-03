namespace Aequu2.Content.Items.Potions.Food.TaintedSeafood;

public class FoodPoisoningGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.IsFood[entity.type];
    }

    public override bool CanUseItem(Item item, Player player) {
        return !ItemSets.IsFood[item.type] || !player.GetModPlayer<FoodPoisonedPlayer>().foodPoisoned;
    }
}
