using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class Bubbler : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 11;
            item.magic = true;
            item.useTime = 6;
            item.useAnimation = 6;
            item.width = 28;
            item.height = 28;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.Bubbler>();
            item.shootSpeed = 5.5f;
            item.mana = 5;
            item.autoReuse = true;
            item.UseSound = SoundID.Item85;
            item.value = AQItem.CrabsonWeaponValue;
            item.noMelee = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var newVelocity = new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-0.157f, 0.157f));
            position += Vector2.Normalize(newVelocity) * (60f * item.scale);
            speedX = newVelocity.X;
            speedY = newVelocity.Y;
            return true;
        }
    }
}