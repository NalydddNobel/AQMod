using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.World
{
    public sealed class PassingDays : ModWorld
    {
        public static int daysPassed;
        public static int daysPassedSinceLastGlimmerEvent;

        public override void Initialize()
        {
            daysPassed = 0;
            daysPassedSinceLastGlimmerEvent = 0;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["DaysPassed"] = daysPassed,
                ["DaysPassedSinceLastGlimmerEvent"] = daysPassedSinceLastGlimmerEvent,
            };
        }

        public override void Load(TagCompound tag)
        {
            daysPassed = tag.GetInt("DaysPassed");
            daysPassedSinceLastGlimmerEvent = tag.GetInt("DaysPassedSinceLastGlimmerEvent");
        }

        public override void PostUpdate()
        {
            if (Main.dayTime && Main.time == 0.0)
            {
                daysPassed++;
                daysPassedSinceLastGlimmerEvent++;
            }
        }
    }
}