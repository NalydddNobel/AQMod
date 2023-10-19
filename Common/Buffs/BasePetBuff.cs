﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Buffs {
    public abstract class BasePetBuff : ModBuff {
        protected virtual bool LightPet => false;
        protected abstract int PetProj { get; }

        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            if (LightPet) {
                Main.lightPet[Type] = true;
            }
            else {
                Main.vanityPet[Type] = true;
            }
        }

        public override void Update(Player player, ref int buffIndex) {
            player.buffTime[buffIndex] = 18000;
            if (player.ownedProjectileCounts[PetProj] <= 0 && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2f, player.position.Y + player.height / 2f, 0f, 0f, PetProj, 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}