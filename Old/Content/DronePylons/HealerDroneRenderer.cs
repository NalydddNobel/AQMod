using Aequus.Core.Graphics;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.DronePylons;
public class HealerDroneRenderer : ModSystem {
    public readonly List<(int, int, float)> HealPairs;

    public static bool RenderingNow { get; private set; }

    public static HealerDroneRenderer Instance { get; private set; }

    private RenderTarget2D _swapTarget;
    private RenderTarget2D _resultTarget;
    private bool _wasPrepared;

    public HealerDroneRenderer() {
        HealPairs = new List<(int, int, float)>();
    }

    public override void Load() {
        Instance = this;
        if (Main.netMode != NetmodeID.Server) {
            Main.OnPreDraw += RenderToTarget;
        }
    }

    public override void SetStaticDefaults() {
        if (Main.netMode != NetmodeID.Server) {
            DrawLayers.Instance.WorldBehindTiles += DrawOntoScreen;
        }
    }

    public override void Unload() {
        if (Main.netMode != NetmodeID.Server) {
            Main.OnPreDraw -= RenderToTarget;
            Main.QueueMainThreadAction(() => {
                DrawHelper.DiscardTarget(ref _swapTarget);
            });
        }
        Instance = null;
    }

    public void AddHealingAura(int npc, int proj, float opacity) {
        HealPairs.Add((npc, proj, opacity));
    }

    protected void RenderToTarget(GameTime gameTime) {
        if (HealPairs.Count <= 0 || !Main.IsGraphicsDeviceAvailable) {
            _wasPrepared = false;
            return;
        }

        GraphicsDevice device = Main.instance.GraphicsDevice;

        RenderTargetBinding[] oldTargets = device.GetRenderTargets();

        RenderingNow = true;
        try {
            if (Main.IsGraphicsDeviceAvailable) {
                if (DrawHelper.BadRenderTarget(_swapTarget, Main.screenWidth, Main.screenHeight)) {
                    _swapTarget = new RenderTarget2D(device, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
                }
                if (DrawHelper.BadRenderTarget(_resultTarget, Main.screenWidth, Main.screenHeight)) {
                    _resultTarget = new RenderTarget2D(device, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
                }
            }

            bool cleared = false;
            foreach (var pair in HealPairs) {
                Main.spriteBatch.BeginWorld(shader: true);
                var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                s.Apply(null);

                bool flag = Main.gameMenu;
                Main.gameMenu = true;

                Main.instance.DrawNPC(pair.Item1, Main.npc[pair.Item1].behindTiles);
                Main.instance.DrawNPC(pair.Item2, Main.npc[pair.Item2].behindTiles);

                Main.gameMenu = flag;

                Main.spriteBatch.End();
                device.SetRenderTarget(_resultTarget);
                if (!cleared) {
                    cleared = true;
                    device.Clear(Color.Transparent);
                }
                Main.spriteBatch.Begin();

                for (int i = 0; i < 8; i++) {
                    Main.spriteBatch.Draw(_swapTarget, (i * MathHelper.PiOver4).ToRotationVector2() * Main.GameViewMatrix.Zoom * 8f, Color.White * 0.15f * pair.Item3);
                }
                for (int i = 0; i < 4; i++) {
                    Main.spriteBatch.Draw(_swapTarget, (i * MathHelper.PiOver2).ToRotationVector2() * Main.GameViewMatrix.Zoom * 2f, Color.White * 0.5f * pair.Item3);
                }
                Main.spriteBatch.Draw(_swapTarget, new Vector2(0f, 0f), Color.White * pair.Item3);

                Main.spriteBatch.End();
                device.SetRenderTarget(_swapTarget);
                device.Clear(Color.Transparent);
            }
        }
        catch {
        }
        RenderingNow = false;

        device.SetRenderTargets(oldTargets);
        HealPairs.Clear();

        //device.SetRenderTarget(helperTarget);
        _wasPrepared = true;
    }

    public void DrawOntoScreen(SpriteBatch spriteBatch) {
        if (!_wasPrepared) {
            return;
        }

        spriteBatch.Draw(_resultTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), CombatText.HealLife);
    }
}