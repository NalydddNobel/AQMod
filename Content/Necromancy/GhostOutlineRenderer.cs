using Aequus.Graphics;
using Aequus.Graphics.RenderTargets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy
{
    public class GhostOutlineRenderer : ScreenTarget
    {
        public static StaticMiscShaderInfo Necromancy { get; private set; }
        internal static GhostOutlineRenderer[] necromancyRenderers;
        public static GhostOutlineRenderer[] NecromancyRenderers { get => necromancyRenderers; }

        public readonly List<Projectile> Projs;
        public readonly DrawList NPCs;
        public readonly int Team;
        public int Index;
        public int DisposeTime;
        public Func<Color> DrawColor;

        public static bool RenderingNow { get; set; }

        public static class IDs
        {
            public const int LocalPlayer = 0;
            public const int Zombie = 1;
            public const int Revenant = 2;
            public const int Osiris = 3;
            public const int Insurgent = 4;
            public const int BloodRed = 5;
            public const int Friendship = 6;
            public const int PVPTeams = 7;
            public const int PVPTeams_Red = 8;
            public const int PVPTeams_Green = 9;
            public const int PVPTeams_Blue = 10;
            public const int PVPTeams_Yellow = 11;
            public const int PVPTeams_Purple = 12;

            /// <summary>
            /// Use and increase respectively for custom Necromancy Screen Renderer layers.
            /// </summary>
            public static int Count = 11;
        }

        public GhostOutlineRenderer()
        {
        }

        public GhostOutlineRenderer(int playerTeam, int index, Func<Color> color)
        {
            Team = playerTeam;
            Index = index;
            DrawColor = color;
            NPCs = new DrawList();
            Projs = new List<Projectile>();
        }

        public override void Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                Necromancy = new StaticMiscShaderInfo("NecromancyOutline", "Aequus:NecromancyOutline", "NecromancyOutlinePass", true);
                necromancyRenderers = new GhostOutlineRenderer[]
                {
                    new GhostOutlineRenderer(0, IDs.LocalPlayer, () => Color.White),
                    new GhostOutlineRenderer(-1, IDs.Zombie, () => new Color(100, 149, 237, 255)),
                    new GhostOutlineRenderer(-1, IDs.Revenant, () => new Color(40, 100, 237, 255)),
                    new GhostOutlineRenderer(-1, IDs.Osiris, () => new Color(255, 128, 20, 255)),
                    new GhostOutlineRenderer(-1, IDs.Insurgent, () => new Color(80, 255, 200, 255)),
                    new GhostOutlineRenderer(-1, IDs.BloodRed, () => new Color(255, 10, 10, 255)),
                    new GhostOutlineRenderer(-1, IDs.Friendship, () => new Color(255, 100, 255, 255)),
                };
            }
        }

        public override void Unload()
        {
            Necromancy = null;
            necromancyRenderers = null;
            base.Unload();
        }

        public void Add(int whoAmI)
        {
            NPCs.Add(whoAmI);
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (NPCs.Count > 0)
            {
                if (!ClientConfig.Instance.NecromancyOutlines)
                {
                    NPCs.Clear();
                    return;
                }
                Begin.GeneralEntities.Begin(spriteBatch);
                RenderingNow = true;
                DrawList.ForceRender = true;
                NPCs.renderingNow = true;
                try
                {
                    foreach (var n in NPCs.List)
                    {
                        Main.instance.DrawNPC(n, Main.npc[n].behindTiles);
                    }
                    foreach (var p in Projs)
                    {
                        if (!p.active)
                            continue;
                        Main.instance.DrawProj(p.whoAmI);
                    }
                }
                catch
                {
                }
                RenderingNow = false;
                DrawList.ForceRender = false;
                Projs.Clear();
                NPCs.Clear();
                Main.spriteBatch.End();

                var color = Color.White * 0.5f;
                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                Main.spriteBatch.Begin();

                Main.spriteBatch.Draw(helperTarget, new Rectangle(2, 0, Main.screenWidth / 2, Main.screenHeight / 2), color);
                Main.spriteBatch.Draw(helperTarget, new Rectangle(-2, 0, Main.screenWidth / 2, Main.screenHeight / 2), color);
                Main.spriteBatch.Draw(helperTarget, new Rectangle(0, 2, Main.screenWidth / 2, Main.screenHeight / 2), color);
                Main.spriteBatch.Draw(helperTarget, new Rectangle(0, -2, Main.screenWidth / 2, Main.screenHeight / 2), color);
                Main.spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth / 2, Main.screenHeight / 2), Color.White);

                Main.spriteBatch.End();

                device.SetRenderTarget(helperTarget);
                NPCs.Clear();
                _wasPrepared = true;
            }
        }
        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            DisposeTime = 0;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

            var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), Color.White * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f));
            Necromancy.ShaderData.UseColor(DrawColor());
            Necromancy.ShaderData.Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            _wasPrepared = false;
        }

        public static GhostOutlineRenderer Target(Player player, int suggestedTarget = 0)
        {
            int index = TargetID(player, suggestedTarget);
            if (necromancyRenderers.Length <= index)
            {
                Array.Resize(ref necromancyRenderers, index + 1);
            }
            if (necromancyRenderers[index] == null)
            {
                int team = player.team;
                necromancyRenderers[index] = new GhostOutlineRenderer(team, index, () => Main.teamColor[team]);
            }
            return necromancyRenderers[index];
        }

        public static int TargetID(Player player, int suggestedTarget = 0)
        {
            if (Main.myPlayer == player.whoAmI || player.team == 0 || !player.hostile)
            {
                return Math.Max(suggestedTarget, 0);
            }
            return player.team + IDs.PVPTeams - 1;
        }

        public void CheckDisposal()
        {
            if (DisposeTime > 0)
            {
                if (_target != null || helperTarget != null)
                {
                    DisposeTime++;
                }
            }
            else if (DisposeTime > 120)
            {
                Main.QueueMainThreadAction(() =>
                {
                    DisposeResources();
                });
                DisposeTime = -1;
            }
        }
    }
}