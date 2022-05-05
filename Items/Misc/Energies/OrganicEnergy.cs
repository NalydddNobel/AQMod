﻿using Aequus.Common;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class OrganicEnergy : EnergyItemBase
    {
        protected override IColorGradient Gradient => Gradients.organicGrad;
        protected override Vector3 LightColor => new Vector3(0.3f, 0.75f, 0.2f);
        public override int Rarity => ItemRarityID.Green;
    }
}