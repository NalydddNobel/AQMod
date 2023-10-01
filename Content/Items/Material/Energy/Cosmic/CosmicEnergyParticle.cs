using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Aequus.Content.Items.Material.Energy.Cosmic;
public class CosmicEnergyParticle : EnergyParticle<CosmicEnergyParticle> {
    public override Asset<Texture2D> Texture => AequusTextures.CosmicEnergyParticle;
}