using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Content.Particles
{
    public abstract class MonoParticle : Particle
    {
        public Rectangle frame;
        public Vector2 origin;
        public Vector2 velocity;
        public Vector2 position;
        public float scale;
        public float rotation;

        public override void OnAdd()
        {
            frame = new Rectangle(0, 30 * Main.rand.Next(3), 30, 28);
        }

        public override bool Update()
        {
            dust.velocity *= 0.9f;
            float velo = dust.velocity.Length();
            dust.rotation += velo * 0.0314f;
            dust.scale -= 0.05f - velo / 1000f;
            if (dust.scale <= 0.1f)
                dust.active = false;
            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);
            dust.position += dust.velocity;
            return false;
        }
    }
}