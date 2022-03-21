namespace AQMod.Effects.Trails
{
    public abstract class Trail
    {
        public virtual void OnAdd()
        {
        }
        /// <summary>
        /// Return false to kill this trail
        /// </summary>
        /// <returns></returns>
        public abstract bool Update();
        public abstract void Render();
    }
}