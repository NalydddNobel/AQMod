using Aequus.Core.Graphics.Animations;
using Aequus.Core.Graphics.GameOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public struct AnimationItemSpew : IOverlayDrawer {
    public readonly Vector2 Location;
    public readonly Point TileLocation;
    public readonly Point16 TileOrigin;
    public readonly int ItemId;
    public float AnimationTime;
    public bool SpawnedParticles;

    public AnimationItemSpew(Vector2 location, Point16 tileOrigin, int itemId) {
        Location = location;
        TileOrigin = tileOrigin;
        TileLocation = location.ToTileCoordinates();
        ItemId = itemId;
    }

    private void SpawnItemEffectParticles(Item itemInstance) {
        if (itemInstance.createTile < TileID.Dirt) {
            return;
        }
        for (int l = 0; l < 3; l++) {
            int d = TileHelper.GetTileDust(TileLocation.X, TileLocation.Y, itemInstance.createTile, itemInstance.placeStyle);
            if (d == -1 || d == Main.maxDust) {
                continue;
            }

            Main.dust[d].position = Location + Main.rand.NextVector2Square(-4f, 4f);
            Main.dust[d].fadeIn = Main.dust[d].scale;
            Main.dust[d].scale *= 0.4f;
            Main.dust[d].velocity.X = Main.rand.NextFloat(-2f, -1f);
            Main.dust[d].velocity.Y = Main.rand.NextFloat(-3f, -1f);
        }
    }

    public bool Update() {
        AnimationTime += 1f;

        var itemInstance = ContentSamples.ItemsByType[ItemId];
        if (AnimationTime > 0f && !SpawnedParticles) {
            lock (AnimationSystem.TileAnimations) {
                var tileAnimation = AnimationSystem.GetValueOrAddDefault<AnimationTrashCompactor>(TileOrigin);
                tileAnimation.ShakeTime = Math.Min(tileAnimation.ShakeTime + 2f, 4f);
            }
            SpawnItemEffectParticles(itemInstance);
            SoundEngine.PlaySound(SoundID.Item102, Location);
            SpawnedParticles = true;
        }

        return AnimationTime <= 60;
    }

    private void GetItemDrawValues(float progress, FastRandom rng, ref Vector2 drawLocation, out Color lightColor, out float opacity, out float rotation, out float scale) {
        bool noGravity = ItemID.Sets.ItemNoGravity[ItemId];
        opacity = 1f;
        rotation = (progress * 6f + rng.NextFloat(10f)) * (noGravity ? 0.03f : 1f);
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
            if (noGravity) {
                drawLocation.Y += MathF.Pow(fallProgress, 2f) * -40f - peakHeight;
            }
            else {
                drawLocation.Y += MathF.Pow(fallProgress, 2f) * 300f - peakHeight;
            }
            opacity *= 1f - MathF.Pow(fallProgress, 5f);
        }

        lightColor = LightHelper.GetLightColor(drawLocation);
        scale = rng.NextFloat(0.6f, 0.8f);
    }

    public void Draw(SpriteBatch spriteBatch) {
        if (AnimationTime < 0) {
            return;
        }

        float progress = AnimationTime / 60f;
        var itemInstance = ContentSamples.ItemsByType[ItemId];
        var drawLocation = Location;
        float sudoRNG = (drawLocation.X * 10f + drawLocation.Y) % 100f;
        var rng = new FastRandom((int)sudoRNG);
        rng.NextSeed();

        GetItemDrawValues(progress, rng, ref drawLocation, out var lightColor, out var opacity, out var rotation, out var scale);
        Main.GetItemDrawFrame(ItemId, out var texture, out var frame);

        Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(Color.White, lightColor)) * opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        if (itemInstance.color != Color.Transparent) {
            Main.spriteBatch.Draw(texture, drawLocation - Main.screenPosition, frame, itemInstance.GetAlpha(Utils.MultiplyRGBA(itemInstance.color, lightColor)) * opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }
    }
}