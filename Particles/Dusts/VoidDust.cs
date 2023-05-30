using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Particles.Dusts {
    public class VoidDust : MonoDust
    {
        public override float VelocityMultiplier => 0.95f;
        public override float ScaleSubtraction => 0.03f;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
            dust.rotation += dust.velocity.Length() * 0.4f;
            return base.Update(dust);
        }
    }
}