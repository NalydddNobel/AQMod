namespace AQMod.Common.Graphics.SceneLayers
{
    /// <summary>
    /// An enumerator used to tell the <see cref="SceneLayersManager"/> when to run drawcode so it all layers properly.
    /// </summary>
    public enum SceneLayering : byte
    {
        BehindTiles_BehindNPCs = 0,
        BehindTiles_InfrontNPCs = 1,
        BehindNPCs = 2,
        InfrontNPCs = 3,
        PreDrawPlayers = 4,
        PostDrawPlayers = 5,
        Count
    }
}