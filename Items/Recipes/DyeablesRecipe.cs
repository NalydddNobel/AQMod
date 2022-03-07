using AQMod.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Recipes
{
    public class DyeablesRecipe : ModRecipe
    {
        public readonly byte SetColor;

        public DyeablesRecipe(Mod mod, byte clr) : base(mod)
        {
            SetColor = clr;
        }

        public override void OnCraft(Item item)
        {
            ((DyeableAccessory)item.modItem).color = SetColor;
        }
    }
}