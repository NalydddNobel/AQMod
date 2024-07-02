using Aequus.Core.Entities.Items.Dedications;

namespace Aequus.Content.Dedicated.Baguette;

public class Baguette : ModItem {
    public override void Load() {
        DedicationRegistry.Register(this, new AnonymousDedication(new Color(127, 92, 32)));
    }

    public override void SetStaticDefaults() {
        this.StaticDefaultsToFood(new Color(194, 136, 36, 255), new Color(147, 103, 27, 255), new Color(100, 49, 2, 255));
    }

    public override void SetDefaults() {
        Item.DefaultToFood(20, 20, ModContent.BuffType<BaguetteBuff>(), 216000);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 10);
        Item.maxStack = 29;
    }
}