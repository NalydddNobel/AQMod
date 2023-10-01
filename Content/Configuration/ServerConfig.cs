using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Content.Configuration;

public class ServerConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public static ServerConfig Instance;

    [DefaultValue(true)]
    public bool MoveTreasureMagnet;
}