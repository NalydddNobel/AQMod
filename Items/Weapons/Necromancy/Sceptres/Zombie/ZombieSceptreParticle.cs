using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    public class ZombieSceptreParticle : ModDust {
        public override void OnSpawn(Dust dust) {
            dust.noGravity = true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) {
            float opacity = dust.Opacity() * dust.scale;
            float powOpacity = MathF.Pow(opacity, 4f);
            dust.rotation += 0.1f;
            return new(powOpacity, powOpacity, opacity, 0);
        }
    }
}