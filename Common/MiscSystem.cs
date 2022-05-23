﻿using Aequus.Items.Accessories.Summon;
using Aequus.Projectiles;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class MiscSystem : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            AequusProjectile.ParentProjectile = -1;
            AequusProjectile.ParentNPC = -1;
            if (AequusHelpers.Main_invasionSize.IsCaching)
            {
                AequusHelpers.Main_invasionSize.EndCaching();
            }
            if (AequusHelpers.Main_invasionType.IsCaching)
            {
                AequusHelpers.Main_invasionType.EndCaching();
            }
            if (AequusHelpers.Main_bloodMoon.IsCaching)
            {
                AequusHelpers.Main_bloodMoon.EndCaching();
            }
            if (AequusHelpers.Main_eclipse.IsCaching)
            {
                AequusHelpers.Main_eclipse.EndCaching();
            }
            if (AequusHelpers.Main_dayTime.IsCaching)
            {
                AequusHelpers.Main_dayTime.EndCaching();
            }
        }
    }
}