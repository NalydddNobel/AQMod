using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Graphics
{
    public sealed class NecromancyScreenRenderer : ScreenTarget
    {
        public readonly DrawIndexCache NPCs;
        public readonly int Team;
        public int Index;
        public Func<Color> DrawColor;

        public static bool RenderingNow;

        public static class TargetIDs
        {
            public const int LocalPlayer = 0;
            public const int FriendlyZombie = 1;
            public const int FriendlyRevenant = 2;
            public const int FriendlyOsiris = 3;
            public const int FriendlyInsurgent = 4;
            public const int FriendlyBloodSacrifice = 5;
            public const int PVPTeams = 6;
            public const int PVPTeams_Red = 6;
            public const int PVPTeams_Green = 7;
            public const int PVPTeams_Blue = 8;
            public const int PVPTeams_Yellow = 9;
            public const int PVPTeams_Purple = 10;

            /// <summary>
            /// Use and increase respectively for custom Necromancy Screen Renderer layers.
            /// </summary>
            public static int Count = 11;
        }

        public NecromancyScreenRenderer(int playerTeam, int index, Func<Color> color)
        {
            Team = playerTeam;
            Index = index;
            DrawColor = color;
            NPCs = new DrawIndexCache();
        }

        public void Add(int whoAmI)
        {
            NPCs.Add(whoAmI);
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (NPCs.Count > 0)
            {
                CommonSpriteBatchBegins.GeneralEntities.Begin(spriteBatch);
                RenderingNow = true;
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
                RenderingNow = false;
                NPCs.renderingNow = false;
                Main.spriteBatch.End();

                var color = Color.White * 0.5f;
                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                Main.spriteBatch.Begin();

                Main.spriteBatch.Draw(helperTarget, new Vector2(2f, 0f), color);
                Main.spriteBatch.Draw(helperTarget, new Vector2(-2f, 0f), color);
                Main.spriteBatch.Draw(helperTarget, new Vector2(0f, 2f), color);
                Main.spriteBatch.Draw(helperTarget, new Vector2(0f, -2f), color);
                Main.spriteBatch.Draw(helperTarget, new Vector2(0f, 0f), Color.White);

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
            ModEffects.NecromancyOutlineShader.UseColor(DrawColor());
            ModEffects.NecromancyOutlineShader.Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            _wasPrepared = false;
        }

        public static int GetScreenTargetIndex(Player player, int suggestedTarget = 0)
        {
            if (Main.myPlayer == player.whoAmI && (player.team == 0 || !player.hostile))
            {
                return Math.Max(suggestedTarget, 0);
            }
            return player.team + TargetIDs.PVPTeams - 1;
        }
    }
}