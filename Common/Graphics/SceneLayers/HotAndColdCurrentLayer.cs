using AQMod.Common.Graphics.DrawTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Common.Graphics.SceneLayers
{
    public sealed class HotAndColdCurrentLayer : SceneLayer
    {
        public static LayerKey Key { get; private set; }

        private static GraphicsDevice _graphics;
        private static Effect _hotAndColdCurrentShader;
        private static List<IDrawType> _hotCurrentList;
        private static List<IDrawType> _coldCurrentList;
        private static RenderTarget2D hotTarget;
        private static RenderTarget2D coldTarget;
        private static RenderTarget2D finalTarget;

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
            Reset(Main.graphics.GraphicsDevice);
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
            hotTarget = null;
            coldTarget = null;
            finalTarget = null;
        }

        internal static void Reset(GraphicsDevice graphics)
        {
            _graphics = graphics;
            if (_graphics == null)
                return;
            try
            {
                hotTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
                coldTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
                finalTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            }
            catch
            {

            }
        }

        public static void AddToHotCurrentList(IDrawType drawType)
        {
            _hotCurrentList.Add(drawType);
        }

        public static void AddToCurrentList(IDrawType drawType)
        {
            _coldCurrentList.Add(drawType);
        }

        public static void DrawTarget()
        {
            if (_hotCurrentList == null)
                _hotCurrentList = new List<IDrawType>();
            if (_coldCurrentList == null)
                _coldCurrentList = new List<IDrawType>();
            if (_hotCurrentList.Count > 0 || _coldCurrentList.Count > 0)
            {
                if (finalTarget == null || finalTarget.IsContentLost
                    || hotTarget == null || hotTarget.IsContentLost
                    || coldTarget == null || coldTarget.IsContentLost)
                {
                    Reset(Main.graphics.GraphicsDevice);
                }
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

                _graphics.SetRenderTarget(coldTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in _coldCurrentList)
                {
                    d.RunDraw();
                }

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(hotTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in _hotCurrentList)
                {
                    d.RunDraw();
                }

                Main.spriteBatch.End();

                _graphics.SetRenderTarget(finalTarget);
                _graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                _graphics.Textures[1] = hotTarget;

                _hotAndColdCurrentShader.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                _hotAndColdCurrentShader.CurrentTechnique.Passes["MergeColorsPass"].Apply();

                Main.spriteBatch.Draw(coldTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                _graphics.SetRenderTargets(renderTargets); // renders the finished two layers onto the main render target

                _hotCurrentList = new List<IDrawType>();
                _coldCurrentList = new List<IDrawType>();
            }
            else
            {
                finalTarget = null;
            }
        }

        protected override void Draw()
        {
            if (finalTarget != null && !finalTarget.IsContentLost)
            {
                Main.spriteBatch.End();

                BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);

                //var drawData = new DrawData(finalTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                //EffectCache.s_OutlineColor.UseColor(Color.Black);
                //EffectCache.s_OutlineColor.Apply(drawData);
                //drawData.Draw(Main.spriteBatch);

                _hotAndColdCurrentShader.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                _hotAndColdCurrentShader.Parameters["uThicknessFromEdge"].SetValue(3f);
                _hotAndColdCurrentShader.Parameters["uOutlineThickness"].SetValue(2.5f);
                _hotAndColdCurrentShader.CurrentTechnique.Passes["DoOutlinePass"].Apply();

                Main.spriteBatch.Draw(finalTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
            }
        }
    }
}