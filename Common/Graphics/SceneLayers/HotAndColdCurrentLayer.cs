using AQMod.Common.Graphics.DrawTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Common.Graphics.SceneLayers
{
    public sealed class HotAndColdCurrentLayer : RenderTargetLayerType
    {
        public static LayerKey Key { get; private set; }

        private static GraphicsDevice _graphics;
        private static Effect _hotAndColdCurrentShader;
        private static List<IDrawType> _hotCurrentList;
        private static List<IDrawType> _coldCurrentList;
        private static RenderTarget2D _hotTarget;
        private static RenderTarget2D _coldTarget;

        public override string Name => "HotAndColdCurrent";
        public override SceneLayering Layering => SceneLayering.BehindNPCs;

        protected override void OnRegister(LayerKey key)
        {
            try
            {
                _hotAndColdCurrentShader = AQMod.Instance.GetEffect("Effects/HotAndColdCurrent/HotAndColdCurrent");
            }
            catch
            {
                throw new Exception("There was an error while loading the Hot and Cold current shader. Try loading shaders in a higher quality or something?");
            }
            _hotCurrentList = new List<IDrawType>();
            _coldCurrentList = new List<IDrawType>();
            Key = key;
        }

        internal override void Unload()
        {
            Key = LayerKey.Null;
            _graphics = null;
            _hotAndColdCurrentShader = null;
            _hotCurrentList = null;
            _coldCurrentList = null;
            _hotTarget = null;
            _coldTarget = null;
            _finalTarget = null;
        }

        public static void AddToHotCurrentList(IDrawType drawType)
        {
            _hotCurrentList.Add(drawType);
        }

        public static void AddToCurrentList(IDrawType drawType)
        {
            _coldCurrentList.Add(drawType);
        }

        public override bool ShouldReset()
        {
            return base.ShouldReset() 
                || _finalTarget == null || _finalTarget.IsContentLost
                || _hotTarget == null || _hotTarget.IsContentLost
                || _coldTarget == null || _coldTarget.IsContentLost;
        }

        public override void ResetTargets(GraphicsDevice graphics)
        {
            _graphics = graphics;
            if (_graphics == null)
                return;
            _hotTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            _coldTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            _finalTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
        }

        public override void DrawTargets()
        {
            if (_hotCurrentList == null)
                _hotCurrentList = new List<IDrawType>();
            if (_coldCurrentList == null)
                _coldCurrentList = new List<IDrawType>();
            if (_hotCurrentList.Count > 0 || _coldCurrentList.Count > 0)
            {
                if (_graphics == null)
                    return;
                RenderTargetBinding[] renderTargets;
                try
                {
                    renderTargets = _graphics.GetRenderTargets();
                }
                catch
                {
                    return;
                }

                _graphics.SetRenderTarget(_coldTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in _coldCurrentList)
                {
                    d.RunDraw();
                }

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(_hotTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in _hotCurrentList)
                {
                    d.RunDraw();
                }

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(_finalTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                _graphics.Textures[1] = _hotTarget;

                _hotAndColdCurrentShader.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                _hotAndColdCurrentShader.CurrentTechnique.Passes["MergeColorsPass"].Apply();

                Main.spriteBatch.Draw(_coldTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                _graphics.SetRenderTargets(renderTargets); // renders the finished two layers onto the main render target

                _hotCurrentList = new List<IDrawType>();
                _coldCurrentList = new List<IDrawType>();
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
            _hotAndColdCurrentShader.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            _hotAndColdCurrentShader.Parameters["uThicknessFromEdge"].SetValue(3f);
            _hotAndColdCurrentShader.Parameters["uOutlineThickness"].SetValue(2.5f);
            _hotAndColdCurrentShader.CurrentTechnique.Passes["DoOutlinePass"].Apply();
        }
        protected override void PostDrawFinal()
        {
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
        }
    }
}