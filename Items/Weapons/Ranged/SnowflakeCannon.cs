using Aequus.Items.Materials;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged {
    public class SnowflakeCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.SetWeaponValues(50, 3f);
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<SnowflakeCannonProj>();
            Item.shootSpeed = 4.5f;
            Item.value = ItemDefaults.ValueGaleStreams;
            Item.useAmmo = AmmoID.Snowball;
            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
        }

        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return Main.rand.NextFloat() < 0.66f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SnowflakeCannonProj>();
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f));
        }

        public override void AddRecipes()
        {
            FrozenTear.UpgradeItemRecipe(this, ItemID.SnowballCannon);
        }
    }
}