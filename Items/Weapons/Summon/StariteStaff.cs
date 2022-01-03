using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon
{
    public class StariteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = AQItem.Prices.GlimmerWeaponValue;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<Projectiles.Summon.Starite>();
            item.buffType = ModContent.BuffType<Buffs.Summon.Starite>();
            item.autoReuse = true;
            item.shootSpeed = 8f;
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


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }
    }
}