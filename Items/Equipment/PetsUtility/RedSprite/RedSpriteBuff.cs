﻿using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsUtility.RedSprite {
    /// <summary>
    /// Applied by <see cref="LightningRod"/>
    /// </summary>
    public class RedSpriteBuff : BasePetBuff {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<RedSpritePet>();
    }
}
