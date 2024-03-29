﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles.Base {
    public abstract class HeldProjBase : ModProjectile {
        public float armRotation;
        protected virtual void SetArmRotation(Player player) {
            if (armRotation > 1.1f) {
                player.bodyFrame.Y = 56;
            }
            else if (armRotation > 0.5f) {
                player.bodyFrame.Y = 56 * 2;
            }
            else if (armRotation < -0.5f) {
                player.bodyFrame.Y = 56 * 4;
            }
            else {
                player.bodyFrame.Y = 56 * 3;
            }
        }
    }
}
