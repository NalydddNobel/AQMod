using Aequus.Common.Effects;
using Aequus.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons
{
    public class HealerDroneRenderer : ScreenTarget
    {
        public readonly List<(int, int, float)> HealPairs;

        public static bool RenderingNow;

        public static HealerDroneRenderer Instance { get; private set; }

        public HealerDroneRenderer()
        {
            HealPairs = new List<(int, int, float)>();
        }

        public override void Load(Mod mod)
        {
            base.Load(mod);
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
            base.Unload();
        }

        public void AddHealingAura(int npc, int proj, float opacity)
        {
            HealPairs.Add((npc, proj, opacity));
            Request();
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (HealPairs.Count > 0)
            {
                RenderingNow = true;
                LegacyDrawList.ForceRender = true;
                try
                {
                    bool cleared = false;
                    foreach (var pair in HealPairs)
                    {
                        Main.spriteBatch.Begin_World(shader: true);
                        var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                        s.Apply(null);

                        bool flag = Main.gameMenu;
                        Main.gameMenu = true;

                        Main.instance.DrawNPC(pair.Item1, Main.npc[pair.Item1].behindTiles);
                        Main.instance.DrawNPC(pair.Item2, Main.npc[pair.Item2].behindTiles);

                        Main.gameMenu = flag;

                        Main.spriteBatch.End();
                        device.SetRenderTarget(_target);
                        if (!cleared)
                        {
                            cleared = true;
                            device.Clear(Color.Transparent);
                        }
                        Main.spriteBatch.Begin();

                        var circular = Helper.CircularVector(8, Main.GlobalTimeWrappedHourly * 2f);

                        for (int i = 0; i < circular.Length; i++)
                        {
                            Main.spriteBatch.Draw(helperTarget, circular[i] * Main.GameViewMatrix.Zoom * 8f, Color.White * 0.15f * pair.Item3);
                        }
                        foreach (var v in Helper.CircularVector(4))
                        {
                            Main.spriteBatch.Draw(helperTarget, v * Main.GameViewMatrix.Zoom * 2f, Color.White * 0.5f * pair.Item3);
                        }
                        Main.spriteBatch.Draw(helperTarget, new Vector2(0f, 0f), Color.White * pair.Item3);

                        Main.spriteBatch.End();
                        device.SetRenderTarget(helperTarget);
                        device.Clear(Color.Transparent);
                    }
                }
                catch
                {
                }
                RenderingNow = false;
                LegacyDrawList.ForceRender = false;
                HealPairs.Clear();

                //device.SetRenderTarget(helperTarget);
                _wasPrepared = true;
            }
        }

        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            spriteBatch.Draw(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), CombatText.HealLife);
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false);
            _wasPrepared = false;
        }
    }
}