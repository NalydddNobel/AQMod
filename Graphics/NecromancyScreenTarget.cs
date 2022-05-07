using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Graphics
{
    public sealed class NecromancyScreenTarget : ScreenTarget
    {
        public DrawIndexCache NPCs { get; private set; }

        public static bool RenderingNow => EffectsSystem.NecromancyDrawer.NPCs.renderingNow;

        public NecromancyScreenTarget()
        {
            NPCs = new DrawIndexCache();
        }

        public static void Add(int whoAmI)
        {
            EffectsSystem.NecromancyDrawer.NPCs.Add(whoAmI);
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (NPCs.Count > 0)
            {
                CommonSpriteBatchBegins.GeneralEntities.Begin(spriteBatch);
                NPCs.renderingNow = true;
                try
                {
                    foreach (var n in NPCs.List)
                    {
                        Main.instance.DrawNPC(n, Main.npc[n].behindTiles);
                    }
                }
                catch
                {
                }
                NPCs.renderingNow = false;
                Main.spriteBatch.End();

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                Main.spriteBatch.Begin();
                Main.spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                Main.spriteBatch.End();

                device.SetRenderTarget(helperTarget);
                NPCs.Clear();
                _wasPrepared = true;
            }
        }
        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

            var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f));
            ModEffects.NecromancyOutlineShader.UseColor(Color.CornflowerBlue);
            ModEffects.NecromancyOutlineShader.Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            _wasPrepared = false;
        }
    }
}