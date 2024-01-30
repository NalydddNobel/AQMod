using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Content.Configuration;

public class ClientConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public static ClientConfig Instance;

    [DefaultValue(true)]
    public System.Boolean ShowDeathTips;
}