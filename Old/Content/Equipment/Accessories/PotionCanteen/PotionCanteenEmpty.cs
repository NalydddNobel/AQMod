using Aequus.Content.DataSets;
using System.Linq;

namespace Aequus.Old.Content.Equipment.Accessories.PotionCanteen;

public class PotionCanteenEmpty : ModItem {
    public static int PotionRequirement { get; set; } = 15;

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 20);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.CrystalShard, 20)
            .AddIngredient(ItemID.PixieDust, 30)
            .Register();

        foreach (int itemId in ItemSets.Potions.Where(i => i.ValidEntry)) {
            Item item = ContentSamples.ItemsByType[itemId];

            if (item.buffType == BuffID.Lucky) {
                continue;
            }

            Recipe r = Recipe.Create(ModContent.ItemType<PotionCanteen>());
            r.AddIngredient(Type);
            r.AddIngredient(item.type, PotionRequirement);

            PotionCanteen canteen = r.createItem.ModItem as PotionCanteen;
            canteen.itemIDLookup = item.type;
            canteen.buffID = item.buffType;
            canteen.SetPotionDefaults();

            r.Register();
            r.DisableDecraft();
        }
    }
}