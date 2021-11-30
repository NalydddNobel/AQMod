using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Common.Graphics.SceneLayers
{
    public abstract class SceneLayer : IAutoloadType
    {
        private Rectangle _screen;

        public abstract string Name { get; }
        public abstract SceneLayering Layering { get; }

        protected bool OnScreen(Vector2 position)
        {
            return _screen.Contains((int)position.X, (int)position.Y);
        }

        void IAutoloadType.OnLoad()
        {
            if (Main.dedServ)
                return;
            var key = SceneLayersManager.Register(Name, this, Layering);
            if (key.IsNull)
                return;
            OnRegister(key);
        }

        void IAutoloadType.Unload()
        {
            Unload();
        }

        /// <summary>
        /// Called before nulling the layers dictionary array
        /// </summary>
        internal virtual void Unload()
        {
        }

        protected virtual void OnRegister(LayerKey key)
        {
        }
        internal void DrawLayer()
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
        internal virtual void Initialize()
        {
        }
    }
}