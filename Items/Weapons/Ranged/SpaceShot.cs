using AQMod.Common.Graphics;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class SpaceShot : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 36;
            item.useAnimation = 36;
            item.autoReuse = true;
            item.damage = 13;
            item.rare = AQItem.Rarities.StariteWeaponRare;
            item.ranged = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 15f;
            item.value = AQItem.Prices.GlimmerWeaponValue;
            item.useAmmo = AmmoID.Bullet;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item11;
            item.noMelee = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Ranged.SpaceShot>(), damage, knockBack, player.whoAmI, type);
            return false;
        }


        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(Main.itemTexture[item.type], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 3f, 0f, 0.3f), 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            AQGraphics.Rendering.DrawFallenStarAura(item, spriteBatch, scale, new Color(20, 20, 80, 50), new Color(50, 50, 180, 127));
            Rectangle frame;
            if (Main.itemAnimations[item.type] != null)
            {
                frame = Main.itemAnimations[item.type].GetFrame(Main.itemTexture[item.type]);
            }
            else
            {
                frame = Main.itemTexture[item.type].Frame();
            }
            var origin = frame.Size() / 2f;
            var drawCoordinates = item.position - Main.screenPosition + origin + new Vector2(item.width / 2 - origin.X, item.height - frame.Height);
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawCoordinates, frame, item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawCoordinates, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0f, 0.5f), rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.FlintlockPistol);
            r.AddIngredient(ItemID.FallenStar, 8);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}