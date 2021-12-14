using AQMod.Assets;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class UltimateSword : ModItem, IOverlayDrawWorld, IOverlayDrawPlayerUse, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => this;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => this;

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.rare = AQItem.Rarities.OmegaStariteRare + 1;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.OmegaStariteWeaponValue * 2;
            item.damage = 65;
            item.melee = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.useTurn = true;
            item.scale = 1.3f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.immuneTime < 5)
            {
                player.immuneTime = 5;
                player.immuneNoBlink = true;
            }
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 1200);
            if (crit)
            {
                player.AddBuff(ModContent.BuffType<Buffs.Ultima>(), 300);
            }
        }

        bool IOverlayDrawWorld.PreDrawWorld(Item item, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        void IOverlayDrawWorld.PostDrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var drawColor = new Color(128, 128, 128, 0);
            var texture = AQUtils.GetTextureobj<UltimateSword>("_Glow");
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            var origin = Main.itemTexture[item.type].Size() / 2;
            Main.spriteBatch.Draw(texture, drawPosition, null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);

            float x = (float)Math.Sin(Main.GlobalTime / 2f) * 4f;
            drawColor *= 0.25f;
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(x, 0f), null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(-x, 0f), null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        void IOverlayDrawPlayerUse.DrawUse(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            var texture = AQUtils.GetTextureobj<UltimateSword>("_Glow");
            var drawColor = new Color(128, 128, 128, 0);

            if (player.gravDir == -1f)
            {
                var drawData = new DrawData(texture, new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y)), new Rectangle(0, 0, texture.Width, texture.Height), item.GetAlpha(drawColor), player.itemRotation, new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f), item.scale, info.spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
                return;
            }

            var swordOrigin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
            var drawPosition = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            drawColor = item.GetAlpha(drawColor);
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, drawColor, player.itemRotation, swordOrigin, item.scale, info.spriteEffects, 0));
            texture = TextureGrabber.GetItem(item.type);
            float x = (float)Math.Sin(Main.GlobalTime / 2f) * 4f;
            drawColor *= 0.5f;
            Main.playerDrawData.Add(new DrawData(texture, drawPosition + new Vector2(x, 0f), drawFrame, drawColor, player.itemRotation, swordOrigin, item.scale, info.spriteEffects, 0));
            Main.playerDrawData.Add(new DrawData(texture, drawPosition + new Vector2(-x, 0f), drawFrame, drawColor, player.itemRotation, swordOrigin, item.scale, info.spriteEffects, 0));
        }
    }
}