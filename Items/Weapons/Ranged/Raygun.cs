using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Raygun : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 32;
            Item.height = 24;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/raygun")?.WithVolume(0.2f);
            Item.value = ItemDefaults.OmegaStariteDropValue;
            Item.knockBack = 1f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, -4f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<RaygunBullet>();
        }
    }
}