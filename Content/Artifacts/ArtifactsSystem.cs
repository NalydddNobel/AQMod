using Terraria.ModLoader;

namespace Aequus.Content.Artifacts
{
    public sealed class ArtifactsSystem : ModSystem
    {
        public static bool CommandGameMode;

        public override void PostUpdateEverything()
        {
            CommandGameMode = false;
        }
    }
}