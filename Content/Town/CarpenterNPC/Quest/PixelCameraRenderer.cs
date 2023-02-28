using Aequus.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Content.Town.CarpenterNPC.Quest
{
    public class PixelCameraRenderer : RequestableRenderTarget
    {
        public struct RequestInfo
        {
            public Ref<RenderTarget2D> target;
            public int width;
            public int height;
            public Color[] arr;
        }

        public static List<RequestInfo> RenderRequests { get; private set; }

        public override void Load(Mod mod)
        {
            base.Load(mod);
            RenderRequests = new List<RequestInfo>();
        }

        public override void Unload()
        {
            RenderRequests?.Clear();
            RenderRequests = null;
        }

        protected override void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (RenderRequests.Count == 0 || RenderRequests[0].width == 0 || RenderRequests[0].height == 0 || RenderRequests[0].target == null || RenderRequests[0].arr == null || RenderRequests[0].width * RenderRequests[0].height != RenderRequests[0].arr.Length)
                return;

            PrepareARenderTarget_WithoutListeningToEvents(ref _target, device, RenderRequests[0].width, RenderRequests[0].height, RenderTargetUsage.PreserveContents);
            PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, device, RenderRequests[0].width, RenderRequests[0].height, RenderTargetUsage.DiscardContents);
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (RenderRequests.Count == 0 || !IsTargetUsable(_target) || !IsTargetUsable(helperTarget)
                || RenderRequests[0].width == 0 || RenderRequests[0].height == 0 || RenderRequests[0].target == null
                || RenderRequests[0].arr == null || RenderRequests[0].width * RenderRequests[0].height != RenderRequests[0].arr.Length)
            {
                RenderRequests.RemoveAt(0);
                return;
            }

            bool end = false;
            try
            {
                spriteBatch.Begin();
                end = true;
                for (int i = 0; i < RenderRequests[0].width; i++)
                {
                    for (int j = 0; j < RenderRequests[0].height; j++)
                    {
                        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(i, j, 1, 1), RenderRequests[0].arr[i + j * RenderRequests[0].width]);
                    }
                }
                spriteBatch.End();
                end = false;

                spriteBatch.Begin();
                end = true;
                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                spriteBatch.Draw(helperTarget, new Rectangle(0, 0, helperTarget.Width, helperTarget.Height), Color.White);

                spriteBatch.End();
                end = false;

                RenderRequests[0].target.Value = _target;
            }
            catch (Exception ex)
            {
                Main.NewText(ex);
                if (end)
                    spriteBatch.End();
            }
            _target = null;

            RenderRequests.RemoveAt(0);

        }

        protected override bool SelfRequest()
        {
            return RenderRequests.Count > 0;
        }
    }
}