using Aequus.Common.Effects;
using Aequus.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy.Renderer
{
    public class GhostRenderer : ScreenTarget
    {
        public static LegacyMiscShaderWrap NecromancyShader { get; private set; }
        public static GhostRenderer Instance { get; private set; }
        public static List<RenderTarget2D> OrphanedRenderTargets { get; private set; }
        public static RenderData[] Colors { get; private set; }
        public static List<(NPC, NPC)> ChainedUpNPCs { get; private set; }

        public static bool Rendering { get; set; }

        public GhostRenderer()
        {
        }

        protected override void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);
        }

        public override void Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                Instance = this;
                ChainedUpNPCs = new List<(NPC, NPC)>();
                OrphanedRenderTargets = new List<RenderTarget2D>();
                Colors = new RenderData[]
                {
                    new RenderData(() => Color.White), // None
                    new RenderData(() => new Color(100, 149, 237, 255)), // Zombie Scepter
                    new RenderData(() => new Color(40, 100, 237, 255)), // Revenant
                    new RenderData(() => new Color(255, 128, 20, 255)), // Osiris
                    new RenderData(() => new Color(80, 255, 200, 255)), // Insurgency
                    new RenderData(() => new Color(255, 10, 10, 255)), // Blood Red
                    new RenderData(() => new Color(255, 100, 255, 255)), // Friendship Magick
                    new RenderData(() => Color.BlueViolet), // Demon Purple
                    new RenderData(() => Color.Blue), // Dungeon Dark Blue
                    new RenderData(() => Main.teamColor[0]), // PVP Team White
                    new RenderData(() => Main.teamColor[1]), // PVP Team Red
                    new RenderData(() => Main.teamColor[2]), // PVP Team Green
                    new RenderData(() => Main.teamColor[3]), // PVP Team Blue
                    new RenderData(() => Main.teamColor[4]), // PVP Team Yellow
                    new RenderData(() => Main.teamColor[5]), // PVP Team Purple
                };
                Main.QueueMainThreadAction(() =>
                {
                    if (Main.spriteBatch != null && Main.spriteBatch.GraphicsDevice != null)
                    {
                        PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, Main.spriteBatch.GraphicsDevice, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);
                    }
                    if (Main.spriteBatch != null && Main.spriteBatch.GraphicsDevice != null)
                    {
                        RenderTarget2D target = null;
                        PrepareARenderTarget_WithoutListeningToEvents(ref target, Main.spriteBatch.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetUsage.DiscardContents);
                        OrphanedRenderTargets.Add(target);
                    }
                });
                NecromancyShader = new LegacyMiscShaderWrap("Aequus/Assets/Effects/NecromancyOutline", "Aequus:NecromancyOutline", "NecromancyOutlinePass", true);
            }
        }

        public override void DisposeResources()
        {
            OrphanedRenderTargets?.Clear();
            OrphanedRenderTargets = null;
            base.DisposeResources();
        }

        public override void Unload()
        {
            Instance = null;
            NecromancyShader = null;
            Colors = null;
            base.Unload();
        }

        public static void DrawChainedNPCs()
        {
            if (ChainedUpNPCs.Count > 0)
            {
                var t = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}SoulChains").Value;
                foreach (var v in ChainedUpNPCs)
                {
                    int i = 0;
                    Helper.DrawChain(t, v.Item2.Center, v.Item1.Center, Main.screenPosition, (loc) =>
                    {
                        var zombie = v.Item1.GetGlobalNPC<NecromancyNPC>();
                        float m = 0.5f;
                        if (zombie.ghostChainsTime < 30)
                        {
                            m *= zombie.ghostChainsTime / 30f;
                        }
                        return GetColorTarget(Main.player[zombie.zombieOwner], zombie.renderLayer).getDrawColor().UseA(128) * m;
                    });
                }
                ChainedUpNPCs.Clear();
            }
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            Rendering = true;
            LegacyDrawList.ForceRender = true;
            for (int i = 0; i < Colors.Length; i++)
            {
                RenderData render = Colors[i];
                try
                {
                    device.Clear(Color.Transparent);
                    if (!render.ContainsContents())
                    {
                        render.CheckSettingAdoption();
                        continue;
                    }

                    if (!render.CheckRenderTarget(out bool request))
                    {
                        if (request && OrphanedRenderTargets.Count < ColorTargetID.Count)
                        {
                            RenderTarget2D target = null;
                            PrepareARenderTarget_WithoutListeningToEvents(ref target, Main.spriteBatch.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetUsage.DiscardContents);
                            OrphanedRenderTargets.Add(target);
                        }
                        continue;
                    }

                    render.setRenderTargetForAdoption = 0;
                    render.DrawContents(spriteBatch);

                    device.SetRenderTarget(render.renderTargetCache);
                    device.Clear(Color.Transparent);

                    Main.spriteBatch.Begin();

                    Main.spriteBatch.Draw(helperTarget, new Rectangle(2, 0, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                    Main.spriteBatch.Draw(helperTarget, new Rectangle(-2, 0, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                    Main.spriteBatch.Draw(helperTarget, new Rectangle(0, 2, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                    Main.spriteBatch.Draw(helperTarget, new Rectangle(0, -2, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                    Main.spriteBatch.Draw(helperTarget, new Rectangle(0, 0, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White);

                    Main.spriteBatch.End();
                    device.SetRenderTarget(helperTarget);
                    _wasPrepared = true;
                }
                catch
                {
                }
            }
            Rendering = false;
            LegacyDrawList.ForceRender = false;
        }
        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            foreach (var render in Colors)
            {
                try
                {
                    if (render.renderTargetCache == null || !render.ContainsContents() || !render.CheckRenderTarget(out bool _))
                        continue;

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

                    var drawData = new DrawData(render.renderTargetCache, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f));
                    NecromancyShader.ShaderData.UseColor(render.getDrawColor());
                    NecromancyShader.ShaderData.Apply(drawData);

                    drawData.Draw(spriteBatch);

                    spriteBatch.End();
                }
                catch
                {
                }
            }
            _wasPrepared = false;
        }

        protected override bool SelfRequest()
        {
            return true;
        }

        public static RenderData GetColorTarget(Player player, int suggestedTarget = 0)
        {
            return Colors[ColorID(player, suggestedTarget)];
        }

        public static int ColorID(Player player, int suggestedTarget = 0)
        {
            if (Main.myPlayer == player.whoAmI || player.team == 0 || !player.hostile)
            {
                return Math.Max(suggestedTarget, 0);
            }
            return player.team + ColorTargetID.TeamWhite;
        }
    }
}