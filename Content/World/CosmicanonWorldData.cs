using AQMod.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public sealed class CosmicanonWorldData : ModWorld
    {
        public static class Hooks
        {
            internal static void AchievementsHelper_NotifyProgressionEvent(On.Terraria.GameContent.Achievements.AchievementsHelper.orig_NotifyProgressionEvent orig, int eventID)
            {
                if (AQSystem.UpdatingTime && AQSystem.CosmicanonActive && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (eventID == AchievementHelperID.Events.BloodMoonStart)
                    {
                        Main.bloodMoon = false;
                        BloodMoonsPrevented++;
                        if (Main.netMode == NetmodeID.Server)
                            NetHelper.PreventedBloodMoon();
                        MessageBroadcast.PreventChatOnce = true;
                    }
                    if (eventID == AchievementHelperID.Events.EclipseStart)
                    {
                        Main.eclipse = false;
                        EclipsesPrevented++;
                        if (Main.netMode == NetmodeID.Server)
                            NetHelper.PreventedEclipse();
                        MessageBroadcast.PreventChatOnce = true;
                    }
                }
                orig(eventID);
            }
        }

        public static ushort BloodMoonsPrevented { get; set; }
        public static ushort GlimmersPrevented { get; set; }
        public static ushort EclipsesPrevented { get; set; }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["BloodMoonsPrevented"] = (int)BloodMoonsPrevented,
                ["GlimmersPrevented"] = (int)GlimmersPrevented,
                ["EclipsesPrevented"] = (int)EclipsesPrevented,
            };
        }

        public override void Load(TagCompound tag)
        {
            BloodMoonsPrevented = (ushort)tag.GetInt("BloodMoonsPrevented");
            GlimmersPrevented = (ushort)tag.GetInt("GlimmersPrevented");
            EclipsesPrevented = (ushort)tag.GetInt("EclipsesPrevented");
        }
    }
}