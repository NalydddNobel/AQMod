using Aequus.Core;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class TrashCompactorSystem : ModSystem {
    public static List<AnimationItemSpew> SpewAnimations { get; private set; } = new();
    public static TrimmableDictionary<Point, AnimationTrashCompactor> TileAnimations { get; private set; } = new();

    public override void ClearWorld() {
        SpewAnimations.Clear();
        TileAnimations.Clear();
    }

    private static void AnimateTrashCompactor(TrashCompactor modTile, AnimationTrashCompactor anim, Point xy) {
        if (anim.Frame != 0 || anim.FrameTime != 0) {
            modTile.AnimateTile(ref anim.Frame, ref anim.FrameTime);
        }

        if (anim.ShakeTime > 0f) {
            anim.ShakeTime *= 0.97f;
            anim.ShakeTime -= 0.1f;
            anim.Shake = Main.rand.NextVector2Square(-anim.ShakeTime, anim.ShakeTime) * 0.25f;
            anim.Shake.Y = Math.Abs(anim.Shake.Y);
        }
        else {
            anim.Shake = Vector2.Zero;
        }

        var tile = Framing.GetTileSafely(xy);
        if (tile.TileType != ModContent.TileType<TrashCompactor>() || tile.TileFrameX != 0 || tile.TileFrameY != 0) {
            TileAnimations.RemoveEnqueue(xy);
        }
    }

    private static void SpawnItemEffectParticles(AnimationItemSpew anim, Item itemInstance) {
        if (itemInstance.createTile < TileID.Dirt) {
            return;
        }
        for (int l = 0; l < 3; l++) {
            int d = TileHelper.GetTileDust(anim.TileLocation.X, anim.TileLocation.Y, itemInstance.createTile, itemInstance.placeStyle);
            if (d == -1 || d == Main.maxDust) {
                continue;
            }

            Main.dust[d].position = anim.Location + Main.rand.NextVector2Square(-4f, 4f);
            Main.dust[d].fadeIn = Main.dust[d].scale;
            Main.dust[d].scale *= 0.4f;
            Main.dust[d].velocity.X = Main.rand.NextFloat(-2f, -1f);
            Main.dust[d].velocity.Y = Main.rand.NextFloat(-3f, -1f);
        }
    }

    private static bool UpdateItemEffects(AnimationItemSpew anim) {
        anim.AnimationTime += 1f;

        var itemInstance = ContentSamples.ItemsByType[anim.ItemId];
        if (anim.AnimationTime > 0f && !anim.SpawnedParticles) {
            var topLeft = anim.TileOrigin;
            if (!TileAnimations.TryGetValue(topLeft, out var tileAnimation)) {
                tileAnimation = TileAnimations[topLeft] = new();
            }

            tileAnimation.ShakeTime = Math.Min(tileAnimation.ShakeTime + 2f, 4f);
            SpawnItemEffectParticles(anim, itemInstance);
            SoundEngine.PlaySound(SoundID.Item102, anim.Location);
            anim.SpawnedParticles = true;
        }

        return anim.AnimationTime <= 60;
    }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var trashCompactorTile = ModContent.GetInstance<TrashCompactor>();
        foreach (var anim in TileAnimations) {
            AnimateTrashCompactor(trashCompactorTile, anim.Value, anim.Key);
        }

        for (int i = 0; i < SpewAnimations.Count; i++) {
            if (!UpdateItemEffects(SpewAnimations[i])) {
                SpewAnimations.RemoveAt(i);
                i--;
            }
        }

        TileAnimations.RemoveAllQueued();
    }

    private static void GetItemDrawValues(float progress, FastRandom rng, ref Vector2 drawLocation, out Color lightColor, out float opacity, out float rotation, out float scale) {
        opacity = 1f;
        drawLocation += new Vector2(MathF.Sin(progress * rng.NextFloat(2f, 5f))).RotatedBy(progress * rng.NextFloat(0.05f, 0.1f)) * 10f;
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

        lightColor = DrawHelper.GetLightColor(drawLocation);
        scale = rng.NextFloat(0.6f, 0.8f);
        rotation = progress * 6f + rng.NextFloat(10f);
    }

    private static void DrawItemEffect(AnimationItemSpew animation) {
        float progress = animation.AnimationTime / 60f;
        var itemInstance = ContentSamples.ItemsByType[animation.ItemId];
        var drawLocation = animation.Location;
        float sudoRNG = (drawLocation.X * 10f + drawLocation.Y) % 100f;
        var rng = new FastRandom((int)sudoRNG);
        rng.NextSeed();

        GetItemDrawValues(progress, rng, ref drawLocation, out var lightColor, out var opacity, out var rotation, out var scale);
        Main.GetItemDrawFrame(animation.ItemId, out var texture, out var frame);

        Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(Color.White, lightColor)) * opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(itemInstance.color, lightColor)) * opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
    }

    public override void PostDrawTiles() {
        if (SpewAnimations.Count <= 0) {
            return;
        }

        Main.spriteBatch.Begin_World();

        for (int i = 0; i < SpewAnimations.Count; i++) {
            if (SpewAnimations[i].AnimationTime > 0) {
                DrawItemEffect(SpewAnimations[i]);
            }
        }

        Main.spriteBatch.End();
    }
}