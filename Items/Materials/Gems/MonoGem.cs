using Aequus.Common.Rendering;
using Aequus.Common.Rendering.Tiles;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Gems
{
    public class MonoGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonoGemTile>());
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override void AddRecipes()
        {
        }
    }

    public class MonoGemTile : BaseGemTile, ISpecialTileRenderer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;

            AddMapEntry(new Color(66, 55, 55), Lang.GetItemName(ModContent.ItemType<MonoGem>()));
            DustType = DustID.Ambient_DarkBrown;
            ItemDrop = ModContent.ItemType<MonoGem>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.2f;
            b = 0.2f;
        }

        public void GetRandomValues(int i, int j, out ulong seed, out float globalIntensity)
        {
            seed = Helper.TileSeed(i, j);
            globalIntensity = Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f + Utils.RandomFloat(ref seed) * 20f, 0.7f, 1f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GetRandomValues(i, j, out ulong seed, out float globalIntensity);
            var drawPos = this.GetDrawPosition(i, j, GetObjectData(i, j)) + Helper.TileDrawOffset;

            Main.spriteBatch.Draw(
                AequusTextures.Bloom0,
                drawPos + new Vector2(8f),
                null,
                Color.Black * globalIntensity * 0.75f,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                0.45f * globalIntensity + 0.33f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPos,
                new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16),
                Color.White);

            SpecialTileRenderer.Add(i, j, TileRenderLayer.PostDrawMasterRelics);
            return false;
        }

        void ISpecialTileRenderer.Render(int i, int j, TileRenderLayer layer)
        {
            GetRandomValues(i, j, out ulong seed, out float globalIntensity);

            var fogTexture = AequusTextures.FogParticleHQ;
            var drawPos = this.GetDrawPosition(i, j, GetObjectData(i, j)) + Main.screenLastPosition + new Vector2(8f);

            MonoGemRenderer.Instance.DrawData.Add(
                new DrawData(
                    AequusTextures.Bloom3,
                    drawPos, 
                    null,
                    Color.White * 0.1f * globalIntensity,
                    0f,
                    AequusTextures.Bloom3.Size() / 2f,
                    1f * globalIntensity, SpriteEffects.FlipHorizontally, 0
                ));

            for (int k = 0; k < 3; k++)
            {
                float intensity = MathF.Sin((k * MathHelper.Pi / 3f + Main.GameUpdateCount / 60f) % MathHelper.Pi);
                var frame = fogTexture.Frame(verticalFrames: 8, frameY: Utils.RandomInt(ref seed, 8));
                MonoGemRenderer.Instance.DrawData.Add(
                    new DrawData(
                        fogTexture,
                        drawPos,
                        frame,
                        Color.White * intensity * 0.75f * globalIntensity,
                        Main.GlobalTimeWrappedHourly * 0.1f,
                        frame.Size() / 2f,
                        3f * globalIntensity, SpriteEffects.FlipHorizontally, 0));
            }
        }
    }

    public class MonoGemRenderer : ScreenTarget
    {
        private class MonoGemScreenShaderData : ScreenShaderData
        {
            public MonoGemScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
            {
            }

            public override void Apply()
            {
                Main.instance.GraphicsDevice.Textures[1] = Instance.GetTarget();
                base.Apply();
            }
        }

        public static MonoGemRenderer Instance { get; private set; }
        public readonly ParticleRenderer Particles = new();
        public readonly List<DrawData> DrawData = new();

        public const string ScreenShaderKey = "Aequus:MonoGem";

        public override void Load(Mod mod)
        {
            base.Load(mod);
            Instance = this;
            Filters.Scene[ScreenShaderKey] = new Filter(new MonoGemScreenShaderData(
                new Ref<Effect>(
                    ModContent.Request<Effect>($"{this.NamespacePath()}/MonoGemScreenShader",
                    AssetRequestMode.ImmediateLoad).Value),
                "GrayscaleMaskPass"), EffectPriority.Low);
        }

        public override void Unload()
        {
            Instance = null;
            base.Unload();
        }

        protected override bool PrepareTarget()
        {
            return Particles.Particles.Count > 0 || DrawData.Count > 0;
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            Main.spriteBatch.Begin_World(shader: false);

            Particles.Draw(spriteBatch);
            foreach (var d in DrawData)
                (d with { position = d.position - Main.screenPosition }).Draw(spriteBatch);

            DrawData.Clear();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            Helper.ShaderColorOnly.Apply(null);

            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            spriteBatch.End();

            _wasPrepared = true;
        }

        public static void HandleScreenRender()
        {
            if (!Lighting.NotRetro)
            {
                if (Instance.IsReady)
                    Instance.DrawOntoScreen(Main.spriteBatch);
            }
            else if (Instance.IsReady)
            {
                Filters.Scene.Activate(ScreenShaderKey, Main.LocalPlayer.Center);
                Filters.Scene[ScreenShaderKey].GetShader().UseOpacity(1f);
            }
            else
            {
                Filters.Scene.Deactivate(ScreenShaderKey, Main.LocalPlayer.Center);
                Filters.Scene[ScreenShaderKey].GetShader().UseOpacity(0f);
            }
        }

        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

            var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 128));

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            _wasPrepared = false;
        }
    }
}