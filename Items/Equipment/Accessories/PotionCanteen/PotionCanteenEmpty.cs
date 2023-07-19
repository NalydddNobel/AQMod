using Aequus.Common.DataSets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.PotionCanteen {
    public class PotionCanteenEmpty : ModItem {
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

            foreach (var itemId in ItemSets.IsPotion) {
                Item item = new(itemId);
                var r = Recipe.Create(ModContent.ItemType<PotionCanteen>())
                    .AddIngredient(Type)
                    .AddIngredient(item.type, 5);

                var canteen = r.createItem.ModItem<PotionCanteen>();
                canteen.itemIDLookup = item.type;
                canteen.buffID = item.buffType;
                canteen.SetPotionDefaults();
                r.Register();
            }
        }
    }
}