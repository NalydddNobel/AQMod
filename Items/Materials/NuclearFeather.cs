using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class NuclearFeather : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(gold: 1);
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}