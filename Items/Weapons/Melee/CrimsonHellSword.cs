using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class CrimsonHellSword : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 38;
            item.useTime = 18;
            item.useAnimation = 18;
            item.autoReuse = true;
            item.useTurn = true;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
        }
    }
}