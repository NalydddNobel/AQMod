using Aequus.Common.Utilities;
using Aequus.Content.Events.GlimmerEvent;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;
using System;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Content.Chroma;

public class OmegaStariteShader : GlimmerShader {
    public class OmegaStariteCondition : ChromaCondition {
        public override bool IsActive() {
            if (Main.gameMenu) {
                return false;
            }

            return GlimmerZone.omegaStarite > -1;
        }
    }

    private readonly List<Vector4> Orbs = new();
    private float _shaderTime;

    public override void Update(float elapsedTime) {
        base.Update(elapsedTime);

        Orbs.Clear();
        Orbs.Add(new(0.5f, 0.5f, 0f, 0.8f));
        float rotationTime = _shaderTime * 1.6f;
        float orbOutwards = Helper.Wave(_shaderTime, 0.25f, 0.45f);
        for (int i = 0; i < 5; i++) {
            float rotationOffset = i / 5f * MathHelper.TwoPi;
            Orbs.Add(new Vector4((rotationTime + rotationOffset + MathHelper.Pi).ToRotationVector2() * orbOutwards + new Vector2(0.5f, 0.5f), MathF.Sin(rotationTime + rotationOffset), 0.4f));
        }
        //SortedOrbs.Sort((v1, v2) => v1.Z.CompareTo(-v2.Z));
        _shaderTime += elapsedTime;
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
        Vector4 omegaStariteColor = new(0.025f, 0.67f, 1f, 1f);
        for (int i = 0; i < fragment.Count; i++) {
            var position = fragment.GetCanvasPositionOfIndex(i);
            Vector4 orb = new(0f, 0f, float.MaxValue, 1f);
            float orbDistance = float.MaxValue;
            float orbScale = 1f;
            foreach (var sortedOrb in Orbs) {
                if (orb.Z < sortedOrb.Z) {
                    continue;
                }
                float scale = ViewHelper.GetViewScale(1f, sortedOrb.Z * 6f);
                float distance = Vector2.Distance(position, new Vector2(sortedOrb.X, sortedOrb.Y) * fragment.CanvasSize);
                if (distance < scale * sortedOrb.W) {
                    orb = sortedOrb;
                    orbDistance = distance;
                    orbScale = scale;
                }
            }
            ProcessSkyTexture(fragment, position, i);
            fragment.Colors[i] = Helper.HueAdd(new Color(fragment.Colors[i]), 0.05f).ToVector4();

            if (orb.Z == float.MaxValue) {
                continue;
            }

            var skyColor = fragment.Colors[i];
            var orbColor = omegaStariteColor * orbScale;
            float orbOpacity = 1f - MathF.Pow(Math.Min(orbScale * (orbDistance / orbScale / orb.W), 1f), 5f);
            if (orbOpacity <= 1f) {
                orbColor = Vector4.Lerp(Color.Blue.ToVector4(), orbColor, orbOpacity);
            }

            orbColor.X = Math.Min(orbColor.X, 1f);
            orbColor.Y = Math.Min(orbColor.Y, 1f);
            orbColor.Z = Math.Min(orbColor.Z, 1f);
            orbColor.W = Math.Min(orbColor.W, 1f);
            fragment.SetColor(i, orbColor);
        }
    }
}