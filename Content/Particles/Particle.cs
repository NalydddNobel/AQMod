namespace AQMod.Content.Particles
{
    public abstract class Particle
    {
        public virtual void OnAdd()
        {
        }
        /// <summary>
        /// Return false to kill this particle
        /// </summary>
        /// <returns></returns>
        public abstract bool Update();
        public abstract void Draw();
    }
}