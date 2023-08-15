using Aequus.Content.Events.DemonSiege;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;
using System;
using Terraria;
using Terraria.GameContent.RGB;

namespace Aequus.Content.Chroma;

public class DemonSiegeShader : ChromaShader {
    public class DemonSiegeCondition : ChromaCondition {
        public override bool IsActive() {
            if (Main.gameMenu) {
                return false;
            }

            return Main.LocalPlayer.InModBiome<DemonSiegeZone>();
        }
    }

    private float _clockProgress;
    private float _clockRotation;

    public override void Update(float elapsedTime) {
        if (Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.eventDemonSiege != Point.Zero && DemonSiegeSystem.ActiveSacrifices.TryGetValue(aequusPlayer.eventDemonSiege, out var sacrifice)) {
            _clockProgress = 1f - MathHelper.Clamp(sacrifice.TimeLeft / (float)sacrifice.TimeLeftMax, 0f, 1f);
        }
        float wantedRotation = MathHelper.Clamp(_clockProgress, 0f, 1f) * MathHelper.TwoPi - MathHelper.Pi;
        float maximumTickDown = elapsedTime * 2f;
        if (_clockRotation < wantedRotation) {
            _clockRotation += maximumTickDown;
            if (_clockRotation > wantedRotation) {
                _clockRotation = wantedRotation;
            }
        }
        if (_clockRotation > wantedRotation) {
            _clockRotation -= maximumTickDown;
            if (_clockRotation < wantedRotation) {
                _clockRotation = wantedRotation;
            }
        }
    }

    private void GetPortalData(Fragment fragment, Vector2 canvasPosition, float shortestEdge, out float distance, out float portalEdgeValue) {
        //distance = Vector2.Distance(canvasPosition / fragment.CanvasSize, new(0.5f, 0.5f));
        //portalEdgeValue = MathF.Pow(distance * Helper.Wave(Main.GlobalTimeWrappedHourly, 1.7f, 1.9f), 2f);
        distance = Vector2.Distance(canvasPosition, fragment.CanvasCenter);
        portalEdgeValue = MathF.Pow(distance / shortestEdge / 1.2f, 2f);
    }

    [RgbProcessor(new EffectDetailLevel[] { EffectDetailLevel.Low, EffectDetailLevel.High })]
    private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time) {
        float shortestEdge = Math.Min(fragment.CanvasSize.X, fragment.CanvasSize.Y);
        float timeHandGradient = 1f;
        float backgroundHandGradient = 0.5f;
        Vector4 portalColor = new(1f, 0f, 0f, 1f);
        Vector4 portalFlameColor = new(1f, 0.8f, 0f, 1f);
        Vector4 portalDarknessColor = new(0f, 0f, 0f, 1f);
        Vector4 backgroundDarknessColor = new(0.1f, 0f, 0.05f, 1f);
        Vector4 backgroundBrightnessColor = new(0.54f, 0.16f, 0.88f, 1f);
        for (int i = 0; i < fragment.Count; i++) {
            var canvasPosition = fragment.GetCanvasPositionOfIndex(i);
            GetPortalData(fragment, canvasPosition, shortestEdge, out float distance, out float portalEdgeValue);
            float clockRotation = (canvasPosition - fragment.CanvasCenter).RotatedBy(-MathHelper.PiOver2).ToRotation();
            if (portalEdgeValue < 1f) {
                if (clockRotation < _clockRotation) {
                    if (clockRotation > _clockRotation - timeHandGradient) {
                        fragment.SetColor(i, Vector4.Lerp(portalColor, portalFlameColor, MathF.Pow(1f - (_clockRotation - clockRotation) / timeHandGradient, 2f)));
                    }
                    else {
                        fragment.SetColor(i, portalColor);
                    }
                }
                else {
                    fragment.SetColor(i, Vector4.Lerp(portalDarknessColor, portalColor, MathF.Pow(portalEdgeValue, Helper.Wave(time, 1.75f, 2.5f))));
                }
            }
            else {
                canvasPosition.Y += (float)Math.Sin(canvasPosition.X * 2f + time * 2f) * 0.2f;
                float staticNoise = NoiseHelper.GetStaticNoise(canvasPosition * new Vector2(0.2f, 0.5f));
                Vector4 color = Vector4.Lerp(backgroundDarknessColor, backgroundBrightnessColor, MathF.Pow(staticNoise, 3f - _clockProgress * 2f));
                if (clockRotation < _clockRotation) {
                    float hueShift = 0.1f;
                    float lightMultiplier = 1.5f;
                    if (clockRotation > _clockRotation - backgroundHandGradient) {
                        float clockGradient = (1f - MathF.Pow(1f - (_clockRotation - clockRotation) / backgroundHandGradient, 2f));
                        hueShift += clockGradient * 0.05f;
                        lightMultiplier += clockGradient * 2f;
                    }
                    color = new Color(color).HueAdd(hueShift).ToVector4() * lightMultiplier;
                }
                else {
                }
                color.X = Math.Min(color.X, 1f);
                color.Y = Math.Min(color.Y, 1f);
                color.Z = Math.Min(color.Z, 1f);
                color.W = Math.Min(color.W, 1f);
                fragment.SetColor(i, color);
            }
        }
    }
}