using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Aequus.Content.Items.Material.Energy.Aquatic;
public class AquaticEnergyParticle : EnergyParticle<AquaticEnergyParticle> {
    public override Asset<Texture2D> Texture => AequusTextures.AquaticEnergyParticle;
}