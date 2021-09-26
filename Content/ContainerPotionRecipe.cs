using AQMod.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public class ContainerPotionRecipe : ModRecipe
    {
        private readonly ushort _chest;

        public ContainerPotionRecipe(Mod mod, ushort chest) : base(mod)
        {
            _chest = chest;
        }

        public override void OnCraft(Item item)
        {
            ((PotionofContainersTag)item.modItem).chestBait = _chest;
        }

        internal static void ConstructRecipe(int chestItem, ModItem item)
        {
            var recipe = new ContainerPotionRecipe(item.mod, (ushort)chestItem);
            recipe.AddIngredient(ModContent.ItemType<PotionofContainers>());
            recipe.AddIngredient(chestItem);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(item);
            ((PotionofContainersTag)recipe.createItem.modItem).chestBait = (ushort)chestItem; // so that it shows in the crafting menu
            recipe.AddRecipe();
        }
    }
}