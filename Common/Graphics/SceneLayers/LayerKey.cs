namespace AQMod.Common.Graphics.SceneLayers
{
    public struct LayerKey
    {
        public readonly byte Index;
        public readonly string Name;

        public SceneLayering Layering => (SceneLayering)Index;

        public static LayerKey Null => new LayerKey(255, null);
        public bool IsNull => Index == 255 || Name == null;

        private LayerKey(byte index, string name)
        {
            Index = index;
            Name = name;
        }

        internal LayerKey(SceneLayering sceneLayering, string name)
        {
            Index = (byte)sceneLayering;
            Name = name;
        }

        public SceneLayer GetLayer()
        {
            return SceneLayersManager.GetLayer(this);
        }

        public T GetLayer<T>() where T : SceneLayer
        {
            return SceneLayersManager.GetLayer<T>(this);
        }
    }
}