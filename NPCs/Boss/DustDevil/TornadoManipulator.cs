using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.NPCs.Boss.DustDevil
{
    public class TornadoManipulator : IParticleManipulator
    {
        public Vector3 Position { get; set; }

        public float InteractionRange => range;

        public NPC DustDevil;
        public int timeLeft;
        public float pull;
        public float range;

        public void InteractWithParticle(DustParticle p)
        {
            var diff = Position - p.Position;
            float m = (float)Math.Pow(1f - diff.Length() / 2000f, 2f) * p.Scale;
            if (p.frame.X == 0)
            {
                diff = Position - new Vector3(0f, 20f, 0f) - p.Position;
                var v = Vector3.Normalize(diff) * (pull * m);

                if (v.Y > 0f)
                {
                    v.X *= 8f;
                }
                if (diff.Y < -50f)
                {
                    m *= 2f;
                    v.Y *= 0.1f;
                    p.Velocity.Y -= 1f * pull * Math.Clamp(1f - Math.Abs(diff.X) * 0.1f * pull, 0f, 1f);
                }

                v.Y *= (float)Math.Pow(m, 2f);
                p.Velocity += v;
                p.Velocity.Y -= 0.5f * m * pull;
                float time = p.Identifier * 0.3f + Main.GlobalTimeWrappedHourly * 6f;
                float waveM = m * pull * 2f;
                p.Velocity.X += (float)Math.Sin(time) * waveM;
                p.Velocity.Z += (float)Math.Cos(time) * waveM;
            }
            else
            {
                var v = Vector3.Transform(diff, Matrix.CreateFromYawPitchRoll((float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f + p.Identifier * 1.241f), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f + p.Identifier * 2.141f), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + p.Identifier)));

                p.Velocity = Vector3.Normalize(Vector3.Lerp(p.Velocity, v * p.Scale, 0.05f)) * 20f * p.Scale;
            }
            p.Velocity += new Vector3(DustDevil.velocity * 0.4f, 0f) * m;
            if (p.Scale < 1.5f && p.Scale > 0.4f && Main.rand.NextBool(DustDevilParticleSystem.Particles.Count / 200 + 6 + p.timeAlive / 30))
                p.Scale += 0.1f * m;
        }

        public bool IsActive()
        {
            timeLeft--;
            return timeLeft > 0;
        }
    }
}