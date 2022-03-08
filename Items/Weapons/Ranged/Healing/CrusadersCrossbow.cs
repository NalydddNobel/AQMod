using AQMod.Projectiles.Ranged.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged.Healing
{
    public class CrusadersCrossbow : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 22;
            item.ranged = true;
            item.useTime = 32;
            item.useAnimation = 32;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Orange;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 3f;
            item.useAmmo = AmmoID.Arrow;
            item.UseSound = SoundID.Item5;
            item.value = AQItem.Prices.CrabCreviceValue;
            item.noMelee = true;
            item.knockBack = 0.1f;
            item.autoReuse = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, 0f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<HealingBolt>();
            return true;
        }
    }
}