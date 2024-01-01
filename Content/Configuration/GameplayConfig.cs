using Aequus.Content.Biomes.PollutedOcean;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Content.Configuration;
internal class GameplayConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public static GameplayConfig Instance;

    [DefaultValue(POGenerationSide.Automatic)]
    public POGenerationSide PollutedOceanSide;
}
