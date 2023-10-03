﻿using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Aequus.Content.Items.Material.Energy.Organic;
public class OrganicEnergyParticle : EnergyParticle<OrganicEnergyParticle> {
    public override Asset<Texture2D> Texture => AequusTextures.OrganicEnergyParticle;
}