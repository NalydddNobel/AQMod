namespace AQMod.Assets.SceneLayers
{
    /// <summary>
    /// A special scene layer object used to interact with SkyLayer better. Please only add these types of layers to SkyLayer, since they might be unstable in the regular SceneLayersManager
    /// </summary>
    public abstract class BackgroundSceneLayer : SceneLayer
    {
        public bool ProperLayerDepth(float layering)
        {
            return layering > 0f;
        }
    }
}