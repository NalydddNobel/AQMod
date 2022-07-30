using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Raygun : ModItem
    {
        public static SoundStyle? CapleweySound { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                CapleweySound = new SoundStyle("Aequus/Sounds/Items/raygun") { Volume = 0.2f, };
            }
        }

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.SetWeaponValues(25, 4f, 0);
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 32;
            Item.height = 24;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.ammo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = CapleweySound;
            Item.value = ItemDefaults.OmegaStariteValue;
            Item.knockBack = 1f;
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PhoenixBlaster)
                .AddIngredient<CosmicEnergy>()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.Anvils)
                .RegisterAfter(ItemID.PhoenixBlaster);
        }
    }
}