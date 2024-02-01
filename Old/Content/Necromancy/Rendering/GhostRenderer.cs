using Aequus.Core.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Necromancy.Rendering;

public class GhostRenderer : ModSystem {
    public static Asset<Effect> OutlineShader { get; private set; }
    public static GhostRenderer Instance { get; private set; }
    public static RenderData[] Colors { get; private set; }

    public static readonly List<(NPC, NPC)> ChainedUpNPCs = new();

    public static readonly List<RenderTarget2D> OrphanedRenderTargets = new();

    public static bool Rendering { get; set; }

    private static RenderTarget2D _helperTarget;

    public static bool WasPrepared { get; private set; }
    private static bool _requestedOld;
    public static bool Requested { get; set; }

    protected static void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
        if (!Requested) {
            if (_requestedOld) {
                _requestedOld = false;
                DrawLayers.Instance.PostDrawDust -= DrawOntoScreen;
            }
            return;
        }

        if (!_requestedOld) {
            _requestedOld = true;
            DrawLayers.Instance.PostDrawDust += DrawOntoScreen;
        }
        Rendering = true;

        Requested = false;
        ExtendRenderTarget.CheckRenderTarget2D(ref _helperTarget, Main.screenWidth, Main.screenHeight);
        RenderTargetBinding[] bindings = device.GetRenderTargets();
        device.SetRenderTarget(_helperTarget);

        for (int i = 0; i < Colors.Length; i++) {
            RenderData render = Colors[i];
            try {
                if (!render.ContainsContents()) {
                    render.CheckSettingAdoption();
                    continue;
                }
                device.Clear(Color.Transparent);

                if (!render.CheckRenderTarget(out bool request)) {
                    if (request && OrphanedRenderTargets.Count < ColorTargetID.Count) {
                        RenderTarget2D target = null;
                        ExtendRenderTarget.CheckRenderTarget2D(ref target, Main.screenWidth / 2, Main.screenHeight / 2);
                        OrphanedRenderTargets.Add(target);
                    }
                    continue;
                }

                render.setRenderTargetForAdoption = 0;
                render.DrawContents(spriteBatch);

                device.SetRenderTarget(render.renderTargetCache);
                device.Clear(Color.Transparent);

                Main.spriteBatch.Begin();

                Main.spriteBatch.Draw(_helperTarget, new Rectangle(2, 0, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                Main.spriteBatch.Draw(_helperTarget, new Rectangle(-2, 0, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                Main.spriteBatch.Draw(_helperTarget, new Rectangle(0, 2, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                Main.spriteBatch.Draw(_helperTarget, new Rectangle(0, -2, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White * 0.5f);
                Main.spriteBatch.Draw(_helperTarget, new Rectangle(0, 0, render.renderTargetCache.Width, render.renderTargetCache.Height), Color.White);

                Main.spriteBatch.End();
                device.SetRenderTarget(_helperTarget);
                WasPrepared = true;
            }
            catch {
            }
        }

        device.SetRenderTargets(bindings);
        Rendering = false;
    }

    public static void DrawOntoScreen(SpriteBatch spriteBatch) {
        foreach (var render in Colors) {
            try {
                if (render.renderTargetCache == null || !render.ContainsContents() || !render.CheckRenderTarget(out bool _)) {
                    continue;
                }

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.Transform);

                var drawData = new DrawData(render.renderTargetCache, Main.screenLastPosition- Main.screenPosition, null, Color.White * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f), 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                OutlineShader.Value.Parameters["uColor"].SetValue(render.getDrawColor().ToVector3());
                OutlineShader.Value.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                OutlineShader.Value.CurrentTechnique.Passes[0].Apply();

                drawData.Draw(spriteBatch);

                spriteBatch.End();
            }
            catch {
            }
        }

    }

    public static RenderData GetColorTarget(Player player, int suggestedTarget = 0) {
        return Colors[ColorID(player, suggestedTarget)];
    }

    public static int ColorID(Player player, int suggestedTarget = 0) {
        if (Main.myPlayer == player.whoAmI || player.team == 0 || !player.hostile) {
            return Math.Max(suggestedTarget, 0);
        }
        return player.team + ColorTargetID.TeamWhite;
    }

    public override void Load() {
        if (Main.netMode != NetmodeID.Server) {
            Instance = this;
            Main.OnPreDraw += Main_OnPreDraw;
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
                new RenderData(() => Color.LimeGreen), // Booger Green
                new RenderData(() => Main.teamColor[0]), // PVP Team White
                new RenderData(() => Main.teamColor[1]), // PVP Team Red
                new RenderData(() => Main.teamColor[2]), // PVP Team Green
                new RenderData(() => Main.teamColor[3]), // PVP Team Blue
                new RenderData(() => Main.teamColor[4]), // PVP Team Yellow
                new RenderData(() => Main.teamColor[5]), // PVP Team Purple
            };
            Main.QueueMainThreadAction(() => {
                if (Main.spriteBatch != null && Main.spriteBatch.GraphicsDevice != null) {
                    ExtendRenderTarget.CheckRenderTarget2D(ref _helperTarget, Main.screenWidth, Main.screenHeight);
                }
                if (Main.spriteBatch != null && Main.spriteBatch.GraphicsDevice != null) {
                    RenderTarget2D target = null;
                    ExtendRenderTarget.CheckRenderTarget2D(ref target, Main.screenWidth, Main.screenHeight);
                    OrphanedRenderTargets.Add(target);
                }
            });

            OutlineShader = ModContent.Request<Effect>("Aequus/Old/Assets/Shaders/NecromancyOutline");
        }
    }

    public override void Unload() {
        Instance = null;
        OutlineShader = null;
        Colors = null;
        ChainedUpNPCs.Clear();
        OrphanedRenderTargets.Clear();
        Main.OnPreDraw -= Main_OnPreDraw;

        if (Main.netMode != NetmodeID.Server) {
            Main.QueueMainThreadAction(() => {
                foreach (var t in OrphanedRenderTargets.Where(t => !t.IsDisposed)) {
                    t.Dispose();
                }

                OrphanedRenderTargets.Clear();
                if (!_helperTarget.IsDisposed) {
                    _helperTarget.Dispose();
                }
                _helperTarget = null;
            });
        }

        base.Unload();
    }

    private static void Main_OnPreDraw(GameTime obj) {
        DrawOntoTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
    }
}