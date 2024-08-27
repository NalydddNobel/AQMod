﻿using Aequus.Common.Buffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs {
    public class BloodthirstBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            LegacyPotionColorsDatabase.BuffToColor.Add(Type, new Color(255, 61, 110));
            AequusBuff.AddPotionConflict(Type, BuffID.Heartreach);
        }
    }
}