using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class ScarletSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.magic = true;
            item.knockBack = 2.45f;
            item.width = 40;
            item.height = 40;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 6;
            item.useAnimation = 12;
            item.reuseDelay = 9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.ScarletSea>();
            item.shootSpeed = 8.45f;
            item.noMelee = true;
            item.UseSound = SoundID.Item8;
            item.mana = 5;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var staffDirection = new Vector2(speedX, speedY);
            var center = Vector2.Normalize(staffDirection) * (45f * item.scale);
            if (Main.myPlayer == player.whoAmI)
            {
                int count = 8;
                float rot = MathHelper.PiOver4 / count;
                const float piOver8 = MathHelper.PiOver4 / 2f;
                float rot2 = staffDirection.ToRotation();
                var dustType = ModContent.DustType<MonoDust>();
                var color = new Color(205, 15, 15, 0);
                float off = 5;
                float dustSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                var plrCenter = player.Center;
                for (int i = 0; i < count; i++)
                {
                    var normal = new Vector2(1f, 0f).RotatedBy(rot * i - piOver8 + rot2);
                    int d = Dust.NewDust(plrCenter + center + normal * off, 2, 2, dustType, 0f, 0f, 0, color);
                    Main.dust[d].velocity = normal * dustSpeed;
                }
            }
            if (player.ownedProjectileCounts[item.shoot] > 10)
            {
                int count = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == item.shoot && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].ai[0] >= 0f)
                    {
                        count++;
                        if (count >= 10)
                        {
                            Main.projectile[i].ai[0] = -1f;
                            ((Projectiles.Magic.ScarletSea)Main.projectile[i].modProjectile).StopFollowingMouse(Main.projectile[i].Center + Vector2.Normalize(Main.projectile[i].velocity) * 195f);
                        }
                    }
                }
            }
            var newVelocity = new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
            position += Vector2.Normalize(staffDirection) * (50f * item.scale);
            speedX = newVelocity.X;
            speedY = newVelocity.Y;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe.AddIngredient(ItemID.Ruby, 2);
            recipe.AddIngredient(ItemID.Sapphire, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}