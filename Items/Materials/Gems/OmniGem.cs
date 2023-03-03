using Aequus;
using Aequus.Common.Rendering.Tiles;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Misc.Dyes;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Gems
{
    public class OmniGem : ModItem
    {
        public static Asset<Texture2D> Mask { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Mask = ModContent.Request<Texture2D>($"{Texture}_Mask");
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void Unload()
        {
            Mask = null;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OmniGemTile>());
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

            var drawData = new DrawData(texture, position, frame, itemColor.A > 0 ? itemColor : Main.inventoryBack, 0f, origin, scale, SpriteEffects.None, 0);
            var maskFrame = Mask.Value.Frame(verticalFrames: 3, frameY: 2);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
            effect.Apply(null, drawData);

            var drawPosition = position + frame.Size() / 2f * scale - origin * scale;
            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame.Frame(0, -1),
                Color.White with { A = 0, } * 0.2f,
                0f,
                maskFrame.Size() / 2f,
                scale, SpriteEffects.None, 0f);

            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame,
                Color.White with { A = 0, } * 0.66f,
                0f,
                maskFrame.Size() / 2f,
                scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: true);

            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = new Vector2(
                Item.position.X - Main.screenPosition.X + frame.Width / 2 + Item.width / 2 - frame.Width / 2,
                Item.position.Y - Main.screenPosition.Y + frame.Height / 2 + Item.height - frame.Height
            );
            var origin = frame.Size() / 2f;
            var drawData = new DrawData(texture, drawPosition, frame, Item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
            effect.Apply(null, drawData);

            var maskFrame = Mask.Value.Frame(verticalFrames: 3, frameY: 2);

            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame.Frame(0, -1),
                Color.White with { A = 0, } * 0.2f,
                rotation,
                maskFrame.Size() / 2f,
                1f, SpriteEffects.None, 0f);

            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame,
                Color.White with { A = 0, } * 0.66f,
                rotation,
                maskFrame.Size() / 2f,
                1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: false);
            return false;
        }

        private void AddGemRecipes()
        {
            int otherGem = ModContent.ItemType<MonoGem>();
            Recipe.Create(ItemID.Amethyst, 5)
                .AddIngredient(Type)
                .AddIngredient(otherGem)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Topaz, 5)
                .AddIngredient(Type)
                .AddIngredient(otherGem, 2)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Sapphire, 5)
                .AddIngredient(Type, 2)
                .AddIngredient(otherGem, 2)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Emerald, 5)
                .AddIngredient(Type, 2)
                .AddIngredient(otherGem, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Ruby, 5)
                .AddIngredient(Type, 2)
                .AddIngredient(otherGem, 4)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Diamond, 5)
                .AddIngredient(Type, 4)
                .AddIngredient(otherGem, 2)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Amber, 5)
                .AddIngredient(Type, 3)
                .AddIngredient(otherGem, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
        private void AddEnchantedRecipes()
        {
            Recipe.Create(ItemID.EnchantedSword)
                .AddIngredient(ItemID.WoodenSword)
                .AddIngredient(Type, 8)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.WoodenSword);

            Recipe.Create(ItemID.Sundial)
                .AddIngredient(ItemID.SunplateBlock, 50)
                .AddIngredient(Type, 8)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.SunplateBlock);
        }

        public override void AddRecipes()
        {
            AddGemRecipes();
            AddEnchantedRecipes();
        }
    }

    public class OmniGemTile : ModTile, IBatchedTile
    {
        public const int MaskFrameWidth = MaskFullWidth / 3;
        public const int MaskFullWidth = 150;

        public static Asset<Texture2D> Mask { get; private set; }

        public bool SolidLayer => false;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Mask = ModContent.Request<Texture2D>($"{Texture}_Mask");
            }
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            AddMapEntry(new Color(222, 222, 222), Lang.GetItemName(ModContent.ItemType<OmniGem>()));
            DustType = DustID.RainbowTorch;
            ItemDrop = ModContent.ItemType<OmniGem>();
        }

        public override void Unload()
        {
            Mask = null;
        }

        public override bool CanPlace(int i, int j)
        {
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                return true;
            }
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = Main.DiscoR / 255f * 0.33f;
            g = Main.DiscoG / 255f * 0.33f;
            b = Main.DiscoB / 255f * 0.33f;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            BatchedTileRenderer.Add(i, j, Type);
        }

        public void BatchedDraw(List<BatchedTileDrawInfo> tiles, int count)
        {
            Main.spriteBatch.Begin_World(shader: true);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
            effect.Apply(null, null);

            var texture = Mask.Value;
            var glowOffset = new Vector2(-1f, -1f);
            for (var i = 0; i < count; i++)
            {
                var info = tiles[i];

                var frame = new Rectangle(info.Tile.TileFrameX / 18 * MaskFullWidth, info.Tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
                var drawPosition = info.Position.ToWorldCoordinates() - Main.screenPosition;
                var origin = frame.Size() / 2f;

                Main.spriteBatch.Draw(
                    TextureAssets.Tile[Type].Value, 
                    drawPosition, 
                    new Rectangle(info.Tile.TileFrameX, info.Tile.TileFrameY, 16, 16), 
                    Color.White, 
                    0f, 
                    Vector2.Zero + new Vector2(8f), 
                    1f, SpriteEffects.None, 0f
                );
                Main.spriteBatch.Draw(
                    texture,
                    drawPosition + glowOffset, 
                    frame.Frame(2, 0), 
                    Color.White with { A = 0 } * 0.75f, 
                    0f, 
                    origin, 
                    1f, 
                    SpriteEffects.None, 
                    0f
                );
            }

            Main.spriteBatch.End();
        }
    }
}