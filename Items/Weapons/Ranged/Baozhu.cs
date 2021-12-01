using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class Baozhu : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 43;
            item.ranged = true;
            item.useTime = 18;
            item.useAnimation = 18;
            item.width = 16;
            item.height = 16;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = AQItem.Rarities.GaleCurrentsRare;
            item.shoot = ModContent.ProjectileType<Projectiles.Ranged.Firecracker>();
            item.shootSpeed = 13.5f;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.noMelee = true;
            item.knockBack = 7f;
            item.noUseGraphic = true;
            item.autoReuse = true;
        }
    }
}