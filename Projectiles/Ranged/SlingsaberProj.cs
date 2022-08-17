using Aequus.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class SlingsaberProj : SlingshotProj
    {
        public override float Gravity => 0.05f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            ItemTexture = ItemID.Bird;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemWithAmmo)
            {
                ItemTexture = itemWithAmmo.AmmoItemIdUsed;
            }
        }

        public override void DrawHeldSlingshot()
        {
            Main.instance.LoadItem(ModContent.ItemType<Slingsaber>());
            var texture = TextureAssets.Item[ModContent.ItemType<Slingsaber>()];
            var glowTexture = ModContent.Request<Texture2D>(ModContent.GetInstance<Slingsaber>().Texture + "_Glow", AssetRequestMode.ImmediateLoad);
            var topMask = TextureAssets.Projectile[Type];
            var topGlowMask = ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad);

            var position = Main.GetPlayerArmPosition(Projectile);
            var drawCoords = position;
            float rotation = Projectile.spriteDirection == -1 ? Projectile.velocity.ToRotation() - MathHelper.Pi : Projectile.velocity.ToRotation();
            var origin = new Vector2(topMask.Value.Width / 2f, topMask.Value.Height);
            var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture.Value, drawCoords - Main.screenPosition, null, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glowTexture.Value, drawCoords - Main.screenPosition, null, new Color(250, 250, 250, 0),
                 rotation, origin, Projectile.scale, spriteEffects, 0);

            Main.EntitySpriteDraw(TextureAssets.Item[ItemTexture].Value, drawCoords - new Vector2(-4f * Projectile.spriteDirection, origin.Y - 10f + Math.Max(TextureAssets.Item[ItemTexture].Value.Height - 20, 0)).RotatedBy(rotation) - Main.screenPosition, null, Projectile.GetAlpha(AequusHelpers.GetColor(drawCoords)),
                Projectile.spriteDirection == -1 ? Projectile.rotation - MathHelper.Pi : Projectile.rotation, TextureAssets.Item[ItemTexture].Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            Main.EntitySpriteDraw(topMask.Value, drawCoords - Main.screenPosition, null, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(topGlowMask.Value, drawCoords - Main.screenPosition, null, new Color(250, 250, 250, 0),
                 rotation, origin, Projectile.scale, spriteEffects, 0);
        }
    }
}