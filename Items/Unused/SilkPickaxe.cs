using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused
{
    public class SilkPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 0;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.SetWeaponValues(12, 0.1f);
            Item.DamageType = DamageClass.Melee;
            Item.pick = 55;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Gray;
            Item.autoReuse = true;
        }
    }
}