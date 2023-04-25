using Aequus;
using Aequus.Common.Rendering.Tiles;
using Aequus.Items.Misc.Dyes;
using Aequus.Particles;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Gems
{
    public class OmniGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OmniGemTile>());
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

            var drawData = new DrawData(texture, position, frame, itemColor.A > 0 ? itemColor : Main.inventoryBack, 0f, origin, scale, SpriteEffects.None, 0);
            var maskTexture = AequusTextures.OmniGem_Mask.Value;
            var maskFrame = maskTexture.Frame(verticalFrames: 3, frameY: 2);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
            effect.Apply(null, drawData);

            var drawPosition = position + frame.Size() / 2f * scale - origin * scale;
            Main.spriteBatch.Draw(
                maskTexture,
                drawPosition,
                maskFrame.Frame(0, -1),
                Color.White with { A = 0, } * 0.2f,
                0f,
                maskFrame.Size() / 2f,
                scale, SpriteEffects.None, 0f);

            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Draw(
                maskTexture,
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

            var maskTexture = AequusTextures.OmniGem_Mask.Value;
            var maskFrame = maskTexture.Frame(verticalFrames: 3, frameY: 2);

            Main.spriteBatch.Draw(
                maskTexture,
                drawPosition,
                maskFrame.Frame(0, -1),
                Color.White with { A = 0, } * 0.2f,
                rotation,
                maskFrame.Size() / 2f,
                1f, SpriteEffects.None, 0f);

            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Draw(
                maskTexture,
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
            Recipe.Create(ItemID.RainbowTorch, 3)
                .AddIngredient(ItemID.Torch, 3)
                .AddIngredient(Type)
                .Register();
            Recipe.Create(ItemID.Topaz, 5)
                .AddIngredient(ItemID.Amethyst, 5)
                .AddIngredient(Type)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Sapphire, 5)
                .AddIngredient(ItemID.Topaz, 5)
                .AddIngredient(Type)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Emerald, 5)
                .AddIngredient(ItemID.Sapphire, 5)
                .AddIngredient(Type)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Ruby, 5)
                .AddIngredient(ItemID.Emerald, 5)
                .AddIngredient(Type)
                .AddTile(TileID.DemonAltar)
                .Register();
            Recipe.Create(ItemID.Diamond, 5)
                .AddIngredient(ItemID.Ruby, 5)
                .AddIngredient(Type)
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

        public static float GetGlobalTime(ulong seed)
        {
            return Main.GlobalTimeWrappedHourly * 2f + Utils.RandomFloat(ref seed) * 20f;
        }
        public static float GetGlobalTime(int i, int j)
        {
            return GetGlobalTime(Helper.TileSeed(i, j));
        }
    }

    public class OmniGemTile : BaseGemTile, IBatchedTile
    {
        public const int MaskFrameWidth = MaskFullWidth / 3;
        public const int MaskFullWidth = 150;

        public bool SolidLayerTile => false;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;

            AddMapEntry(new Color(222, 222, 222), Lang.GetItemName(ModContent.ItemType<OmniGem>()));
            DustType = DustID.RainbowRod;
            ItemDrop = ModContent.ItemType<OmniGem>();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 7;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ParticleSystem.New<OmniGemParticle>(ParticleLayer.AboveDust)
                    .Setup(
                        new Vector2(i * 16f + Main.rand.Next(16), j * 16f + Main.rand.Next(16)),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.2f, 1f) * 3.3f,
                        Color.White with { A = 0 },
                        (Helper.HueSet(Color.Red, Main.rand.NextFloat(1f)) * 0.4f).UseA(33),
                        Main.rand.NextFloat(0.3f, 0.8f),
                        0.225f,
                        0f
                    );
            }
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.1f;
            b = 0.5f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            BatchedTileRenderer.Add(i, j, Type);
            return false;
        }

        public void BatchedPreDraw(List<BatchedTileDrawInfo> tiles, int count)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: true);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());

            var maskTexture = AequusTextures.OmniGemTile_Mask.Value;
            var glowOffset = new Vector2(-1f, -1f);
            for (var i = 0; i < count; i++)
            {
                var info = tiles[i];

                var frame = new Rectangle(info.Tile.TileFrameX / 18 * MaskFullWidth, info.Tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
                var drawPosition = this.GetDrawPosition(tiles[i].Position.X, tiles[i].Position.Y, GetObjectData(info.Position.X, info.Position.Y));
                var origin = frame.Size() / 2f;
                float globalTime = Main.GlobalTimeWrappedHourly;
                Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(tiles[i].Position.X, tiles[i].Position.Y);

                effect.Apply(null, null);

                try
                {
                    Main.spriteBatch.Draw(
                        TextureAssets.Tile[Type].Value,
                        drawPosition,
                        new Rectangle(info.Tile.TileFrameX, info.Tile.TileFrameY, 16, 16),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f, SpriteEffects.None, 0f
                    );
                }
                catch
                {
                }

                Main.GlobalTimeWrappedHourly = globalTime;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: false);
        }
        public void BatchedPostDraw(List<BatchedTileDrawInfo> tiles, int count)
        {
            Main.spriteBatch.Begin_World(shader: true);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());

            var maskTexture = AequusTextures.OmniGemTile_Mask.Value;
            var glowOffset = new Vector2(7f, 7f);
            for (var i = 0; i < count; i++)
            {
                var info = tiles[i];

                ulong seed = Helper.TileSeed(tiles[i].Position);
                var frame = new Rectangle(info.Tile.TileFrameX / 18 * MaskFullWidth, info.Tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
                var drawPosition = this.GetDrawPosition(tiles[i].Position.X, tiles[i].Position.Y, GetObjectData(info.Position.X, info.Position.Y));
                var origin = frame.Size() / 2f;
                float globalTime = Main.GlobalTimeWrappedHourly;
                Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(tiles[i].Position.X, tiles[i].Position.Y);

                effect.Apply(null, null);

                try
                {
                    Main.spriteBatch.Draw(
                        maskTexture,
                        drawPosition + glowOffset,
                        frame.Frame(2, 0),
                        Color.White with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * (0.5f + Utils.RandomFloat(ref seed) * 1f) * 2.5f, 0.6f, 1f),
                        0f,
                        origin,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                catch
                {
                }

                Main.GlobalTimeWrappedHourly = globalTime;
            }

            Main.spriteBatch.End();
        }
    }

    public class OmniGemParticle : BaseBloomParticle<OmniGemParticle>
    {
        private float fadeIn;

        public override OmniGemParticle CreateInstance()
        {
            return new OmniGemParticle();
        }

        protected override void SetDefaults()
        {
            SetTexture(ParticleTextures.monoParticle);
            fadeIn = 0f;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (fadeIn == 0f)
            {
                fadeIn = Scale + 0.9f;
            }
            Velocity *= 0.9f;
            float velo = Velocity.Length();
            Rotation += velo * 0.0314f;
            if (fadeIn > Scale)
            {
                Scale += 0.05f;
            }
            else
            {
                fadeIn = -1f;
                Scale -= 0.05f - velo / 1000f;
            }
            if (Scale <= 0.1f || float.IsNaN(Scale))
            {
                RestInPool();
                return;
            }
            if (!dontEmitLight)
                Lighting.AddLight(Position, BloomColor.ToVector3() * 0.5f);
            Position += Velocity;
        }
    }
}