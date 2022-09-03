using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Graphics.DustDevilEffects
{
    public class DustDevilTornadoManipulator : IDDParticleManipulator
    {
        public Vector3 Position { get; set; }

        public float InteractionRange => range;

        public NPC DustDevil;
        public int timeLeft;
        public float pull;
        public float range;

        public void InteractWithParticle(DDParticle p)
        {
            var diff = Position - p.Position;
            float m = (float)Math.Pow(1f - diff.Length() / 2000f, 2f);
            if (p.frame.X == 0)
            {
                if (diff.Y < 0f)
                {
                    diff.X *= 8f;
                }
                var v = Vector3.Normalize(diff) * (pull * m);
                v.Y *= (float)Math.Pow(m, 2f);
                p.Velocity += v;
                p.Velocity.Y -= 0.4f * m * pull;
                p.Velocity.X += (float)Math.Sin(p.Identifier + Main.GlobalTimeWrappedHourly * 10f) * m * pull * 2f;
            }
            else
            {
                var v = Vector3.Transform(diff, Matrix.CreateFromYawPitchRoll((float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f + p.Identifier * 1.241f), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f + p.Identifier * 2.141f), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + p.Identifier)));

                p.Velocity = Vector3.Normalize(Vector3.Lerp(p.Velocity, v, 0.05f)) * 20f;
            }
            p.Velocity += new Vector3(DustDevil.velocity * 0.05f, 0f) * m;
            if (p.Scale < 1f && Main.rand.NextBool(DDParticleSystem.Particles.Count / 100))
                p.Scale += 0.1f * m;
        }

        public bool IsActive()
        {
            timeLeft--;
            return timeLeft > 0;
        }
    }
}