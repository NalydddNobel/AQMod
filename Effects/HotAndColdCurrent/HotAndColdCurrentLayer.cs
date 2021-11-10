using AQMod.Assets.DrawCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects.HotAndColdCurrent
{
    public class HotAndColdCurrentLayer : SceneLayer
    {
        public const string Name = "HotAndColdCurrent";
        public const SceneLayering Layer = SceneLayering.BehindNPCs;

        public override void OnLoad()
        {
            reset(Main.graphics.GraphicsDevice);
            try
            {
                _hotAndColdCurrentShader = AQMod.Instance.GetEffect("Effects/HotAndColdCurrent/HotAndColdCurrent");
            }
            catch
            {
                throw new Exception("There was an error while loading the Hot and Cold current shader.");
            }
            _hotCurrentList = new List<IDrawType>();
            _coldCurrentList = new List<IDrawType>();
        }

        internal static void Reset(GraphicsDevice graphics)
        {
            ((HotAndColdCurrentLayer)AQMod.WorldLayers.GetLayer(Layer, Name)).reset(graphics);
        }

        private void reset(GraphicsDevice graphics)
        {
            this.graphics = graphics;
            hotTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            coldTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            finalTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
        }

        private GraphicsDevice graphics;

        private Effect _hotAndColdCurrentShader;

        private List<IDrawType> _hotCurrentList;
        private List<IDrawType> _coldCurrentList;

        public static bool TargetsDrawn { get; private set; }

        public static void AddToHotCurrentList(IDrawType drawType)
        {
            ((HotAndColdCurrentLayer)AQMod.WorldLayers.GetLayer(Layer, Name))._hotCurrentList.Add(drawType);
        }

        public static void AddToColdCurrentList(IDrawType drawType)
        {
            ((HotAndColdCurrentLayer)AQMod.WorldLayers.GetLayer(Layer, Name))._coldCurrentList.Add(drawType);
        }

        public static void DrawTarget()
        {
            ((HotAndColdCurrentLayer)AQMod.WorldLayers.GetLayer(Layer, Name)).drawTarget();
        }

        private void drawTarget()
        {
            if (_hotCurrentList.Count > 0 && _coldCurrentList.Count > 0)
            {
                if (finalTarget == null || hotTarget == null || coldTarget == null)
                {
                    reset(Main.graphics.GraphicsDevice);
                }
                var renderTargets = graphics.GetRenderTargets();

                graphics.SetRenderTarget(coldTarget);
                graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in _coldCurrentList)
                {
                    d.RunDraw();
                }

                Main.spriteBatch.End();

                graphics.SetRenderTarget(hotTarget);
                graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                foreach (var d in _hotCurrentList)
                {
                    d.RunDraw();
                }

                Main.spriteBatch.End();

                graphics.SetRenderTarget(finalTarget);
                graphics.Clear(new Color(0, 0, 0, 0));

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

                graphics.Textures[1] = hotTarget;

                _hotAndColdCurrentShader.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));

                _hotAndColdCurrentShader.CurrentTechnique.Passes["MergeColorsPass"].Apply();

                Main.spriteBatch.Draw(coldTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                graphics.SetRenderTargets(renderTargets); // renders the finished two layers onto the main render target

                _hotCurrentList = new List<IDrawType>();
                _coldCurrentList = new List<IDrawType>();
                TargetsDrawn = true;
            }
            else
            {
                finalTarget = null;
            }
        }

        private RenderTarget2D hotTarget;
        private RenderTarget2D coldTarget;

        private RenderTarget2D finalTarget;

        protected override void Draw()
        {
            if (finalTarget != null && !finalTarget.IsContentLost)
            {
                Main.spriteBatch.End();

                BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);

                _hotAndColdCurrentShader.Parameters["uThicknessFromEdge"].SetValue(3f);
                _hotAndColdCurrentShader.Parameters["uOutlineThickness"].SetValue(2.5f);
                _hotAndColdCurrentShader.CurrentTechnique.Passes["DoOutlinePass"].Apply();

                Main.spriteBatch.Draw(finalTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();

                BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);
            }
            else if (!TargetsDrawn)
            {
                if (_coldCurrentList.Count > 0)
                {
                    Main.spriteBatch.End();

                    BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);

                    _hotAndColdCurrentShader.Parameters["uThicknessFromEdge"].SetValue(3f);
                    _hotAndColdCurrentShader.Parameters["uOutlineThickness"].SetValue(2.5f);
                    _hotAndColdCurrentShader.CurrentTechnique.Passes["DoOutlinePass"].Apply();

                    foreach (var d in _coldCurrentList)
                    {
                        d.RunDraw();
                    }

                    Main.spriteBatch.End();

                    BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);

                    _coldCurrentList = new List<IDrawType>();
                }
                else if (_hotCurrentList.Count > 0)
                {
                    Main.spriteBatch.End();

                    BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);

                    _hotAndColdCurrentShader.Parameters["uThicknessFromEdge"].SetValue(3f);
                    _hotAndColdCurrentShader.Parameters["uOutlineThickness"].SetValue(2.5f);
                    _hotAndColdCurrentShader.CurrentTechnique.Passes["DoOutlinePass"].Apply();

                    foreach (var d in _hotCurrentList)
                    {
                        d.RunDraw();
                    }

                    Main.spriteBatch.End();

                    BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);

                    _hotCurrentList = new List<IDrawType>();
                }
            }
            TargetsDrawn = false;
        }
    }
}