using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences;
public class ClientConfig : ConfigurationBase {
    public static ClientConfig Instance;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("Graphics")]

    [DefaultBackgroundColor]
    [Range(0.5f, 1f)]
    [DefaultValue(1f)]
    [Increment(0.05f)]
    [SliderColor(120, 40, 255, 255)]
    public float ScreenshakeIntensity { get; set; }

    [DefaultBackgroundColor]
    [Range(0f, 1f)]
    [DefaultValue(1f)]
    [DefaultSliderColor]
    public float FlashIntensity { get; set; }

    [DefaultBackgroundColor]
    [Increment(4)]
    [DefaultValue(40)]
    [Range(10, 80)]
    [Slider]
    [DefaultSliderColor]
    public int FlashShaderRepetitions { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    public bool HighQuality { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    public bool NecromancyOutlines { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    public bool AdamantiteChestMimic { get; set; }

    [Header("General")]

    [DefaultBackgroundColor]
    [DefaultValue(false)]
    public bool InfoDebugLogs { get; set; }

#if CUSTOM_RESOURCE_UI
    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool CustomResourceBars { get; set; }
#endif

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    public bool ShowDeathTips { get; set; }
}