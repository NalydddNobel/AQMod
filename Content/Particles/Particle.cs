namespace AQMod.Content.Particles
{
    public abstract class Particle
    {
        public virtual void OnAdd()
        {
        }
        public abstract void Update();
        public abstract void Draw();
    }
}