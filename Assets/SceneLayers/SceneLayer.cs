namespace AQMod.Assets.SceneLayers
{
    public abstract class SceneLayer
    {
        public abstract void Draw();

        public virtual void Update()
        {
        }

        public virtual void Reset()
        {
        }
    }
}