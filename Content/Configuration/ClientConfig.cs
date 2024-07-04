using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AequusRemake.Content.Configuration;

public class ClientConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public static ClientConfig Instance;

    [DefaultValue(1f)]
    [Range(0f, 1f)]
    public float FlashIntensity { get; set; }

    [Increment(4)]
    [DefaultValue(40)]
    [Range(10, 80)]
    [Slider]
    public int FlashLoops { get; set; }

    [DefaultValue(true)]
    public bool ShowDeathTips { get; set; }

    [DefaultValue(true)]
    public bool ShowNecromancyOutlines { get; set; }
}