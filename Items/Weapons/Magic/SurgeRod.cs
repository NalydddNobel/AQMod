using Aequus.Items.Recipes;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [GlowMask]
    public class SurgeRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<SurgeRodProj>(), 20, 30f, hasAutoReuse: false);
            Item.SetWeaponValues(65, 0f, 0);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 10;
            Item.UseSound = SoundID.Item66;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.GaleStreamsValue;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[p].timeLeft = (int)((Main.MouseWorld - position).Length() / velocity.Length());
            return false;
        }

        public override void AddRecipes()
        {
            AequusRecipes.RedSpriteRecipe(this, ItemID.NimbusRod);
        }
    }
}