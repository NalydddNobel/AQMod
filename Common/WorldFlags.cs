using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    public class WorldFlags : ModSystem
    {
        public static bool CompletedGlimmerEvent;
        public static bool DownedOmegaStarite;

        public static bool ObtainedUltimateSword;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["DownedGlimmerEvent"] = CompletedGlimmerEvent;
            tag["DownedOmegaStarite"] = DownedOmegaStarite;
            tag["ObtainedUltimateSword"] = ObtainedUltimateSword;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            CompletedGlimmerEvent = tag.GetBool("DownedGlimmerEvent");
            DownedOmegaStarite = tag.GetBool("DownedOmegaStarite");
            ObtainedUltimateSword = tag.GetBool("ObtainedUltimateSword");
        }
    }
}