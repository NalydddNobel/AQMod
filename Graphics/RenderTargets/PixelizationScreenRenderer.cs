using Aequus.Common.Utilities.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Graphics.RenderTargets
{
    public class PixelizationScreenRenderer : ScreenTarget
    {
        private static Dictionary<string, List<Action<SpriteBatch>>> RenderQueue;
        private static Dictionary<string, RenderTarget2D> RenderResult;

        public override int FinalResultResolutionDiv => 2;

        public static void PrepareRender(string name, Action<SpriteBatch> action)
        {
            if (RenderQueue.TryGetValue(name, out var val))
            {
                val.Add(action);
            }
            else
            {
                RenderQueue[name] = new List<Action<SpriteBatch>>() { action };
            }
        }

        public static bool TryGetResult(string name, out RenderTarget2D result)
        {
            return RenderResult.TryGetValue(name, out result);
        }

        public override void Load(Mod mod)
        {
            base.Load(mod);
            RenderQueue = new Dictionary<string, List<Action<SpriteBatch>>>();
            RenderResult = new Dictionary<string, RenderTarget2D>();
        }

        public override void Unload()
        {
            RenderQueue?.Clear();
            RenderQueue = null;
            RenderResult?.Clear();
            RenderResult = null;
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            foreach (var renderList in RenderQueue)
            {
                Main.spriteBatch.Begin_World(shader: false);;

                foreach (var renderMethod in renderList.Value)
                {
                    try
                    {
                        renderMethod(spriteBatch);
                    }
                    catch
                    {
                    }
                }


                if (RenderResult.TryGetValue(renderList.Key, out var renderTarget))
                {
                    _target = renderTarget;
                }
                else
                {
                    _target = null;
                }
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv, RenderTargetUsage.PreserveContents);

                spriteBatch.End();

                spriteBatch.Begin();
                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv), Color.White);

                spriteBatch.End();
                RenderResult[renderList.Key] = _target;
            }
            RenderQueue.Clear();
            _wasPrepared = true;
        }

        protected override bool SelfRequest()
        {
            return RenderQueue.Count > 0;
        }
    }
}