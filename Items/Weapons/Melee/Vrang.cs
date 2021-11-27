using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Vrang : ModItem
    {        
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 20;
            item.useAnimation = 20;
            item.useTime = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.melee = true;
            item.damage = 39;
            item.knockBack = 3f;
            item.value = AQItem.AtmosphericCurrentsValue;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Pink;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.shootSpeed = 22f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.Vrang>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var normal = Vector2.Normalize(new Vector2(speedX, speedY));
            int p = Projectile.NewProjectile(position + normal.RotatedBy(MathHelper.PiOver4 * 3f) * 50f, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            var aQProj = Main.projectile[p].GetGlobalProjectile<AQProjectile>();
            Main.projectile[p].localAI[0] = 1f;
            if (player.frostArmor)
            {
                aQProj.temperature = -20;
            }
            else
            {
                aQProj.temperature = 20;
            }
            p = Projectile.NewProjectile(position + normal.RotatedBy(-MathHelper.PiOver4 * 3f) * 50f, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].localAI[0] = -1f;
            aQProj = Main.projectile[p].GetGlobalProjectile<AQProjectile>();
            aQProj.temperature = -20;
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float temp = (float)Math.Sin(Main.GlobalTime);
            if (Main.LocalPlayer.frostArmor) // found out that the frost armor flag is raised here if you just have a full set visible... weird
            {
                temp = -1f;
            }
            Texture2D texture;
            if (temp < 0f)
            {
                texture = ModContent.GetTexture(this.GetPath("_Cold"));
            }
            else if (temp > 0f)
            {
                texture = ModContent.GetTexture(this.GetPath("_Hot"));
            }
            else
            {
                return true;
            }
            Main.spriteBatch.Draw(Main.itemTexture[item.type], position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position, frame, drawColor * temp.Abs(), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}