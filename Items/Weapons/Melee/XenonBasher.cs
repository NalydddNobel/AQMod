using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common.ItemOverlays;
using AQMod.Content.Dusts;
using AQMod.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class XenonBasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyDynamicGlowmask(GlowID.XenonBasher, GetGlowmaskColor), item.type);
        }

        private static Color GetGlowmaskColor()
        {
            return new Color(128, 128, 128, 0) * (((float)Math.Sin(Main.GlobalTime * 10f) + 1f) / 2f);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 32;
            item.useTime = 32;
            item.useAnimation = 32;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 50);
            item.melee = true;
            item.knockBack = 4f;
            item.scale = 1.35f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            knockBack *= 0.5f;
            var spawnPosition = target.Center;
            int type = ModContent.ProjectileType<Projectiles.XenonSpore>();
            float speed = 6f * player.meleeSpeed;
            int amount = Main.rand.Next(3) + 1;
            if (crit)
            {
                amount = (int)(amount * 2.5f);
            }
            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(spawnPosition, new Vector2(speed, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), type, damage, knockBack, player.whoAmI);
            }
            spawnPosition += new Vector2(-6f, -6f);
            type = ModContent.DustType<XenonDust>();
            for (int i = 0; i < amount * 10; i++)
            {
                Main.dust[Dust.NewDust(spawnPosition, 8, 8, type)].velocity = new Vector2(10f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
            type = ModContent.DustType<XenonMist>();
            for (int i = 0; i < amount; i++)
            {
                Main.dust[Dust.NewDust(spawnPosition, 8, 8, type)].velocity = new Vector2(10f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
        }

        public static Color DustColor(float time = 0f)
        {
            return new Color(175 + (int)(Math.Sin(time) * 20), 175 + (int)(Math.Cos(time) * 20), 225 + (int)((Math.Cos(time * 0.2f) + 0.75d) * 15), 40);
        }

        public static void SpawnDust(Vector2 position)
        {
            int size = 12;
            var spawnPosition = position + new Vector2(-size / 2f);
            Main.dust[Dust.NewDust(spawnPosition, size, size, ModContent.DustType<MonoDust>(), 0f, 0f, 0, DustColor(Main.GlobalTime), 1.25f)].velocity = new Vector2(0f, -6f);
            if (Main.rand.NextBool(4))
            {
                Main.dust[Dust.NewDust(spawnPosition, size, size, ModContent.DustType<MonoDust>(), 0f, 0f, 0, DustColor(Main.GlobalTime), 1.25f)].velocity = new Vector2(0f, -6f);
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(spawnPosition, size, size, ModContent.DustType<XenonDust>());
            }
            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(spawnPosition, size, size, ModContent.DustType<XenonMist>());
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var texture = TextureCache.GetItem(item.type);
                float length = texture.Size().Length();
                Vector2 itemPosition = new Vector2((int)player.itemLocation.X, (int)player.itemLocation.Y);
                Vector2 center = player.MountedCenter;
                var origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
                var directionToTopRight = Vector2.Normalize(itemPosition - origin - (itemPosition + origin));
                Vector2 normal = itemPosition + directionToTopRight.RotatedBy(player.itemRotation) * (length / 2f) - center;
                var swordTipOffset = Vector2.Normalize(normal) * (normal.Length() + length / 2f - 8f) * item.scale;
                if (player.direction == 1)
                {
                    swordTipOffset = swordTipOffset.RotatedBy(0.575f);
                }
                SpawnDust(center + swordTipOffset);
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (AQMod.GameWorldActive)
            {
                var center = item.Center;
                var texture = TextureCache.GetItem(item.type);
                var size = texture.Size();
                var length = size.Length();
                var origin = texture.Size() / 2f;
                var itemPosition = new Vector2(item.position.X + Main.itemTexture[item.type].Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y + texture.Height / 2 + item.height - texture.Height + 2f);
                var directionToTopRight = Vector2.Normalize(itemPosition - origin - (itemPosition + origin));
                Vector2 normal = Vector2.Normalize(new Vector2(-directionToTopRight.X, directionToTopRight.Y)).RotatedBy(rotation);
                SpawnDust(center + normal * (length / 2f * scale - 8f) + new Vector2(0f, -8f));
            }
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<VineSword>());
            r.AddIngredient(ModContent.ItemType<XenonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}