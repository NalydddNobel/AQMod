namespace AQMod.Assets.DrawCode
{
    /// <summary>
    /// An enumerator used to tell the <see cref="SceneLayersManager"/> when to run drawcode so it all layers properly.
    /// </summary>
    public enum SceneLayering : byte
    {
        Sky = 0,
        BehindTiles_BehindNPCs = 1,
        BehindTiles_InfrontNPCs = 2,
        BehindNPCs = 3,
        InfrontNPCs = 4,
        PreDrawPlayers = 5,
        PostDrawPlayers = 6,
        Count
    }
}