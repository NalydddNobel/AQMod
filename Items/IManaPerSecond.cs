namespace AQMod.Items
{
    public interface IManaPerSecond
    {
        /// <summary>
        /// Return null for the tooltip to find the mana per second itself
        /// </summary>
        /// <returns></returns>
        int? ManaPerSecond { get; }
    }
}