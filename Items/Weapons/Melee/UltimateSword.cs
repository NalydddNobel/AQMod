using AQMod.Assets;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class UltimateSword : ModItem, IOverlayDrawWorld, IOverlayDrawPlayerUse, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => this;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => this;

        public override void SetStaticDefaults()
        {
            AQItem.CreativeMode.SingleItem(this);
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.rare = AQItem.Rarities.OmegaStariteRare + 1;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = AQItem.Prices.OmegaStariteWeaponValue * 2;
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.scale = 1.3f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.immuneTime < 5)
            {
                player.immuneTime = 5;
                player.immuneNoBlink = true;
            }
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Shimmering>(), 1200);
            if (crit)
            {
                player.AddBuff(ModContent.BuffType<Buffs.SuperCharged>(), 300);
            }
        }

        bool IOverlayDrawWorld.PreDrawWorld(Item item, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        void IOverlayDrawWorld.PostDrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var asset = ModContent.Request<Texture2D>(this.GetPath("_Glow"));
            if (asset.Value == null)
            {
                return;
            }
            var texture = asset.Value; 
            var drawColor = new Color(128, 128, 128, 0);
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + texture.Width / 2 + Item.width / 2 - texture.Width / 2, Item.position.Y - Main.screenPosition.Y + texture.Height / 2 + Item.height - texture.Height + 2f);
            var origin = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, drawPosition, null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);

            float x = (float)Math.Sin(Main.GlobalTimeWrappedHourly / 2f) * 4f;
            drawColor *= 0.25f;
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(x, 0f), null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(-x, 0f), null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        void IOverlayDrawPlayerUse.DrawUse(Player player, AQPlayer aQPlayer, Item item, PlayerDrawSet info)
        {
            var asset = ModContent.Request<Texture2D>(this.GetPath("_Glow"));
            if (asset.Value == null)
            {
                return;
            }
            var texture = asset.Value;
            var drawColor = new Color(128, 128, 128, 0);

            if (player.gravDir == -1f)
            {
                var drawData = new DrawData(texture, new Vector2((int)(info.ItemLocation.X - Main.screenPosition.X), (int)(info.ItemLocation.Y - Main.screenPosition.Y)), new Rectangle(0, 0, texture.Width, texture.Height), Item.GetAlpha(drawColor), player.itemRotation, new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f), Item.scale, info.itemEffect, 0);
                info.DrawDataCache.Add(drawData);
                return;
            }

            var swordOrigin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
            var drawPosition = new Vector2((int)(info.ItemLocation.X - Main.screenPosition.X), (int)(info.ItemLocation.Y - Main.screenPosition.Y));
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            drawColor = Item.GetAlpha(drawColor);
            info.DrawDataCache.Add(new DrawData(texture, drawPosition, drawFrame, drawColor, player.itemRotation, swordOrigin, Item.scale, info.itemEffect, 0));
            texture = TextureAssets.Item[item.type].Value;
            float x = (float)Math.Sin(Main.GlobalTimeWrappedHourly / 2f) * 4f;
            drawColor *= 0.5f;
            info.DrawDataCache.Add(new DrawData(texture, drawPosition + new Vector2(x, 0f), drawFrame, drawColor, player.itemRotation, swordOrigin, Item.scale, info.itemEffect, 0));
            info.DrawDataCache.Add(new DrawData(texture, drawPosition + new Vector2(-x, 0f), drawFrame, drawColor, player.itemRotation, swordOrigin, Item.scale, info.itemEffect, 0));
        }
    }
}