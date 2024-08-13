using Aequus.Common.Drawing;
using Aequus.Common.Effects;
using Terraria.DataStructures;

namespace Aequus.Common.Rendering;
public class RadonMossFogRenderer : ScreenTarget {
    public static RadonMossFogRenderer Instance => ModContent.GetInstance<RadonMossFogRenderer>();
    public static LegacyMiscShaderWrap Shader { get; private set; }
    public DrawLayer Draw = new();
    public bool Active { get; internal set; }

    public override int FinalResultResolutionDiv => 2;

    public override void Load(Mod mod) {
        base.Load(mod);
        if (!Main.dedServ) {
            Shader = new LegacyMiscShaderWrap("Aequus/Assets/Effects/RadonMossShader", "Aequus:RadonMossFog", "RadonShaderPass", loadStatics: true);
        }
    }

    public override void Unload() {
        Shader = null;
    }

    protected override bool PrepareTarget() {
        return Active;
    }

    protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
        //Main.NewText(Tiles.Count);
        //Main.NewText(DrawInfoCache.Count);
        spriteBatch.Begin_World(shader: false);
        Draw.Draw(spriteBatch);
        spriteBatch.End();

        PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv, RenderTargetUsage.PreserveContents);

        spriteBatch.Begin();
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv), Color.White);

        spriteBatch.End();

        _wasPrepared = true;
    }

    public void DrawOntoScreen(SpriteBatch spriteBatch) {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

        var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(255, 255, 255, 255));
        //Shader.ShaderData.Apply(drawData);

        drawData.Draw(spriteBatch);

        spriteBatch.End();
        _wasPrepared = false;
    }
}
