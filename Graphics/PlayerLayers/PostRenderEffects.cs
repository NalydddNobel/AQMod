using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Graphics.PlayerLayers
{
    public class PostRenderEffects : ScreenTarget, ILoadable
    {
        public static PostRenderEffects Instance { get; private set; }

        public struct Data
        {
            public PlayerDrawSet drawInfo;
            public List<RotationalAura> backRotationalAuras;
            public struct RotationalAura
            {
                public Color color;
                public Vector2 multiplier;
                public float rotation;
                public int amt;
            }

            public Data(PlayerDrawSet info)
            {
                drawInfo = info;
                drawInfo.DrawDataCache = new List<DrawData>(info.DrawDataCache);
                backRotationalAuras = new List<RotationalAura>();
            }
        }

        public static Dictionary<int, Data> PlayerRenderInfo { get; internal set; }
        public static Color FullBodyColorOnly;
        public static Color FullBodyColor;

        public RenderTarget2D _backTarget;
        public RenderTarget2D _fullColorHelperTarget;

        void ILoadable.Load(Mod mod)
        {
            Instance = this;
            PlayerRenderInfo = new Dictionary<int, Data>();
        }

        void ILoadable.Unload()
        {
            Instance = null;
            Clear();
        }

        public static void Prepare(int p, ref PlayerDrawSet info)
        {
            Instance.Request();
            if (PlayerRenderInfo.ContainsKey(p))
                return;

            PlayerRenderInfo.Add(p, new Data(info));
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (PlayerRenderInfo == null || PlayerRenderInfo.Count == 0)
            {
                return;
            }

            PrepareARenderTarget_AndListenToEvents(ref _backTarget, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.PreserveContents);
            PrepareARenderTarget_WithoutListeningToEvents(ref _fullColorHelperTarget, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);

            bool cleared = false;
            foreach (var d in PlayerRenderInfo.Values)
            {
                var drawInfo = d.drawInfo;
                Begin.GeneralEntities.BeginShader(Main.spriteBatch);
                PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawInfo);
                spriteBatch.End();

                device.SetRenderTarget(_fullColorHelperTarget);
                if (!cleared)
                {
                    device.Clear(Color.Transparent);
                }
                bool flag = Main.gameMenu;
                Main.gameMenu = true;
                Begin.GeneralEntities.BeginShader(Main.spriteBatch);
                PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawInfo);
                spriteBatch.End();
                Main.gameMenu = flag;

                device.SetRenderTarget(_backTarget);
                if (!cleared)
                {
                    device.Clear(Color.Transparent);
                }
                DrawBackTarget(d, spriteBatch, helperTarget);

                device.SetRenderTarget(_target);
                if (!cleared)
                {
                    cleared = true;
                    device.Clear(Color.Transparent);
                }

                if (FullBodyColor != Color.Transparent)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(helperTarget, new Vector2(0f, 0f), FullBodyColor);
                    spriteBatch.End();
                }

                if (FullBodyColorOnly != Color.Transparent)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
                    AequusHelpers.ColorOnlyShader.Apply(null);
                    spriteBatch.Draw(helperTarget, new Vector2(0f, 0f), FullBodyColorOnly);
                    spriteBatch.End();
                }
                device.SetRenderTarget(helperTarget);
                device.Clear(Color.Transparent);
                _wasPrepared = true;
            }
            Clear();
        }
        public void DrawBackTarget(Data data, SpriteBatch spriteBatch, RenderTarget2D helperTarget)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            AequusHelpers.ColorOnlyShader.Apply(null);
            foreach (var rotational in data.backRotationalAuras)
            {
                foreach (var c in AequusHelpers.CircularVector(rotational.amt, rotational.rotation))
                {
                    spriteBatch.Draw(_fullColorHelperTarget, c * rotational.multiplier * Main.GameViewMatrix.Zoom, rotational.color);
                }
            }
            spriteBatch.End();
        }

        public void Clear()
        {
            if (PlayerRenderInfo != null)
            {
                PlayerRenderInfo?.Clear();
            }
            FullBodyColor = Color.Transparent;
            FullBodyColorOnly = Color.Transparent;
        }

        public void DrawBackOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            spriteBatch.Draw(_backTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            spriteBatch.End();
            Begin.GeneralEntities.Begin(spriteBatch);
        }

        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            spriteBatch.Draw(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            spriteBatch.End();
            Begin.GeneralEntities.Begin(spriteBatch);
            _wasPrepared = false;
        }
    }
}
