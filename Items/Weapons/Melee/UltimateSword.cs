using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    [GlowMask]
    public class UltimateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.rare = ItemDefaults.RarityOmegaStarite + 1;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.OmegaStariteValue * 2;
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.scale = 1.5f;
        }
    }
}