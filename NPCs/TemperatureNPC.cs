using Terraria.ModLoader;

namespace Aequus.NPCs
{
    /// <summary>
    /// Carries temperature data to inflict onto the player
    /// </summary>
    public sealed class TemperatureNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public int temperature;
    }
}