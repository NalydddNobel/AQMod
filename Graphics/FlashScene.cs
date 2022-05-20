using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed class FlashScene : ModSceneEffect
    {
        public const string FlashFilterName = "Aequus:Flash";

        public static Asset<Effect> FlashShader { get; private set; }
        public static bool UsingOldFlashShader { get; private set; }

        public static Filter FlashFilter { get => Filters.Scene[FlashFilterName]; set => Filters.Scene[FlashFilterName] = value; }

        public static ScreenFlash Flash { get; private set; }

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return true;
        }

        public override void Load()
        {
            LoadFlashShader();
            FlashFilter = new Filter(new ScreenShaderData(new Ref<Effect>(FlashShader.Value), "FlashCoordinatePass"), EffectPriority.VeryHigh);
            Flash = new ScreenFlash();
        }
        private void LoadFlashShader()
        {
            if (ClientConfig.Instance.HighQualityShaders)
            {
                UsingOldFlashShader = false;
                try
                {
                    FlashShader = ModContent.Request<Effect>("Aequus/Assets/Effects/Flash", AssetRequestMode.ImmediateLoad);
                    return;
                }
                catch
                {
                    Aequus.Instance.Logger.Error("Couldn't load HiDef ps_3_0 flash shader... Loading old instead");
                }
            }
            UsingOldFlashShader = true;
            FlashShader = ModContent.Request<Effect>("Aequus/Assets/Effects/Flash_Old", AssetRequestMode.ImmediateLoad);
        }

        public override void Unload()
        {
            Flash = null;
        }

        public override void SpecialVisuals(Player player)
        {
            if (Flash.FlashLocation != Vector2.Zero)
            {
                try
                {
                    FlashFilter.GetShader().UseTargetPosition(Flash.FlashLocation);
                    FlashFilter.GetShader().UseIntensity(Math.Max(Flash.Intensity * ClientConfig.Instance.FlashIntensity, 1f / 18f));
                    if (!FlashFilter.IsActive())
                    {
                        Main.NewText(4);
                        Filters.Scene.Activate(FlashFilterName, Flash.FlashLocation).GetShader()
                        .UseOpacity(1f).UseTargetPosition(Flash.FlashLocation);
                    }
                    float intensity = Math.Max(Flash.Intensity - Flash.Intensity * Flash.Multiplier, 0.05f);
                    Flash.Intensity -= intensity;
                    if (Flash.Intensity <= 0.00001f)
                    {
                        Flash.Clear();
                    }

                    if (!UsingOldFlashShader)
                        FlashFilter.GetShader().Shader.Parameters["Repetitions"].SetValue(ClientConfig.Instance.FlashShaderRepetitions);
                }
                catch
                {
                    Flash.Clear();
                }
            }
            else
            {
                if (FlashFilter.IsActive())
                {
                    FlashFilter.GetShader()
                        .UseIntensity(0f)
                        .UseProgress(0f)
                        .UseOpacity(0f);
                    Filters.Scene.Deactivate(FlashFilterName, null);
                }
            }
        }
    }
}