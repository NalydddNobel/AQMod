using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems
{
    public class PotionofBossrushium : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Red;
            item.value = AQItem.PotionValue;
            item.buffTime = (int)Main.nightLength;
        }

        public override bool CanUseItem(Player player) => !AQNPC.BossRush;

        public override bool UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Buffs.Bossrush>(), item.buffTime);
            AQNPC.BeginBossRush(player.whoAmI);
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = DrawUtils.LegacyTextureCache.Glows[GlowID.PotionofBussrushium];
            float colorOffset = ((float)Math.Sin(Main.GlobalTime) + 1f) * 60f;
            Main.spriteBatch.Draw(texture, position, null, new Color((int)(20 + colorOffset * 2), 10, (int)(255 - colorOffset / 2), 180), 0f, origin + new Vector2(2, 2), scale, SpriteEffects.None, 0f);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            Vector2 drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height);
            float colorOffset = ((float)Math.Sin(Main.GlobalTime) + 1f) * 60f;
            Texture2D texture = DrawUtils.LegacyTextureCache.Glows[GlowID.PotionofBussrushium];
            Main.spriteBatch.Draw(texture, drawPosition, null, new Color((int)(20 + colorOffset * 2), 10, (int)(255 - colorOffset / 2), 180), rotation, new Vector2(texture.Width, texture.Height) / 2f, scale, SpriteEffects.None, 0f);
            Vector2 origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, alphaColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.alchemy = true;
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Lens, 2);
            recipe.AddIngredient(ItemID.RottenChunk, 6);
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ItemID.SoulofLight, 8);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}