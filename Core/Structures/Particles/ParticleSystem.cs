namespace AequusRemake.Core.Structures.Particles;

[Autoload(Side = ModSide.Client)]
public abstract class ParticleSystem : IParticleSystem {
    protected Mod Mod { get; private set; }

    public abstract int ParticleCount { get; }
    public bool Active { get; protected set; }

    public abstract void Update();

    public abstract void Draw(SpriteBatch spriteBatch);

    public void Activate() {
        if (!Active) {
            Active = true;
            ModContent.GetInstance<ParticleManager>()._activeSystems.AddLast(this);
            OnActivate();
        }
    }
    /// <summary>
    /// Allows the renderer to subscribe to various drawing actions.
    /// This is ran when the particle system is activated.
    /// </summary>
    public virtual void OnActivate() { }

    /// <summary>
    /// Allows the renderer to unsubscribe to various drawing actions.
    /// This is ran when all particles are cleared from the system.
    /// </summary>
    public abstract void Deactivate();

    public virtual void Load(Mod mod) {
        Mod = mod;
    }
    public virtual void Unload() {
        Mod = null;
    }
}