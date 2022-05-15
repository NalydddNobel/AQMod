using Aequus.Biomes;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class HamaYumi : ModItem
    {
        public override void SetStaticDefaults()
        {
            DemonSiegeInvasion.Register(DemonSiegeInvasion.PHM(ItemID.DemonBow, Type));

            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12.5f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 6f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(200);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<HamaYumiArrow>();
        }
    }
}