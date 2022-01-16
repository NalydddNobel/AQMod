namespace AQMod.Effects.Particles
{
    public abstract class ParticleType
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