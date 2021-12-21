using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Default;

namespace AQMod
{
    public static class AprilFoolsJoke
    {
        public static bool Active { get; internal set; }

        public static void UpdateActive()
        {
            Active = false;
            if (Main.netMode != NetmodeID.SinglePlayer)
                return;
            if (AprilFools.CheckAprilFools())
                Active = true;
        }
    }
}