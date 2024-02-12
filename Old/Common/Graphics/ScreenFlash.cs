using Aequus.Content.Configuration;
using ReLogic.Content;
using System;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequus.Old.Common.Graphics;

public class ScreenFlash : ModSceneEffect {
    public const string FlashFilterName = "Aequus:Flash";

    public static Asset<Effect> FlashShader { get; private set; }
    public static bool UsingOldFlashShader { get; private set; }

    public static Filter FlashFilter { get => Filters.Scene[FlashFilterName]; set => Filters.Scene[FlashFilterName] = value; }

    public static ScreenFlashData Instance { get; private set; }

    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override bool IsSceneEffectActive(Player player) {
        return true;
    }

    public override void Load() {
        LoadFlashShader();
        FlashFilter = new Filter(new ScreenShaderData(new Ref<Effect>(FlashShader.Value), "FlashCoordinatePass"), EffectPriority.VeryHigh);
        Instance = new ScreenFlashData();
    }
    private void LoadFlashShader() {
        UsingOldFlashShader = false;
        try {
            FlashShader = ModContent.Request<Effect>("Aequus/Old/Assets/Shaders/Flash", AssetRequestMode.ImmediateLoad);
            return;
        }
        catch {
            Aequus.Instance.Logger.Error("Couldn't load HiDef ps_3_0 flash shader... Loading old instead");
        }
        UsingOldFlashShader = true;
        FlashShader = ModContent.Request<Effect>("Aequus/Old/Assets/Shaders/Flash_Old", AssetRequestMode.ImmediateLoad);
    }

    public override void Unload() {
        Instance = null;
    }

    public override void SpecialVisuals(Player player, bool isActive) {
        if (Instance.FlashLocation != Vector2.Zero) {
            try {
                FlashFilter.GetShader().UseTargetPosition(Instance.FlashLocation);
                FlashFilter.GetShader().UseIntensity(Math.Max(Instance.Intensity * ClientConfig.Instance.FlashIntensity, 0f));
                if (!FlashFilter.IsActive()) {
                    Filters.Scene.Activate(FlashFilterName, Instance.FlashLocation).GetShader()
                    .UseOpacity(1f).UseTargetPosition(Instance.FlashLocation);
                }
                float intensity = Math.Max(Instance.Intensity - Instance.Intensity * Instance.Multiplier, 0.01f);
                Instance.Intensity -= intensity;
                if (Instance.Intensity <= 0f) {
                    Instance.Clear();
                }

                if (!UsingOldFlashShader) {
                    FlashFilter.GetShader().Shader.Parameters["Repetitions"].SetValue(ClientConfig.Instance.FlashLoops);
                }
            }
            catch {
                Instance.Clear();
            }
        }
        else {
            if (FlashFilter.IsActive()) {
                FlashFilter.GetShader()
                    .UseIntensity(0f)
                    .UseProgress(0f)
                    .UseOpacity(0f);
                Filters.Scene.Deactivate(FlashFilterName, null);
            }
        }
    }
}
public class ScreenFlashData {
    public Vector2 FlashLocation;
    public float Intensity;
    public float Multiplier;

    public void Set(Vector2 location, float brightness, float multiplier = 0.9f) {
        FlashLocation = location;
        Intensity = brightness;
        Multiplier = multiplier;
    }

    public void Clear() {
        FlashLocation = default(Vector2);
        Intensity = 0f;
        Multiplier = 0f;
    }
}