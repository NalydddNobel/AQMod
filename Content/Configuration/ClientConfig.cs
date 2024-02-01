using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Content.Configuration;

public class ClientConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public static ClientConfig Instance;

    [DefaultValue(true)]
    public bool ShowDeathTips { get; set; }

    [DefaultValue(true)]
    public bool ShowNecromancyOutlines { get; set; }
}