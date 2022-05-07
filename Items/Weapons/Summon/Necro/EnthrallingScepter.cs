using Aequus.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class EnthrallingScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;

            Tooltip.SetDefault("Testing Item");
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(30);
            Item.shoot = ModContent.ProjectileType<EnthrallingBolt>();
            Item.shootSpeed = 20f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}