using AQMod.Assets.ItemOverlays;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class SpiralingTwilight : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow")), item.type);
        }

        public override void SetDefaults()
        {
            item.height = 20;
            item.width = 20;
            item.ranged = true;
            item.damage = 45;
            item.shoot = ProjectileID.Seed;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Dart;
            item.knockBack = 5f;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.UseSound = SoundID.Item99;
            item.rare = ItemRarityID.Red;
            item.value = AQItem.Prices.PillarWeaponValue;
            item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, 2f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.IchorDart)
            {
                var velo = new Vector2(speedX, speedY);
                Projectile.NewProjectile(position, velo, type, damage, knockBack, player.whoAmI, 0, -1000f);
                float length = velo.Length();
                var velocity = Vector2.Normalize(velo);
                int amount = Main.rand.Next(3) + 1;
                for (int i = 0; i < amount; i++)
                {
                    Projectile.NewProjectile(position, Vector2.Normalize(Vector2.Normalize(new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101))) + velocity * 2f) * length, type, damage, knockBack, player.whoAmI, 0f, -1000f);
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Sprinkler>());
            recipe.AddIngredient(ItemID.FragmentVortex, 18);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}