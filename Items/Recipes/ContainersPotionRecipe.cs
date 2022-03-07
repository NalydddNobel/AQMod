using AQMod.Items.Potions;
using AQMod.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Recipes
{
    public sealed class ContainersPotionRecipe : ModRecipe
    {
        private readonly PotionofContainersTag.ContainerTag chestTag;

        public ContainersPotionRecipe(Mod mod, int chest) : base(mod)
        {
            chestTag = new PotionofContainersTag.ContainerTag(chest);
        }

        public override void OnCraft(Item item)
        {
            ((PotionofContainersTag)item.modItem).chestTag = chestTag;
        }

        internal static void ConstructRecipe(int chestItem, ModItem item)
        {
            var r = new ContainersPotionRecipe(item.mod, (ushort)chestItem);
            r.AddIngredient(ModContent.ItemType<PotionofContainers>());
            r.AddIngredient(chestItem);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(item);
            ((PotionofContainersTag)r.createItem.modItem).chestTag = r.chestTag;
            r.AddRecipe();
        }
    }
}
