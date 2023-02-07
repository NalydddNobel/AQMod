using Aequus.Items.Misc.Materials;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class Snowgrave : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.damage = 1800;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 160;
            Item.useAnimation = 160;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.shootSpeed = 32f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = ItemDefaults.GaleStreamsValue;
            Item.mana = 200;
            Item.shoot = ModContent.ProjectileType<SnowgraveProjSpawner>();
        }

        // mana cost is 80% less effective on this item!
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (reduce < 0f)
                reduce = MathHelper.Lerp(reduce, 0f, 0.8f);
            if (mult < 0f)
                mult = MathHelper.Lerp(reduce, 1f, 0.8f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.X = Main.MouseWorld.X;
            player.LimitPointToPlayerReachableArea(ref position);
            position.Y += 600f;
            velocity *= 0.001f;
        }

        public override void AddRecipes()
        {
            FrozenTear.UpgradeItemRecipe(this, ItemID.WaterBolt);
        }
    }
}