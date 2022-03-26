using AQMod.Effects.Trails;
using Microsoft.Xna.Framework;
using System;

namespace AQMod.Effects.Prims
{
    public sealed class DyingPrimTrail : Trail
    {
        private readonly PrimRenderer Prims;
        private readonly float UVAdd;
        private readonly float UVMult;
        private readonly int RemoveAmt;

        private Vector2[] Vertices;

        public DyingPrimTrail(PrimRenderer prims, Vector2[] positions, float uvAdd = 0f, float uvMult = 1f, int removeAmt = 1)
        {
            Prims = prims;
            Vertices = PrimRenderer.RemoveZerosAndDoOffset(positions, Vector2.Zero);
            UVAdd = uvAdd;
            UVMult = uvMult;
            RemoveAmt = Math.Max(removeAmt, 1);
        }

        public override bool Update()
        {
            if (Vertices.Length <= Math.Max(RemoveAmt, 2))
            {
                return false;
            }
            Array.Resize(ref Vertices, Vertices.Length - RemoveAmt);
            return true;
        }

        public override void Render()
        {
            Prims.Draw(Vertices, UVAdd, UVMult);
        }
    }
}