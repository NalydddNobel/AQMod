using Aequus.Common.Drawing;
using Aequus.Common.Graphics.Shaders;
using Aequus.Common.Particles.New;

namespace Aequus.Content.Tiles.Herbs;

internal class HerbBreakEffectParticle : ParticleArray<HerbBreakEffectParticle.Particle> {
    public override int ParticleCount => 100;

    public override void Draw(SpriteBatch spriteBatch) {
        Texture2D texture = AequusTextures.None;

        spriteBatch.Begin_Dusts(immediate: true);
        var effect = AequusShaders.CircleRings.Value;
        effect.Techniques[0].Passes[0].Apply();
        var endCircle = effect.Parameters["uvEnd"];
        var startCircle = effect.Parameters["uvStart"];
        var circleColor = effect.Parameters["ringColor"];
        for (int i = 0; i < Particles.Length; i++) {
            var p = Particles[i];
            if (p == null || !p.Active) {
                continue;
            }

            Vector2 drawCoordinates = p.Location - Main.screenPosition;
            Color color = Color.White with { A = 100 } * p.Opacity;
            float end = (1f - p.Opacity) * 0.2f;
            //startCircle.SetValue(end - 0.02f);
            startCircle.SetValue(end - 0.1f);
            endCircle.SetValue(end);
            circleColor.SetValue(color.ToVector4());

            spriteBatch.DrawAlign(texture, drawCoordinates, null, color, 0f, 60f, SpriteEffects.None);
        }
        spriteBatch.End();
    }

    public override void Update() {
        for (int i = 0; i < Particles.Length; i++) {
            var p = Particles[i];

            if (p == null || !p.Active) {
                continue;
            }

            p.Opacity -= 0.08f;
            if (p.Opacity <= 0f) {
                p.Active = false;
            }
            else {
                Active = true;
            }
        }
    }

    public override void Activate() {
        Instance<DrawLayers>().PostDrawDust += Draw;
    }

    public override void Deactivate() {
        Instance<DrawLayers>().PostDrawDust -= Draw;
    }

    public class Particle : IParticle {
        public bool Active { get; set; }
        public Vector2 Location;
        public float Opacity;
    }
}
