using Aequus.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Material.MonoGem;

public class MonoGemRenderer : ScreenRenderer {
    private class MonoGemScreenShaderData : ScreenShaderData {
        public MonoGemScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName) {
        }

        public override void Apply() {
            Main.instance.GraphicsDevice.Textures[1] = Instance.GetTarget();
            base.Apply();
        }
    }

    public static MonoGemRenderer Instance { get; private set; }
    public readonly ParticleRenderer Particles = new();
    public readonly List<DrawData> DrawData = new();

    public const string ScreenShaderKey = "Aequus:MonoGem";

    public override void Load(Mod mod) {
        base.Load(mod);
        Instance = this;
        if (!Main.dedServ) {
            Filters.Scene[ScreenShaderKey] = new Filter(new MonoGemScreenShaderData(
                new Ref<Effect>(
                    ModContent.Request<Effect>($"{this.NamespaceFilePath()}/MonoGemScreenShader",
                    AssetRequestMode.ImmediateLoad).Value),
                "GrayscaleMaskPass"), EffectPriority.Low);
        }
    }

    public override void Unload() {
        Instance = null;
        base.Unload();
    }

    protected override bool PrepareTarget() {
        return Particles.Particles.Count > 0 || DrawData.Count > 0;
    }

    protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
        Main.spriteBatch.BeginWorld(shader: false);

        Particles.Draw(spriteBatch);
        foreach (var d in DrawData) {
            (d with { position = d.position - Main.screenPosition }).Draw(spriteBatch);
        }

        DrawData.Clear();
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
        DrawData dd = new(helperTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        //DrawHelper.ShaderColorOnly.Apply(null);
        GameShaders.Armor.GetShaderFromItemId(ItemID.MirageDye).Apply(null, dd);

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        dd.Draw(spriteBatch);

        spriteBatch.End();

        _wasPrepared = true;
    }

    public static void HandleScreenRender() {
        if (!Lighting.NotRetro) {
            if (Instance.IsReady) {
                Instance.DrawOntoScreen(Main.spriteBatch);
            }
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