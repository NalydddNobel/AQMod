namespace AQMod.Common.NetCode
{
    public static class AQPacketID
    {
        /// <summary>
        /// Tells the server that someone wants to summon Omega Starite with the Ultimate Sword.
        /// </summary>
        public const byte SummonOmegaStarite = 0;
        /// <summary>
        /// Tells the server that someone wants to summon Omega Starite with the Ultimate Sword.
        /// </summary>
        public const byte UpdateGlimmerEvent = 1;
        /// <summary>
        /// Tells the server that this player's encore kills needs to be updated
        /// </summary>
        public const byte UpdateAQPlayerEncoreKills = 2;
        /// <summary>
        /// Tells the server that the celeste torus needs to be updated
        /// </summary>
        public const byte UpdateAQPlayerCelesteTorus = 3;
    }
}