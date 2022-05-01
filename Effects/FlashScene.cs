using Aequus.Common.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Effects
{
    internal class FlashScene : ModSceneEffect // ngl I don't really trust this ModSceneEffect class with SpecialVisuals it seems like if you have a scene which modifies everything it just skips over the rest of the passed scenes which is stink !!!! !!! ! 
    {
        public const string FlashFilterName = "Aequus:Flash";

        public static Asset<Effect> FlashShader { get; private set; }

        public static Filter FlashFilter { get => Filters.Scene[FlashFilterName]; set => Filters.Scene[FlashFilterName] = value; }

        public static ScreenFlashData Flash { get; private set; }

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return true;
        }

        public override void Load()
        {
            FlashShader = ModContent.Request<Effect>("Aequus/Assets/Effects/Flash", AssetRequestMode.ImmediateLoad);
            FlashFilter = new Filter(new ScreenShaderData(new Ref<Effect>(FlashShader.Value), "FlashCoordinatePass"), EffectPriority.VeryHigh);
            Flash = new ScreenFlashData();
        }

        public override void Unload()
        {
            Flash = null;
        }

        public override void SpecialVisuals(Player player)
        {
            if (Flash.FlashLocation != Vector2.Zero)
            {
                FlashFilter.GetShader().UseIntensity(Math.Max(Flash.Intensity * ClientConfig.Instance.FlashIntensity, 1f / 18f));
                if (!FlashFilter.IsActive())
                {
                    Filters.Scene.Activate(FlashFilterName, Flash.FlashLocation, null).GetShader()
                    .UseOpacity(1f).UseTargetPosition(Flash.FlashLocation);
                }
                Flash.Intensity *= Flash.MultiplyPerTick;
                if (Flash.Intensity <= 0.01f)
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