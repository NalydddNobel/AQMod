using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Aequus.Content.Items.Material.Energy.Atmospheric;
public class AtmosphericEnergyParticle : EnergyParticle<AtmosphericEnergyParticle> {
    public override Asset<Texture2D> Texture => AequusTextures.AtmosphericEnergyParticle;
}