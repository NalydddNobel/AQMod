namespace AQMod.Common.NetCode
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
        /// Tells the server that this player's encore kills needs to be updated
        /// </summary>
        UpdateAQPlayerEncoreKills = 2,
        /// <summary>
        /// Tells the server that the celeste torus needs to be updated
        /// </summary>
        UpdateAQPlayerCelesteTorus = 3,
        Count
    }
}