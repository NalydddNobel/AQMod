using AQMod.Common.Graphics.DrawTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace AQMod.Common.Graphics.SceneLayers
{
    public sealed class WindLayer : RenderTargetLayerType
    {
        public static LayerKey Key { get; private set; }

        private static GraphicsDevice _graphics;
        private static Effect _windShader;
        private static List<IDrawType> windDraws;
        private static RenderTarget2D _windTarget;

        public override string Name => "HotAndColdCurrent";
        public override SceneLayering Layering => SceneLayering.BehindNPCs;

        protected override void OnRegister(LayerKey key)
        {
            base.OnRegister(key);
            try
            {
                _windShader = AQMod.Instance.GetEffect("Effects/WindShader");
            }
            catch
            {
                throw new Exception("There was an error while loading the Hot and Cold current shader. Try loading shaders in a higher quality or something?");
            }
            windDraws = new List<IDrawType>();
            Key = key;
        }

        internal override void Unload()
        {
            Key = LayerKey.Null;
            _graphics = null;
            _windShader = null;
            windDraws = null;
            _windTarget = null;
            _finalTarget = null;
        }

        public static void AddToCurrentList(IDrawType drawType)
        {
            windDraws.Add(drawType);
        }

        public override void ResetTargets(GraphicsDevice graphics)
        {
            _graphics = graphics;
            if (_graphics == null)
                return;
            _windTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            _finalTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
        }

        public override void DrawTargets()
        {
            if (windDraws == null)
                windDraws = new List<IDrawType>();
            if (windDraws.Count > 0)
            {
                if (_graphics == null)
                    return;
                if (_finalTarget == null || _finalTarget.IsContentLost
                || _windTarget == null || _windTarget.IsContentLost)
                {
                    ResetTargets(Main.instance.GraphicsDevice);
                }
                RenderTargetBinding[] renderTargets;
                try
                {
                    renderTargets = _graphics.GetRenderTargets();
                }
                catch
                {
                    return;
                }

                _graphics.SetRenderTarget(_windTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in windDraws)
                {
                    d.RunDraw();
                }
                windDraws = new List<IDrawType>();

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(_finalTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                _windShader.Parameters["uTime"].SetValue(Main.GlobalTime);
                _windShader.Parameters["uSourceRect"].SetValue(new Vector4(0, 0, Main.screenWidth, Main.screenHeight));
                _windShader.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                _windShader.Techniques[0].Passes["WavyStuffPass"].Apply();

                Main.spriteBatch.Draw(_windTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(_windTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                _windShader.Techniques[0].Passes["DoOutlinesPass"].Apply();

                Main.spriteBatch.Draw(_finalTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(_finalTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin();

                Main.spriteBatch.Draw(_windTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                _graphics.SetRenderTargets(renderTargets);

            }
            else
            {
                _finalTarget = null;
            }
        }

        protected override void PreDrawFinal()
        {
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);

            //_hotAndColdCurrentShader.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            //_hotAndColdCurrentShader.Parameters["uThicknessFromEdge"].SetValue(3f);
            //_hotAndColdCurrentShader.Parameters["uOutlineThickness"].SetValue(2.5f);
            //_hotAndColdCurrentShader.CurrentTechnique.Passes["DoOutlinePass"].Apply();

            _windShader.Parameters["uTime"].SetValue(Main.GlobalTime);
            _windShader.Parameters["uSourceRect"].SetValue(new Vector4(0, 0, Main.screenWidth, Main.screenHeight));
            _windShader.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            _windShader.Techniques[0].Passes["MakeTransparentPass"].Apply();

            //var shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId(ItemID.AcidDye), null);
            //shader.Apply(null, new DrawData(_finalTarget, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0));
        }
        protected override void PostDrawFinal()
        {
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
        }
    }
}