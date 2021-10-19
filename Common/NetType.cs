namespace AQMod.Common
{
    public enum NetType : byte
    {
        /// <summary>
        /// Tells the server that someone wants to summon Omega Starite with the Ultimate Sword.
        /// </summary>
        SummonOmegaStarite = 0,
        /// <summary>
        /// Tells the server that someone wants to summon Omega Starite with the Ultimate Sword.
        /// </summary>
        UpdateGlimmerEvent = 1,
        /// <summary>
        /// Tells the server that this player needs to be updated
        /// </summary>
        UpdateAQPlayer = 2,
        /// <summary>
        /// Tells the server that this player's encore kills needs to be updated
        /// </summary>
        UpdateAQPlayerEncoreKills = 3,
        UpdateAQPlayerCelesteTorus = 4,
        Count
    }
}
