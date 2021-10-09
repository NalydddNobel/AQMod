using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class ATM : ModItem, IUpdatePlayerSafe
    {
        public override string Texture => "Terraria/Item_" + ItemID.Safe;

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.MoneyTrough);
            item.UseSound = SoundID.Item97;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<ATMPet>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            return player.altFunctionUse != 2;
        }

        void IUpdatePlayerSafe.UpdatePlayerSafe(Player player, int i)
        {
            var aQPlr = player.GetModPlayer<AQPlayer>();
            var type = ModContent.ProjectileType<ATMPet>();
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(player.Center, new Vector2(0f, 0f), type, 0, 0f, player.whoAmI);
            for (int j = 0; j < Main.maxProjectiles; j++)
            {
                if (Main.projectile[j].active && Main.projectile[j].type == type)
                    Main.projectile[j].timeLeft = 30;
            }
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Safe);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.PiggyBank);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(ItemID.MoneyTrough);
            r.AddRecipe();
        }
    }

    public class ATMPet : ModProjectile
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
                var outlineTexture = TextureCache.ATMPetHighlight.GetValue();
                plr.noThrow = 2;
                plr.showItemIcon = true;
                plr.showItemIcon2 = ItemID.Safe;
                if (PlayerInput.UsingGamepad)
                    plr.GamepadEnableGrappleCooldown();
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