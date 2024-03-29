﻿using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsUtility.Miner {
    /// <summary>
    /// Applied by <see cref="MiningPetSpawner"/>
    /// </summary>
    public class MiningPetBuff : BasePetBuff {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<MiningPet>();
    }
}