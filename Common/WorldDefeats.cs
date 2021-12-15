using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    /// <summary>
    /// Carries all of the downed flags for Bosses and Events.
    /// </summary>
    public class WorldDefeats : ModWorld
    {
        public static bool DownedStarite;
        public static bool DownedCrabson;
        public static bool DownedCurrents;

        public static bool DownedRedSprite;

        public static bool DownedGlimmer;
        public static bool DownedCrabSeason;
        public static bool DownedDemonSiege;
        public static bool DownedGaleStreams;

        public static bool NoHitOmegaStarite { get; set; }

        public static bool ObtainedUltimateSword { get; set; }
        public static bool ObtainedMothmanMask { get; set; }
        public static bool ObtainedCatalystPainting { get; set; }

        public static bool HunterIntroduction { get; set; }
        public static bool PhysicistIntroduction { get; set; }

        public override void Initialize()
        {
            DownedGlimmer = false;
            DownedStarite = false;
            DownedCrabson = false;
            DownedDemonSiege = false;
            DownedCrabSeason = false;
            DownedGaleStreams = false;
            DownedCurrents = false;
            DownedRedSprite = false;

            NoHitOmegaStarite = false;

            ObtainedUltimateSword = false;
            ObtainedCatalystPainting = false;
            ObtainedMothmanMask = false;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["DownedGlimmer"] = DownedGlimmer,
                ["DownedStarite"] = DownedStarite,
                ["DownedCrabson"] = DownedCrabson,
                ["DownedDemonSiege"] = DownedDemonSiege,
                ["DownedCrabSeason"] = DownedCrabSeason,
                ["DownedGaleStreams"] = DownedGaleStreams,
                ["DownedCurrents"] = DownedCurrents,
                ["DownedRedSprite"] = DownedRedSprite,

                ["NoHitOmegaStarite"] = NoHitOmegaStarite,

                ["ObtainedUltimateSword"] = ObtainedUltimateSword,
                ["ObtainedMothmanMask"] = ObtainedMothmanMask,
                ["ObtainedCatalystPainting"] = ObtainedCatalystPainting,

                ["HunterIntroduction"] = HunterIntroduction,
                ["PhysicistIntroduction"] = PhysicistIntroduction,
            };
        }

        public override void Load(TagCompound tag)
        {
            DownedGlimmer = tag.GetBool("DownedGlimmer");
            DownedStarite = tag.GetBool("DownedStarite");
            DownedCrabson = tag.GetBool("DownedCrabson");
            DownedDemonSiege = tag.GetBool("DownedDemonSiege");
            DownedCrabSeason = tag.GetBool("DownedCrabSeason");
            DownedGaleStreams = tag.GetBool("DownedGaleStreams");
            DownedCurrents = tag.GetBool("DownedCurrents");
            DownedRedSprite = tag.GetBool("DownedRedSprite");

            NoHitOmegaStarite = tag.GetBool("NoHitOmegaStarite");

            ObtainedUltimateSword = tag.GetBool("ObtainedUltimateSword");
            ObtainedMothmanMask = tag.GetBool("ObtainedMothmanMask");
            ObtainedCatalystPainting = tag.GetBool("ObtainedCatalystPainting");

            HunterIntroduction = tag.GetBool("HunterIntroduction");
            PhysicistIntroduction = tag.GetBool("PhysicistIntroduction");
        }
    }
}