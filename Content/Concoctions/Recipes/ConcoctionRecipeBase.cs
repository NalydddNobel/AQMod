using Terraria;

namespace AQMod.Content.Concoctions.Recipes
{
    public abstract class ConcoctionRecipeBase
    {
        public abstract Item GetItem(Item potion, Item ingredient);
    }
}