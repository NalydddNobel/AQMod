using AQMod.Dusts;
using AQMod.Items.Fish.Corruption;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class SeltzerRain : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 23;
            item.magic = true;
            item.knockBack = 1.25f;
            item.width = 40;
            item.height = 40;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 40;
            item.useAnimation = 40;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.SeltzerRain>();
            item.shootSpeed = 16.88f;
            item.noMelee = true;
            item.UseSound = SoundID.Item8;
            item.mana = 11;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int count = 5;
            float mult = 1f / (count + 1);
            var staffDirection = new Vector2(speedX, speedY);
            position += Vector2.Normalize(staffDirection) * (50f * item.scale);
            if (Main.myPlayer == player.whoAmI)
            {
                int dustCount = 10;
                float rot = MathHelper.PiOver2 / dustCount;
                float rot2 = staffDirection.ToRotation();
                var dustType = ModContent.DustType<MonoDust>();
                float off = 5;
                var plrCenter = player.Center;
                float speed = staffDirection.Length();
                for (int j = 0; j < count; j++)
                {
                    float mult2 = mult * (j + 1f);
                    var color = new Color(50, 15, 190, 0) * (mult2 + 0.1f);
                    float dustSpeed = speed * mult2;
                    for (int i = 0; i < dustCount; i++)
                    {
                        var normal = new Vector2(1f, 0f).RotatedBy(rot * i - MathHelper.PiOver4 + rot2);
                        int d = Dust.NewDust(position + normal * off, 2, 2, dustType, 0f, 0f, 0, color);
                        Main.dust[d].velocity = normal * dustSpeed;
                    }
                }
            }
            for (int i = 0; i < count - 1; i++)
            {
                Projectile.NewProjectile(position, staffDirection * (mult * (i + 1f)), type, damage, knockBack, player.whoAmI);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddIngredient(ModContent.ItemType<Fizzler>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}