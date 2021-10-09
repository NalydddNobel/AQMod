using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Assets.SceneLayers
{
    public abstract class SceneLayer
    {
        private Rectangle _screen;

        protected bool OnScreen(Vector2 position)
        {
            return _screen.Contains((int)position.X, (int)position.Y);
        }

        public void DrawLayer()
        {
            _screen = new Rectangle(-20, -20, Main.screenWidth, Main.screenHeight);
            Draw();
        }
        protected abstract void Draw();

        public virtual void Update()
        {
        }

        /// <summary>
        /// Called when loading a world
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Called when added to the layers dictionary array
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// Called before nulling the layers dictionary array
        /// </summary>
        public virtual void Unload()
        {
        }
    }
}