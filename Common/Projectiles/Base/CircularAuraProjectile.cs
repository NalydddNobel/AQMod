using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles.Base {
    public abstract class CircularAuraProjectile : ModProjectile {
        public override void SetDefaults() {
            Projectile.DefaultToNoInteractions();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            behindNPCsAndTiles.Add(index);
            overPlayers.Add(index);
            Projectile.hide = false;
        }

        public override bool PreDraw(ref Color lightColor) {
            if (Projectile.hide) {
                DrawAbovePlayers(lightColor);
                return false;
            }

            DrawBehindTiles(lightColor);
            Projectile.hide = true;
            return false;
        }

        public virtual void DrawBehindTiles(Color lightColor) {
        }

        public virtual void DrawAbovePlayers(Color lightColor) {
        }
    }
}