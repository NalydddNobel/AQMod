using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Events.GlimmerEvent.Peaceful;
using Aequus.Content.MainMenu;
using Aequus.Tiles.Monoliths.CosmicMonolith;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Chroma;

public class GlimmerShader : ChromaShader {
    public class GlimmerCondition : ChromaCondition {
        public override bool IsActive() {
            if (Main.gameMenu) {
                return MenuLoader.CurrentMenu is GlimmerMenu;
            }

            return Main.LocalPlayer.InModBiome<GlimmerZone>() || Main.LocalPlayer.InModBiome<PeacefulGlimmerZone>() || CosmicMonolithScene.MonolithNearby || Main.LocalPlayer.Aequus().cosmicMonolithShader;
        }
    }

    private record GlimmerSparkle(Vector2 Location) {
        public float Intensity = 1f;
        public Vector2 FragmentPosition { get; private set; }

        public void UpdateFragmentPosition(Fragment fragment) {
            FragmentPosition = Location * fragment.CanvasSize + fragment.CanvasTopLeft + (Intensity * 8f).ToRotationVector2() * 0.05f;
        }
    }

    private float sparkleTime;
    private readonly List<GlimmerSparkle> Sparkles = new();

    public override void Update(float elapsedTime) {
        sparkleTime += elapsedTime * Main.rand.NextFloat(5f);
        for (int i = 0; i < Sparkles.Count; i++) {
            Sparkles[i].Intensity -= Main.rand.NextFloat(0.005f);
            if (Sparkles[i].Intensity <= 0f) {
                Sparkles.RemoveAt(i);
                i--;
                continue;
            }
        }
        if (sparkleTime > 1f) {
            Sparkles.Add(new(new(Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f))));
            sparkleTime = 0f;
        }
    }

    protected void ProcessSkyTexture(Fragment fragment, Vector2 position, int i) {
        GlimmerSparkle glimmerSparkle = null;
        float minimumDistance = float.MaxValue;
        foreach (var sparkle in Sparkles) {
            float distance = Vector2.Distance(position, sparkle.FragmentPosition);
            if (distance < minimumDistance) {
                minimumDistance = distance;
                glimmerSparkle = sparkle;
            }
        }
        Vector4 color = Vector4.Lerp(new Vector4(0f, 0f, 1f, 1f), new Vector4(0.05f, 0f, 0.2f, 1f), (float)Math.Sin(position.Y / fragment.CanvasSize.Y * MathHelper.PiOver2 + MathHelper.PiOver2) * 0.5f + 0.5f);
        if (glimmerSparkle != null) {
            float sparkleLightDistance = MathF.Sin(glimmerSparkle.Intensity * MathHelper.Pi) * 0.15f;
            if (minimumDistance < sparkleLightDistance) {
                color = Vector4.Lerp(new(0.8f, 1f, 1f, 1f), color, minimumDistance / sparkleLightDistance);
            }
        }
        fragment.SetColor(i, color);
    }

    public override void Process(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time) {
        foreach (var sparkle in Sparkles) {
            sparkle.UpdateFragmentPosition(fragment);
        }
        base.Process(device, fragment, quality, time);
    }

    [RgbProcessor(new EffectDetailLevel[] { EffectDetailLevel.Low })]
    private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time) {
        for (int i = 0; i < fragment.Count; i++) {
            var position = fragment.GetCanvasPositionOfIndex(i);
            ProcessSkyTexture(fragment, position, i);
        }
    }

    [RgbProcessor(new EffectDetailLevel[] { EffectDetailLevel.High })]
    private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time) {
        for (int i = 0; i < fragment.Count; i++) {
            var position = fragment.GetCanvasPositionOfIndex(i);
            ProcessSkyTexture(fragment, position, i);
        }
    }
}