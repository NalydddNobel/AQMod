using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Projectiles
{
    public sealed class ItemDropChooser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainbowRodBullet;

        public List<Vector3> drops;

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            if (drops == null || drops.Count == 0)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var center = Projectile.Center;
            var origin = texture.Size() / 2f;
            
            if (HoveringCheck())
            {
                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    Main.EntitySpriteDraw(texture, center - Main.screenPosition + v * 2f, null, Main.OurFavoriteColor, 0f, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture, center - Main.screenPosition, null, Color.White, 0f, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        private bool HoveringCheck()
        {
            if (Main.gamePaused || Main.gameMenu)
            {
                return false;
            }

            bool flag = !Main.SmartCursorIsUsed && !PlayerInput.UsingGamepad;
            Player player = Main.LocalPlayer;
            Point point = Projectile.Center.ToTileCoordinates();
            Vector2 compareSpot = player.Center;

            if (!InRange(player))
            {
                return false;
            }

            //Matrix matrix = Matrix.Invert(Main.GameViewMatrix.ZoomMatrix);
            Vector2 val = Main.ReverseGravitySupport(Main.MouseScreen);
            //Vector2 v = Vector2.Transform(val, matrix) + Main.screenPosition;
            Vector2 v = val + Main.screenPosition;
            Rectangle hitbox = Projectile.Hitbox;
            bool flag2 = hitbox.Contains(v.ToPoint());
            if (!((flag2 || Main.SmartInteractProj == Projectile.whoAmI) & !player.lastMouseInterface))
            {
                if (!flag)
                {
                    return true;
                }
                return false;
            }
            Main.HasInteractibleObjectThatIsNotATile = true;
            if (flag2)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ItemID.Bunny;
            }
            if (PlayerInput.UsingGamepad)
            {
                player.GamepadEnableGrappleCooldown();
            }
            if (Main.mouseRight && Main.mouseRightRelease && Player.BlockInteractionWithProjectiles == 0)
            {
                Main.mouseRightRelease = false;
                player.tileInteractAttempted = true;
                player.tileInteractionHappened = true;
                player.releaseUseTile = false;
                player.chest = -1;
                for (int i = 0; i < 40; i++)
                {
                    ItemSlot.SetGlow(i, -1f, chest: true);
                }
                player.GetModPlayer<AequusPlayer>().itemDropChooser.Set(Projectile);
                player.chestX = point.X;
                player.chestY = point.Y;
                player.SetTalkNPC(-1);
                Main.SetNPCShopIndex(0);
                Main.playerInventory = true;
                SoundEngine.PlaySound(SoundID.MenuOpen);
                Recipe.FindRecipes();
                UISystem.InventoryInterface.SetState(new ItemChooseUI(drops));
            }
            if (!Main.SmartCursorIsUsed && !PlayerInput.UsingGamepad)
            {
                return false;
            }
            if (!flag)
            {
                return true;
            }
            return flag2;
        }

        private bool InRange(Player player)
        {
            Point point = Projectile.Hitbox.ClosestPointInRect(player.Center).ToTileCoordinates();
            return player.IsInTileInteractionRange(point.X, point.Y);
        }
    }
}