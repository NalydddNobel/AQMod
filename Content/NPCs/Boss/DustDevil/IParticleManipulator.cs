using Microsoft.Xna.Framework;

namespace Aequus.Content.NPCs.Boss.DustDevil {
    public interface IParticleManipulator {
        Vector3 Position { get; set; }
        float InteractionRange { get; }
        void InteractWithParticle(DustParticle p);
        bool IsActive();
    }
}