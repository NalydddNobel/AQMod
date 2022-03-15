using AQMod.Assets;
using AQMod.Common;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class UltimateSword : ModItem, GlowmaskData.IPlayerHeld
    {
        public override void SetStaticDefaults()
        {
            this.CustomGlowmask(new AQUtils.ItemGlowmask(), null, this);
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.rare = AQItem.RarityOmegaStarite + 1;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.OmegaStariteWeaponValue * 2;
            item.damage = 75;
            item.melee = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.useTurn = true;
            item.scale = 1.5f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.immuneTime < 5)
            {
                player.immuneTime = 5;
                player.immuneNoBlink = true;
            }
            var center = target.Center;
            for (int i = 0; i < 26; i++)
            {
                int d = Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<UltimateEnergyDust>());
                Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
            }
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 1200);
            if (crit)
            {
                player.AddBuff(ModContent.BuffType<Buffs.Ultima>(), 300);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<UltimateEnergyDust>());
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(1.1f, 1.3f);
                Main.dust[d].fadeIn = 0.2f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (WorldDefeats.ObtainedUltimateSword)
            {
                return;
            }
            try
            {
                int index = AQItem.LegacyGetLineIndex(tooltips, "Tooltip#");
                tooltips.Insert(index + 1, new TooltipLine(mod, "ObtainedFrom", Language.GetTextValue("Mods.AQMod.ItemTooltipExtra.UltimateSword.0")) { overrideColor = AQMod.MysteriousGuideTooltip, });
            }
            catch
            {
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
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

        void GlowmaskData.IPlayerHeld.Draw(GlowmaskData glowmask, Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            var texture = glowmask.Tex;
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