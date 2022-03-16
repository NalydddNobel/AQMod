using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public sealed class Crabax : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 60;
            item.height = 60;
            item.melee = true;
            item.damage = 20;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 10f;
            item.axe = 25; // has the highest axe power
            item.tileBoost = 5;
            item.value = Item.sellPrice(gold: 2);
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.expert = true;
        }
    }
}