using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.Concoctions.Recipes
{
    public sealed class ConcoctionItemRecipe<T> : ConcoctionRecipeBase where T : ConcoctionItem
    {
        public override Item GetItem(Item potion, Item ingredient)
        {
            var item = AQItem.GetDefault(ModContent.ItemType<T>());
            var con = item.modItem as ConcoctionItem;
            con.original = potion.Clone();
            con.original.stack = 1;
            con.SetPotion();
            return item;
        }
    }
}