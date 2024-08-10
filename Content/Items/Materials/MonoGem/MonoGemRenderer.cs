using Aequus.Common.Rendering;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Materials.MonoGem;

// TODO -- Rework mono gem rendering.
public class MonoGemRenderer : ScreenTarget {
    private class MonoGemScreenShaderData : ScreenShaderData {
        public MonoGemScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName) {
        }

        public override void Apply() {
            Main.instance.GraphicsDevice.Textures[1] = Instance.GetTarget();
            base.Apply();
        }
    }

    public static MonoGemRenderer Instance { get; private set; }
    public readonly List<DrawData> DrawData = new();

    public const string ScreenShaderKey = "Aequus:MonoGem";

    public override void Load(Mod mod) {
        base.Load(mod);
        Instance = this;
        Filters.Scene[ScreenShaderKey] = new Filter(new MonoGemScreenShaderData(
            new Ref<Effect>(
                ModContent.Request<Effect>($"{this.NamespacePath()}/MonoGemScreenShader",
                AssetRequestMode.ImmediateLoad).Value),
            "GrayscaleMaskPass"), EffectPriority.Low);
    }

    public override void Unload() {
        Instance = null;
        base.Unload();
    }

    protected override bool PrepareTarget() {
        return DrawData.Count > 0;
    }

    protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
        Main.spriteBatch.Begin_World(shader: false);

        foreach (var d in DrawData)
            (d with { position = d.position - Main.screenPosition }).Draw(spriteBatch);

        DrawData.Clear();
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
        Helper.ShaderColorOnly.Apply(null);

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

        spriteBatch.End();

        _wasPrepared = true;
    }

    public static void HandleScreenRender() {
        if (!Lighting.NotRetro) {
            if (Instance.IsReady)
                Instance.DrawOntoScreen(Main.spriteBatch);
        }
        else if (Instance.IsReady) {
            Filters.Scene.Activate(ScreenShaderKey, Main.LocalPlayer.Center);
            Filters.Scene[ScreenShaderKey].GetShader().UseOpacity(1f);
        }
        else {
            Filters.Scene.Deactivate(ScreenShaderKey, Main.LocalPlayer.Center);
            Filters.Scene[ScreenShaderKey].GetShader().UseOpacity(0f);
        }
    }

    public void DrawOntoScreen(SpriteBatch spriteBatch) {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

        var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 128));

        drawData.Draw(spriteBatch);

        spriteBatch.End();
        _wasPrepared = false;
    }
}