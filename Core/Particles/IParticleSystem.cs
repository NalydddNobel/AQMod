namespace Aequus.Core.Particles;

public interface IParticleSystem : ILoadable {
    void Update();
    void Draw(SpriteBatch spriteBatch);
    void Activate();
    void Deactivate();

    System.Int32 ParticleCount { get; }
    System.Boolean Active { get; }
}
