using AQMod.Common.CrossMod.BossChecklist;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents
{
    public abstract class WorldEvent : ModWorld
    {
        internal virtual EventEntry? BossChecklistEntry()
        {
            return null;
        }
    }
}