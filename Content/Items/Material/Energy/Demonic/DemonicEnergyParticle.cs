using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Aequus.Content.Items.Material.Energy.Demonic;
public class DemonicEnergyParticle : EnergyParticle<DemonicEnergyParticle> {
    public override Asset<Texture2D> Texture => AequusTextures.DemonicEnergyParticle;
}