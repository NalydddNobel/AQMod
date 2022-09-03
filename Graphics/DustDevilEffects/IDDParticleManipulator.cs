using Microsoft.Xna.Framework;

namespace Aequus.Graphics.DustDevilEffects
{
    public interface IDDParticleManipulator
    {
        Vector3 Position { get; set; }
        float InteractionRange { get; }
        void InteractWithParticle(DDParticle p);
        bool IsActive();
    }
}