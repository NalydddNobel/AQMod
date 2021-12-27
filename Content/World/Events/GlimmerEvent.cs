using Terraria;

namespace AQMod.Content.World.Events
{
    public static class GlimmerEvent
    {
        public static bool IsActive { get; private set; }
        public static bool DiscoParty { get; internal set; }
        public static int deactivationDelay;

        public static bool ShouldKillStar(NPC npc)
        {
            return Main.dayTime;
        }
    }
}