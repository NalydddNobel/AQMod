using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fishing.Rods
{
    public class BeeRod : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodFishingPole);
            item.fishingPole = 36;
            item.shootSpeed = 16f;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<BeeBobber>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.strongBees)
            {
                var offset = new Vector2(20f, 10f);
                int p = Projectile.NewProjectile(position + offset, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                ((BeeBobber)Main.projectile[p].modProjectile).gotoPosition = Main.MouseWorld + offset;
                offset.X = -offset.X;
                p = Projectile.NewProjectile(position + offset, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                ((BeeBobber)Main.projectile[p].modProjectile).gotoPosition = Main.MouseWorld + offset;
            }
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.BeeWax, 6);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddIngredient(ItemID.Hive, 20);
            r.AddIngredient(ItemID.BottledHoney);
            r.AddTile(ModContent.TileType<Tiles.FishingCraftingStation>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }

    public class BeeBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BobberWooden);
            drawOriginOffsetY = -8;
        }

        public Vector2 gotoPosition = new Vector2(0f, 0f);
        public float visualRotation = 0f;
        public byte dropDelay = byte.MaxValue;

        public override bool PreAI()
        {
            if (gotoPosition.X <= -1f && gotoPosition.Y <= -1f)
            {
                if (dropDelay != byte.MaxValue)
                {
                    projectile.timeLeft = 10;
                    dropDelay--;
                    if (dropDelay <= 0f)
                        dropDelay = byte.MaxValue;
                    return false;
                }
                if (projectile.honeyWet && projectile.ai[1] < 0f)
                {
                    projectile.ai[1] += 5;
                    if (projectile.ai[1] >= 0f)
                    {
                        projectile.ai[1] = 0f;
                        projectile.localAI[1] = 0f;
                        projectile.netUpdate = true;
                    }
                }
                return true;
            }
            projectile.timeLeft = 10;
            if (Main.myPlayer == projectile.owner && gotoPosition == new Vector2(0f, 0f))
                gotoPosition = Main.MouseWorld + new Vector2(0f, -20f);
            if ((projectile.Center - gotoPosition).Length() < 10f)
            {
                gotoPosition = new Vector2(-1f, -1f);
                projectile.velocity *= 0.1f;
                visualRotation = projectile.rotation;
                dropDelay = 20;
            }
            else
            {
                projectile.velocity = Vector2.Normalize(gotoPosition - projectile.Center) * Main.player[projectile.owner].HeldItem.shootSpeed;
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            return false;
        }

        public override void PostAI()
        {
            if (gotoPosition.X == -1f && gotoPosition.Y == -1f)
            {
                visualRotation = visualRotation.AngleLerp(0f, 0.1f);
                projectile.rotation = visualRotation;
            }
            if (projectile.wet)
                gotoPosition = new Vector2(-2f, -2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            gotoPosition = new Vector2(-2f, -2f);
            return true;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            var player = Main.player[projectile.owner];
            if (!projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            DrawMethods.DrawFishingLine(projectile.whoAmI % 2 == 0 ? new Color(255, 255, 0, 255) : new Color(20, 0, 20, 255), player, projectile.position, projectile.width, projectile.height, projectile.velocity, projectile.localAI[0], new Vector2(36f, 28f));
            return false;
        }
    }
}