using AQMod.Items.BuffItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public class MolitePotionRecipe : ModRecipe
    {
        private readonly ushort _potion;

        public MolitePotionRecipe(Mod mod, ushort potionType) : base(mod)
        {
            _potion = potionType;
        }

        public override void OnCraft(Item item)
        {
            ((MoliteTag)item.modItem).potion = _potion;
        }

        internal static void ConstructRecipe(int potionItem, ModItem item)
        {
            var recipe = new MolitePotionRecipe(item.mod, (ushort)potionItem);
            recipe.AddIngredient(ModContent.ItemType<Molite>());
            recipe.AddIngredient(potionItem);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(item);
            var molite = (MoliteTag)recipe.createItem.modItem;
            molite.potion = (ushort)potionItem;
            molite.SetupPotionStats();
            recipe.AddRecipe();
        }
    }
}