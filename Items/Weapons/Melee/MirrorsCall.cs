using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Melee;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class MirrorsCall : ModItem
    {
        public static Color[] EightWayRainbow => new Color[]
            {
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.Green,
                Color.Cyan,
                Color.Blue,
                Color.Violet,
                Color.Magenta,
            };

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

            AequusTooltips.Dedicated[Type] = new AequusTooltips.ItemDedication(new Color(110, 110, 128, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<MirrorsCallProj>(6);
            Item.SetWeaponValues(150, 6f, 26);
            Item.width = 20;
            Item.height = 20;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float pitch = 1.8f;
            if (player.altFunctionUse == 2)
            {
                pitch += 1.8f;
            }
            SoundHelper.Play(SoundType.Sound, "swordswoosh" + Main.rand.Next(4), position, 0.7f, pitch);
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawRainbowAura(spriteBatch, TextureAssets.Item[Type].Value, position, frame, 0f, origin, scale, entitySpriteDraw: false);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[Type].Value;
            DrawRainbowAura(spriteBatch, texture, Item.Center + new Vector2(0f, -27f) - Main.screenPosition, null, rotation, texture.Size() / 2f, scale);
            return false;
        }

        public static void DrawRainbowAura(SpriteBatch spriteBatch, Texture2D texture, Vector2 vector, Rectangle? frame, float rotation, Vector2 origin,
            float scale, SpriteEffects effects = SpriteEffects.None, float opacity = 1f, bool entitySpriteDraw = true, bool drawWhite = true,
            float rainbowScaleMultiplier = 1f, float rainbowOffsetScaleMultiplier = 4f, float rainbowOffsetScaleMultiplier2 = 1f)
        {
            DrawRainbowAura(spriteBatch, texture, vector, frame, rotation, origin, new Vector2(scale), effects, opacity, entitySpriteDraw, drawWhite, rainbowScaleMultiplier, rainbowOffsetScaleMultiplier, rainbowOffsetScaleMultiplier2);
        }
        public static void DrawRainbowAura(SpriteBatch spriteBatch, Texture2D texture, Vector2 vector, Rectangle? frame, float rotation, Vector2 origin,
            Vector2 scale, SpriteEffects effects = SpriteEffects.None, float opacity = 1f, bool entitySpriteDraw = true, bool drawWhite = true,
            float rainbowScaleMultiplier = 1f, float rainbowOffsetScaleMultiplier = 4f, float rainbowOffsetScaleMultiplier2 = 1f)
        {
            var clrs = EightWayRainbow;
            var v = AequusHelpers.CircularVector(clrs.Length, Main.GlobalTimeWrappedHourly * 5f);

            if (drawWhite)
            {
                if (entitySpriteDraw)
                {
                    spriteBatch.Draw(texture, vector, frame, Color.White * opacity, rotation, origin, scale, effects, 0f);
                    spriteBatch.Draw(texture, vector, frame, Color.White.UseA(0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 0.25f) * opacity, rotation, origin, scale, effects, 0f);
                }
                else
                {
                    Main.EntitySpriteDraw(texture, vector, frame, Color.White * opacity, rotation, origin, scale, effects, 0);
                    Main.EntitySpriteDraw(texture, vector, frame, Color.White.UseA(0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 0.25f) * opacity, rotation, origin, scale, effects, 0);
                }
            }

            for (float rainbowOffset = Math.Min(scale.X > scale.Y ? scale.Y : scale.X, 1.01f) * rainbowOffsetScaleMultiplier; rainbowOffset > 0f; rainbowOffset -= 2f)
            {
                for (int i = 0; i < clrs.Length; i++)
                {
                    clrs[i] = clrs[i].MaxRGBA(1);
                    if (entitySpriteDraw)
                    {
                        spriteBatch.Draw(texture, vector + v[i] * rainbowOffset * rainbowOffsetScaleMultiplier2, frame, clrs[i].UseA(50) * 0.2f * opacity, rotation, origin, scale * rainbowScaleMultiplier, effects, 0f);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(texture, vector + v[i] * rainbowOffset * rainbowOffsetScaleMultiplier2, frame, clrs[i].UseA(50) * 0.2f * opacity, rotation, origin, scale * rainbowScaleMultiplier, effects, 0);
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PiercingStarlight)
                .AddIngredient(ModContent.ItemType<Slice>())
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ModContent.ItemType<UltimateEnergy>(), 5)
                .AddIngredient(ModContent.ItemType<LightMatter>(), 20)
                .AddIngredient(ItemID.WhitePearl)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}