using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class ATM : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ItemID.Safe;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlyingPiggyBank);
        }

        public override void AI()
        {
            var center = projectile.Center;
            var plrCenter = Main.player[projectile.owner].Center;
            var length = (plrCenter - center).Length();
            if (length > 2000f + Main.player[projectile.owner].velocity.Length() * 100f)
            {
                projectile.velocity *= 0.1f;
                projectile.Center = plrCenter;
            }
            else if (length > 96f)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(plrCenter - center) * (Main.player[projectile.owner].velocity.Length() * 0.1f + 3f), 0.1f);
            }
            else if (projectile.ai[0] == 1)
            {
                projectile.velocity *= 0.95f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var pos = projectile.Center;
            var drawPos = pos - Main.screenPosition;
            var orig = texture.Size() / 2f;
            var plr = Main.LocalPlayer;
            if (projectile.getRect().Contains(Main.MouseWorld.ToPoint()) && plr.IsInTileInteractionRange((int)pos.X / 16, (int)pos.Y / 16))
            {
                var outlineTexture = SpriteUtils.Textures.Extras[ExtraID.ATMOutline];
                plr.noThrow = 2;
                plr.showItemIcon = true;
                plr.showItemIcon2 = ItemID.Safe;
                if (PlayerInput.UsingGamepad)
                {
                    plr.GamepadEnableGrappleCooldown();
                }
                spriteBatch.Draw(outlineTexture, drawPos, null, Color.Lerp(lightColor, Main.OurFavoriteColor, 0.5f), projectile.rotation, outlineTexture.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
                if (Main.mouseRight && Main.mouseRightRelease && Player.StopMoneyTroughFromWorking == 0)
                {
                    Main.mouseRightRelease = false;
                    if (Main.player[Main.myPlayer].chest == -3)
                    {
                        Main.PlaySound(SoundID.Item97);
                        plr.chest = -1;
                        Recipe.FindRecipes();
                    }
                    else
                    {
                        plr.chest = -3;
                        plr.GetModPlayer<AQPlayer>().flyingSafe = projectile.whoAmI;
                        plr.chestX = (int)(projectile.Center.X / 16f);
                        plr.chestY = (int)(projectile.Center.Y / 16f);
                        plr.talkNPC = -1;
                        Main.npcShop = 0;
                        Main.playerInventory = true;
                        Main.PlaySound(SoundID.Item97);
                        Recipe.FindRecipes();
                    }
                }
            }
            spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, lightColor, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

    }
}