using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Common.Particles;

public abstract class ParticleBatch : ModType {
    public abstract void Update();

    public abstract void Draw(SpriteBatch spriteBatch);

    protected sealed override void Register() {
        ParticleSystem.Batches.Add(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }
}
