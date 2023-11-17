using Aequus.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class TrashCompactorSystem : ModSystem {
    public class ShakeAnimation {
        public Vector2 Shake;
        public float Time;
    }

    public class ItemSpewAnimation {
        public readonly Vector2 Location;
        public readonly int ItemId;
        public float Time;

        public ItemSpewAnimation(Vector2 location, int itemId) {
            Location = location;
            ItemId = itemId;
        }
    }

    public static List<ItemSpewAnimation> SpewAnimations { get; private set; } = new();
    public static TrimmableDictionary<Point, ShakeAnimation> ShakeAnimations { get; private set; } = new();

    public override void ClearWorld() {
        SpewAnimations.Clear();
        ShakeAnimations.Clear();
    }

    public override void PostDrawTiles() {
        foreach (var anim in ShakeAnimations) {
            if (anim.Value.Time <= 0f) {
                ShakeAnimations.RemoveEnqueue(anim.Key);
                continue;
            }
            anim.Value.Time *= 0.97f;
            anim.Value.Time -= 0.1f;
            anim.Value.Shake = Main.rand.NextVector2Square(-anim.Value.Time, anim.Value.Time);
            anim.Value.Shake.Y = Math.Abs(anim.Value.Shake.Y);
        }
        ShakeAnimations.RemoveAllQueued();

        if (SpewAnimations.Count <= 0) {
            return;
        }

        Main.spriteBatch.Begin_World();
        for (int i = 0; i < SpewAnimations.Count; i++) {
            float progress = SpewAnimations[i].Time / 60f;
            SpewAnimations[i].Time += 1f;
            if (SpewAnimations[i].Time < 0) {
                continue;
            }

            if (SpewAnimations[i].Time > 60) {
                SpewAnimations.RemoveAt(i);
                i--;
                continue;
            }

            var drawLocation = SpewAnimations[i].Location;
            float sudoRNG = (drawLocation.X * 10f + drawLocation.Y) % 100f;
            var rng = new FastRandom((int)sudoRNG);
            rng.NextSeed();

            float opacity = 1f;
            drawLocation += new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * rng.NextFloat(2f, 5f))).RotatedBy(Main.GlobalTimeWrappedHourly * rng.NextFloat(0.05f, 0.1f)) * 10f;
            drawLocation.X += (1f - MathF.Pow(1f - progress, 1.5f)) * rng.NextFloat(-120f, -30f);

            float peakHeight = rng.NextFloat(20f, 60f);
            if (progress < 0.25f) {
                if (progress < 0.1f) {
                    opacity *= progress / 0.1f;
                }
                drawLocation.Y -= MathF.Sin(progress * MathHelper.TwoPi) * peakHeight;
            }
            else {
                float fallProgress = (progress - 0.25f) / 0.75f;
                drawLocation.Y += MathF.Pow(fallProgress, 2f) * 300f - peakHeight;
                opacity *= 1f - MathF.Pow(fallProgress, 5f);
            }

            var itemInstance = ContentSamples.ItemsByType[SpewAnimations[i].ItemId];
            var lightColor = DrawHelper.GetLightColor(drawLocation);

            Main.GetItemDrawFrame(SpewAnimations[i].ItemId, out var texture, out var frame);
            Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(Color.White, lightColor)) * opacity, Main.GlobalTimeWrappedHourly * 6f + sudoRNG * 10f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(itemInstance.color, lightColor)) * opacity, Main.GlobalTimeWrappedHourly * 6f + sudoRNG * 10f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }
        Main.spriteBatch.End();
    }
}