using Aequus;
using Aequus.Common.ModPlayers;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class GlowCore : ModItem, ItemHooks.IUpdateItemDye
    {
        public static HashSet<int> ProjectileBlacklist { get; private set; }

        public override void Load()
        {
            ProjectileBlacklist = new HashSet<int>()
            {
                ProjectileID.StormTigerGem,
                ProjectileID.AbigailCounter,
            };
        }

        public override void Unload()
        {
            ProjectileBlacklist?.Clear();
            ProjectileBlacklist = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            try
            {
                var texture = TextureAssets.Item[Type].Value;

                var coloring = Color.White;

                float animation = Main.GlobalTimeWrappedHourly % 1.2f;
                animation -= 0.2f;
                if (animation > 0f)
                {
                    foreach (var v in AequusHelpers.CircularVector(4, Main.GlobalTimeWrappedHourly * 2.5f))
                    {
                        Main.spriteBatch.Draw(texture, position + v * scale * (8f * animation), frame, coloring.UseA(0) * 0.4f * (1f - animation), 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
                spriteBatch.Draw(texture, position, frame, coloring, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            catch
            {
                return true;
            }
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            try
            {
                var texture = TextureAssets.Item[Type].Value;
                var frame = texture.Frame();
                var origin = frame.Size() / 2f;
                var drawCoordinates = Item.position - Main.screenPosition + origin + new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
                var itemOrigin = frame.Size() / 2f;

                var coloring = Color.White;

                float animation = Main.GlobalTimeWrappedHourly % 1.2f;
                animation -= 0.2f;
                if (animation > 0f)
                {
                    foreach (var v in AequusHelpers.CircularVector(4, Main.GlobalTimeWrappedHourly * 2.5f))
                    {
                        Main.spriteBatch.Draw(texture, drawCoordinates + v * scale * (8f * animation), frame, coloring.UseA(0) * 0.4f * (1f - animation), rotation, origin, scale, SpriteEffects.None, 0f);
                    }
                }

                Main.spriteBatch.Draw(texture, drawCoordinates, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            catch
            {
                return true;
            }
            return false;
        }

        public static void AddLight(Vector2 location, Player player, AequusPlayer aequus)
        {
            var clr = new Vector3(1f);
            if (aequus.cGlowCore > 0)
            {
                clr = DyeColorSampler.GetColor(aequus.cGlowCore, player.whoAmI).ToVector3();
            }
            float largest = clr.X;
            if (largest < clr.Y)
                largest = clr.Y;
            if (largest < clr.Z)
                largest = clr.Z;

            float multiplier = 1f / largest;
            clr *= multiplier;
            Lighting.AddLight(location, clr * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.4f, 0.6f) * aequus.accGlowCore);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accGlowCore++;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cGlowCore = dyeItem.dye;
        }
    }
}