using AQMod.Common;
using AQMod.Common.Graphics.DrawTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects.Wind
{
    public sealed class WindLayer : IAutoloadType
    {
        private static Effect _windShader;
        private static GraphicsDevice _graphics;
        public static List<IDrawType> windDraws;
        private static RenderTarget2D _windTarget;
        private static RenderTarget2D _finalTarget;

        public static void AddToCurrentList(IDrawType drawType)
        {
            windDraws.Add(drawType);
        }

        public static void ResetTargets(GraphicsDevice graphics)
        {
            _graphics = graphics;
            if (graphics == null || graphics.IsDisposed)
                return;
            try
            {
                _windTarget?.Dispose();
                _windTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
                _finalTarget?.Dispose();
                _finalTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            }
            catch
            {
                _windTarget?.Dispose();
                _windTarget = null;
                _finalTarget?.Dispose();
                _finalTarget = null;
            }
        }

        internal static void DrawTargets()
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

        internal static void DrawFinal()
        {
            if (_finalTarget != null && !_finalTarget.IsDisposed && !_finalTarget.IsContentLost)
            {
                Main.spriteBatch.End();
                BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);

                _windShader.Parameters["uTime"].SetValue(Main.GlobalTime);
                _windShader.Parameters["uSourceRect"].SetValue(new Vector4(0, 0, Main.screenWidth, Main.screenHeight));
                _windShader.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                _windShader.Techniques[0].Passes["MakeTransparentPass"].Apply();

                Main.spriteBatch.Draw(_finalTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
            }
        }

        void IAutoloadType.OnLoad()
        {
            if (!Main.dedServ)
            {
                try
                {
                    _windShader = AQMod.GetInstance().GetEffect("Effects/Wind/WindShader");
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error while loading the Wind current shader. Try loading shaders in a higher/lower quality or smth?", ex);
                }
                windDraws = new List<IDrawType>();
            }
        }

        void IAutoloadType.Unload()
        {
            _graphics = null;
            _windShader = null;
            windDraws = null;
            _windTarget = null;
            _finalTarget = null;
        }
    }
}