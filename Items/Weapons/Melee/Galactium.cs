using AQMod.Common.Graphics;
using AQMod.Items.Materials.Energies;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Galactium : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 42;
            item.melee = true;
            item.knockBack = 6f;
            item.width = 32;
            item.height = 32;
            item.useTime = 38;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.Rarities.OmegaStariteRare;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.Galactium>();
            item.shootSpeed = 25f;
            item.value = AQItem.Prices.OmegaStariteWeaponValue;
            item.autoReuse = true;
            item.scale = 1.1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(Main.itemTexture[item.type], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 3f, 0f, 0.3f), 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            AQGraphics.Rendering.FallenStarAura(item, spriteBatch, scale, new Color(10, 10, 40, 50), new Color(40, 40, 120, 127));
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

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 15);
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(1.1f, 1.3f);
                Main.dust[d].fadeIn = 0.2f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            int dustAmount = damage / 10;
            if (dustAmount < 1)
            {
                dustAmount = 1;
            }
            if (crit)
            {
                dustAmount *= 2;
            }
            if (target.life > 0 && !target.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] && Main.rand.NextBool(2))
            {
                dustAmount *= 2;
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.Play(SoundType.NPCHit, "inflict_bluefire", target.Center, 0.8f);
                }
            }
            for (int i = 0; i < dustAmount; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.MonoSparkleDust>(),
                    Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(150, 170, 200, 100)).noGravity = true;
            }
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            damage = (int)(damage * 1.5);
            position = new Vector2(player.position.X, player.position.Y - 800f);
            var newVelocity = Vector2.Normalize(Main.MouseWorld - position) * (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            speedX = newVelocity.X;
            speedY = newVelocity.Y;
            Projectile.NewProjectile(position, Vector2.Normalize(Main.MouseWorld + new Vector2((float)Math.Sin(position.X + position.Y + Main.MouseWorld.X + Main.MouseWorld.Y) * 100f, 0f) - position) * (float)Math.Sqrt(speedX * speedX + speedY * speedY), type, damage, knockBack, player.whoAmI);
            position += new Vector2((float)Math.Sin(position.X - position.Y + Main.MouseWorld.X - Main.MouseWorld.Y) * 10f, 0f);
            Main.PlaySound(SoundID.Item9, position);
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<StariteBlade>());
            r.AddIngredient(ItemID.Starfury);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 10);
            r.AddIngredient(ItemID.SoulofFlight, 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}