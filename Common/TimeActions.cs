using AQMod.Content.World.Events.GlimmerEvent;
using System.Reflection;
using Terraria;
using Terraria.ID;

namespace AQMod.Common
{
    public static class TimeActions
    {
        public static class Hooks
        {
            internal static MethodInfo Main_UpdateTime_SpawnTownNPCs;

            internal static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
            {
                AQSystem.UpdatingTime = true;
                Main.dayRate += AQSystem.DayrateIncrease;
                if (Main.dayTime)
                {
                    if (Main.time + Main.dayRate > Main.dayLength)
                    {
                        AQSystem.CosmicanonActive = AQPlayer.IgnoreMoons();
                        AprilFoolsJoke.UpdateActive();
                        if (Main.netMode != NetmodeID.Server)
                        {
                            GlimmerEventSky.InitNight();
                        }
                    }
                    orig();
                    AQSystem.CosmicanonActive = false;
                }
                else
                {
                    if (Main.time + Main.dayRate > Main.nightLength)
                    {
                        AQSystem.CosmicanonActive = AQPlayer.IgnoreMoons();
                    }
                    orig();
                    if (WorldDefeats.TownNPCMoveAtNight && !Main.dayTime)
                    {
                        Main_UpdateTime_SpawnTownNPCs.Invoke(null, null);
                    }
                    AQSystem.CosmicanonActive = false;
                }
                AQSystem.DayrateIncrease = 0;
                MessageBroadcast.PreventChat = false;
                MessageBroadcast.PreventChatOnce = false;
                AQSystem.UpdatingTime = false;
            }
        }
    }
}