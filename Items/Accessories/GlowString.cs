using AQMod.Common.Graphics;
using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Items.Accessories
{
    public class GlowString : ModItem, IUpdateEquipVisuals
    {
        public override string Texture => "Terraria/Item_" + ItemID.WhiteString;
        public byte clr = 0;

        public override bool CloneNewInstances => true;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 2);
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.yoyoString = true;
            if (clr == 255)
            {
                player.stringColor = 27;
            }
            else
            {
                player.stringColor = clr;
            }
            player.GetModPlayer<AQPlayer>().glowString = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = Main.itemTexture[DyeColorToYoyoStringItemID(clr)];
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(texture, position + new Vector2(2f * scale, 0f).RotatedBy(AQGraphics.TimerBasedOnTimeOfDay * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, new Color(50, 50, 50, 0), 0f, origin, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, position, frame, item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = Main.itemTexture[DyeColorToYoyoStringItemID(clr)];
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var drawCoordinates = item.position - Main.screenPosition + origin + new Vector2(item.width / 2 - origin.X, item.height - frame.Height);
            var itemOrigin = frame.Size() / 2f;
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(2f * scale, 0f).RotatedBy(AQGraphics.TimerBasedOnTimeOfDay * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, new Color(50, 50, 50, 0), rotation, origin, scale, SpriteEffects.None, 0f);
            }

            var spotlight = ModContent.GetTexture("AQMod/Assets/Lights/Spotlight");
            Main.spriteBatch.Draw(spotlight, drawCoordinates, null, WorldGen.paintColor(clr) * 0.2f, 0f, spotlight.Size() / 2f, scale * 0.9f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, drawCoordinates, frame, item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.WhiteString);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.CosmicEnergy>());
            r.AddTile(TileID.Loom);
            r.SetResult(this);
            r.AddRecipe();
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.RedString, ItemID.RedDye, PaintID.Red, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.OrangeString, ItemID.OrangeDye, PaintID.Orange, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.YellowString, ItemID.YellowDye, PaintID.Yellow, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.LimeString, ItemID.LimeDye, PaintID.Lime, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.GreenString, ItemID.GreenDye, PaintID.Green, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.TealString, ItemID.TealDye, PaintID.Teal, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.CyanString, ItemID.CyanDye, PaintID.Cyan, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.SkyBlueString, ItemID.SkyBlueDye, PaintID.SkyBlue, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.BlueString, ItemID.BlueDye, PaintID.Blue, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.PurpleString, ItemID.PurpleDye, PaintID.Purple, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.VioletString, ItemID.VioletDye, PaintID.Violet, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.PinkString, ItemID.PinkDye, PaintID.Pink, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.BlackString, ItemID.BlackDye, PaintID.Black, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.BrownString, ItemID.BrownDye, PaintID.Brown, this);
            AQRecipes.r_GlowString.ConstructRecipe(ItemID.RainbowString, ItemID.RainbowDye, 255, this);
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["paint"] = clr,
            };
        }

        public override void Load(TagCompound tag)
        {
            clr = tag.GetByte("paint");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(clr);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            clr = reader.ReadByte();
        }

        private static int DyeColorToYoyoStringItemID(byte clr)
        {
            switch (clr)
            {
                case PaintID.Red:
                    return ItemID.RedString;
                case PaintID.Orange:
                    return ItemID.OrangeString;
                case PaintID.Yellow:
                    return ItemID.YellowString;
                case PaintID.Lime:
                    return ItemID.LimeString;
                case PaintID.Green:
                    return ItemID.GreenString;
                case PaintID.Teal:
                    return ItemID.TealString;
                case PaintID.Cyan:
                    return ItemID.CyanString;
                case PaintID.SkyBlue:
                    return ItemID.SkyBlueString;
                case PaintID.Blue:
                    return ItemID.BlueString;
                case PaintID.Purple:
                    return ItemID.PurpleString;
                case PaintID.Violet:
                    return ItemID.VioletString;
                case PaintID.Pink:
                    return ItemID.PinkString;
                case PaintID.Black:
                    return ItemID.BlackString;
                case PaintID.Brown:
                    return ItemID.BrownString;
                case 255:
                    return ItemID.RainbowString;
            }
            return ItemID.WhiteString;
        }

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            if (clr == 255)
            {
                player.stringColor = 27;
            }
            else
            {
                player.stringColor = clr;
            }
        }
    }
}