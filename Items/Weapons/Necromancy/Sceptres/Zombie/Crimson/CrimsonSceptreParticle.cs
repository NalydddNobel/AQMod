using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Crimson {
    public class CrimsonSceptreParticle : ModDust {
        public override void OnSpawn(Dust dust) {
            dust.noGravity = true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) {
            float opacity = dust.Opacity() * dust.scale;
            float powOpacity = MathF.Pow(opacity, 4f);
            return new(opacity, powOpacity, powOpacity, 0);
        }

        public override bool MidUpdate(Dust dust) {
            dust.rotation += 0.1f;
            return false;
        }
    }
}