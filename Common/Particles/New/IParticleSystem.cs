namespace Aequus.Common.Particles.New;

public interface IParticleSystem : ILoadable {
    void Update();
    void Draw(SpriteBatch spriteBatch);
    void Activate();
    void Deactivate();

    int ParticleCount { get; }
    bool Active { get; }
}
