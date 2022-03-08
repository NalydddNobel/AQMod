using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Crabsol : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.melee = true;
            item.damage = 40;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6f;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 5);
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.scale = 1.1f;
        }
    }
}