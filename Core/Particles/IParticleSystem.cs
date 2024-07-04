namespace AequusRemake.Core.Particles;

public interface IParticleSystem : ILoad {
    void Update();
    void Draw(SpriteBatch spriteBatch);
    void Activate();
    void Deactivate();

    int ParticleCount { get; }
    bool Active { get; }
}
