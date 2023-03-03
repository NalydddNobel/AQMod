using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;

namespace Aequus.Content.Boss.OmegaStarite
{
    public class OmegaStariteRing : ICloneable
    {
        public const int SEGMENTS_1 = 5;
        public const int SEGMENTS_2 = 8;
        public const int SEGMENTS_3 = 13;

        public const float SCALE_1 = 1f;
        public const float SCALE_2 = 1.1f;
        public const float SCALE_2_EXPERT = 1.2f;

        public const float RING_3_SCALE = 1.45f;

        public const float DIAMETERMULT_2 = 1.5f;
        public const float DIAMETERMULT_2_EXPERT = 1.75f;
        public const float DIAMETERMULT_3 = 2.5f;

        public readonly byte OrbCount;
        public readonly float rotationOrbLoop;
        public readonly Vector3[] CachedPositions;
        public readonly Rectangle[] CachedHitboxes;

        public float pitch;
        public float roll;
        public float yaw;
        public float radiusFromOrigin;

        public float OriginalRadiusFromOrigin { get; private set; }
        public float Scale { get; private set; }

        public Vector3 rotationVelocity;

        public OmegaStariteRing(int amount, float radiusFromOrigin, float scale)
        {
            OrbCount = (byte)amount;
            rotationOrbLoop = MathHelper.TwoPi / OrbCount;
            OriginalRadiusFromOrigin = radiusFromOrigin;
            this.radiusFromOrigin = OriginalRadiusFromOrigin;
            Scale = scale;
            CachedPositions = new Vector3[OrbCount];
            CachedHitboxes = new Rectangle[OrbCount];
        }

        /// <summary>
        /// Creates a Ring through a net package
        /// </summary>
        /// <param name="reader"></param>
        public OmegaStariteRing(BinaryReader reader)
        {
            OrbCount = reader.ReadByte();
            rotationOrbLoop = MathHelper.TwoPi / OrbCount;
            OriginalRadiusFromOrigin = reader.ReadSingle();
            radiusFromOrigin = OriginalRadiusFromOrigin;
            Scale = reader.ReadSingle();
            CachedPositions = new Vector3[OrbCount];
            CachedHitboxes = new Rectangle[OrbCount];
        }

        public static OmegaStariteRing[] FromNetPackage(BinaryReader reader)
        {
            byte amount = reader.ReadByte();
            var rings = new OmegaStariteRing[amount];
            for (byte i = 0; i < amount; i++)
            {
                rings[i] = new OmegaStariteRing(reader);
            }
            return rings;
        }

        public void Update(Vector2 origin)
        {
            pitch += rotationVelocity.X;
            roll += rotationVelocity.Y;
            yaw = (yaw + rotationVelocity.Z) % rotationOrbLoop;
            int i = 0;
            for (float r = 0f; i < OrbCount; r += rotationOrbLoop)
            {
                CachedPositions[i] = Vector3.Transform(new Vector3(radiusFromOrigin, 0f, 0f), Matrix.CreateFromYawPitchRoll(pitch, roll, r + yaw)) + new Vector3(origin, 0f);
                CachedHitboxes[i] = Utils.CenteredRectangle(new Vector2(CachedPositions[i].X, CachedPositions[i].Y), new Vector2(50f, 50f) * Scale);
                i++;
            }
        }

        public void MultScale(float scale)
        {
            OriginalRadiusFromOrigin *= scale;
            radiusFromOrigin *= scale;
            Scale *= scale;
        }

        public void SendNetPackage(BinaryWriter writer)
        {
            writer.Write(pitch);
            writer.Write(roll);
            writer.Write(yaw);
        }

        public void RecieveNetPackage(BinaryReader reader)
        {
            pitch = reader.ReadSingle();
            roll = reader.ReadSingle();
            yaw = reader.ReadSingle();
        }

        public object Clone()
        {
            return new OmegaStariteRing(OrbCount, OriginalRadiusFromOrigin, Scale) { pitch = pitch, roll = roll, yaw = yaw, radiusFromOrigin = radiusFromOrigin, };
        }
    }
}