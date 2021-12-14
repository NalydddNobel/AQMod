using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public class CosmicanonCounts : ModWorld
    {
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