using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [GlowMask]
    public class GhastlyBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 58;
            Item.useTime = 38;
            Item.useAnimation = 25;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityHardmodeDungeon;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(gold: 7);
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 3f;
            Item.shootSpeed = 35f;
            Item.shoot = ModContent.ProjectileType<GhastlyBlasterProj>();
            Item.scale = 1.2f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.mana = 20;
        }

        public override bool AltFunctionUse(Player player)
        {
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SkyFracture)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 5)
                .AddIngredient(ItemID.SpookyWood, 250)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}