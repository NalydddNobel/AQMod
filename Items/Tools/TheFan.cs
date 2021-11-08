using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class TheFan : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 0;
            item.knockBack = 8f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 32;
            item.useAnimation = 32;
            item.UseSound = SoundID.Item22;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Projectiles.FriendlyWind>();
            item.shootSpeed = 2f;
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectiles.FriendlyWind.NewWind(player, position, new Vector2(speedX, speedY), 0.1f, 60, 80);
            return false;
        }
    }
}