namespace Aequu2.Old.Content.Items.Potions.Foods.SusSteak;

public class SuspiciousLookingSteak : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
        this.StaticDefaultsToFood(Color.Red, Color.DarkRed);
        ItemSets.DrinkParticleColors[Type] = new Color[] { Color.Red, };
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Steak);
        Item.buffTime = 18000;
    }
}