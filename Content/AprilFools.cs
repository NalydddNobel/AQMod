using System;
using Terraria;
using Terraria.ID;

namespace AQMod.Content
{
    public static class AprilFools
    {
        public static bool Active { get; internal set; }

        public static void UpdateActive()
        {
            Active = false;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                return;
            }
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                Active = true;
            }
        }
    }
}