using AQMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Recipes
{
    internal class StarbyteRecipe : ModRecipe
    {
        private readonly MoliteTag.StarbyteTagData potionTag;

        public StarbyteRecipe(Mod mod, ushort potionType) : base(mod)
        {
            potionTag = new MoliteTag.StarbyteTagData(potionType);
        }

        public override void OnCraft(Item item)
        {
            ((MoliteTag)item.modItem).potion = potionTag;
        }

        internal static void ConstructRecipe(int potionItem, ModItem item)
        {
            var recipe = new StarbyteRecipe(item.mod, (ushort)potionItem);
            recipe.AddIngredient(potionItem);
            recipe.AddIngredient(ModContent.ItemType<Molite>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(item);
            var molite = (MoliteTag)recipe.createItem.modItem;
            molite.potion = recipe.potionTag;
            molite.SetupPotionStats();
            recipe.AddRecipe();
        }
    }
}