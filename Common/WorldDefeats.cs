using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    /// <summary>
    /// Carries all of the downed flags for Bosses and Events.
    /// </summary>
    public class WorldDefeats : ModWorld
    {
        public static bool DownedGlimmer { get; set; }
        public static bool DownedStarite { get; set; }
        public static bool DownedYinYang { get; set; }
        public static bool DownedCrabson { get; set; }
        public static bool DownedDemonSiege { get; set; }

        public override void Initialize()
        {
            DownedGlimmer = false;
            DownedStarite = false;
            DownedYinYang = false;
            DownedCrabson = false;
            DownedDemonSiege = false;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["DownedGlimmer"] = DownedGlimmer,
                ["DownedStarite"] = DownedStarite,
                ["DownedYinYang"] = DownedYinYang,
                ["DownedCrabson"] = DownedCrabson,
                ["DownedDemonSiege"] = DownedDemonSiege,
            };
        }

        public override void Load(TagCompound tag)
        {
            DownedGlimmer = tag.GetBool("DownedGlimmer");
            DownedStarite = tag.GetBool("DownedStarite");
            DownedYinYang = tag.GetBool("DownedYinYang");
            DownedCrabson = tag.GetBool("DownedCrabson");
            DownedDemonSiege = tag.GetBool("DownedDemonSiege");
        }
    }
}