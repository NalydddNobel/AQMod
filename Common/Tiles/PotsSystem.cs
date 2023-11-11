using Aequus.Common.UI;
using Aequus.Content.DataSets;
using Aequus.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Common.Tiles;

public class PotsSystem : ModSystem {
    public record PotLootPreview(Texture2D Texture, Rectangle? Frame, int Stack, bool Dangerous) {
        public float Opacity;
    }

    public static Dictionary<Point, PotLootPreview> LootPreviews { get; private set; } = new();

    public static Queue<Point> RemoveQueue { get; private set; } = new();

    internal static MethodInfo SpawnThingsFromPot;
    internal static MethodInfo KillTile_DropItems;

    public override void Load() {
        KillTile_DropItems = typeof(WorldGen).GetMethod("KillTile_DropItems", BindingFlags.NonPublic | BindingFlags.Static);
        SpawnThingsFromPot = typeof(WorldGen).GetMethod("SpawnThingsFromPot", BindingFlags.NonPublic | BindingFlags.Static);
        On_WorldGen.SpawnThingsFromPot += On_WorldGen_SpawnThingsFromPot;
    }

    private static void On_WorldGen_SpawnThingsFromPot(On_WorldGen.orig_SpawnThingsFromPot orig, int i, int j, int x2, int y2, int style) {
        var mainRand = Main.rand;
        var genRand = WorldGen._genRand;

        try {
            var randomizer = new UnifiedRandom((int)Helper.TileSeed(x2, y2));
            Main.rand = randomizer;
            WorldGen._genRand = randomizer;

            //var d = Dust.NewDustPerfect(new Vector2(x2, y2).ToWorldCoordinates(), DustID.FrostHydra);
            //d.noGravity = true;
            //d.velocity = Vector2.Zero;
            //d.fadeIn = d.scale + 3f;
            //d = Dust.NewDustPerfect(new Vector2(i, j).ToWorldCoordinates(), DustID.Torch);
            //d.noGravity = true;
            //d.velocity = Vector2.Zero;
            //d.fadeIn = d.scale + 3f;
            //Main.NewText($"x:{Main.tile[i, j].TileFrameX} y:{Main.tile[i, j].TileFrameY}");

            orig(i, j, x2, y2, style);
        }
        catch {
        }

        NewNPCCache.End();
        NewProjectileCache.End();
        NewItemCache.End();
        GoreDisabler.End();

        Main.rand = mainRand;
        WorldGen._genRand = genRand;
    }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        lock (LootPreviews) {
            var aequusPlayer = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
            int effectRange = aequusPlayer.potSightRange;
            foreach (var preview in LootPreviews) {
                if (!Main.tile[preview.Key].HasTile || !TileSets.IsSmashablePot.Contains(Main.tile[preview.Key].TileType) || !InPotSightRange(Main.LocalPlayer, preview.Key, effectRange)) {
                    preview.Value.Opacity -= 0.04f;
                    if (preview.Value.Opacity <= 0f) {
                        RemoveQueue.Enqueue(preview.Key);
                    }
                }
                else if (preview.Value.Opacity < 1f) {
                    preview.Value.Opacity += 0.1f;
                    if (preview.Value.Opacity > 1f) {
                        preview.Value.Opacity = 1f;
                    }
                }
            }

            while (RemoveQueue.TryDequeue(out var removePoint)) {
                LootPreviews.Remove(removePoint);
            }
        }
    }

    private void DrawPreview(Point tileCoordinates, PotLootPreview preview) {
        var seed = Helper.TileSeed(tileCoordinates) % 10000f;

        float scale = Math.Min(preview.Opacity, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f + seed, 0.9f, 1f));
        float pulseScale = scale;

        var frame = preview.Frame ?? preview.Texture.Bounds;
        int largestSide = Math.Max(frame.Width, frame.Height);
        if (largestSide > 24f) {
            scale *= 24f / largestSide;
        }

        var drawCoordinates = new Vector2(tileCoordinates.X * 16f + 16f, tileCoordinates.Y * 16f + 20f) - Main.screenPosition;
        var itemWobbleOffset = new Vector2(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 3f + seed * 0.9f, -1f, 1f), Helper.Oscillate(Main.GlobalTimeWrappedHourly * 1.2f + seed * 0.8f, -2f, 2f));
        float rotation = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 4.2f, -0.1f, 0.1f);
        float opacity = 1f;
        if (preview.Dangerous) {
            opacity *= Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f + seed, 0.3f, 1f);
        }
        MiscWorldInterfaceElements.Draw(AequusTextures.BloomStrong, drawCoordinates, null, Color.Black * opacity * (preview.Dangerous ? 0.33f : 0.75f) * preview.Opacity, 0f, AequusTextures.BloomStrong.Size() / 2f, 0.4f, SpriteEffects.None, 0f);
        MiscWorldInterfaceElements.Draw(preview.Texture, drawCoordinates + itemWobbleOffset + new Vector2(2f) * scale, frame, Color.Black * 0.33f * opacity * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        MiscWorldInterfaceElements.Draw(preview.Texture, drawCoordinates + itemWobbleOffset, frame, Color.White * 0.75f * opacity * pulseScale * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);

        int sparkleCount = Aequus.highQualityEffects ? 6 : 3;
        for (int i = 0; i < sparkleCount; i++) {
            float timer = seed + (i * (seed + 500) + Main.GlobalTimeWrappedHourly * 1.1f) + i / (float)sparkleCount;
            var random = new FastRandom((int)timer);
            timer %= 1f;
            if (timer > 1f) {
                continue;
            }

            timer = MathF.Pow(timer, random.NextFloat(1f, 2.5f));
            random.NextSeed();
            var sparkleOffset = new Vector2(random.NextFloat(-12f, 12f), random.NextFloat(-12f, 12f) + 4f);
            var sparkleFrame = AequusTextures.BaseParticleTexture.Frame(verticalFrames: 3, frameY: random.Next(3));
            float sparkleFade = MathF.Sin(timer * MathHelper.Pi);
            MiscWorldInterfaceElements.Draw(AequusTextures.BaseParticleTexture, drawCoordinates + new Vector2(0f, -timer * 4f) + sparkleOffset, sparkleFrame, Color.Orange with { A = 0 } * sparkleFade * 0.45f * preview.Opacity, 0f, sparkleFrame.Size() / 2f, sparkleFade * random.NextFloat(1f, 1.5f), SpriteEffects.None, 0f);
        }

        if (preview.Stack > 1) {
            MiscWorldInterfaceElements.DrawColorCodedString(FontAssets.MouseText.Value, preview.Stack.ToString(), drawCoordinates + new Vector2(-12f, -2f), Color.White * 0.66f * preview.Opacity, 0f, Vector2.Zero, Vector2.One * 0.66f);
        }
    }

    public override void PostDrawTiles() {
        lock (LootPreviews) {
            foreach (var preview in LootPreviews) {
                DrawPreview(preview.Key, preview.Value);
            }
        }
    }

    public static bool InPotSightRange(Player player, Point potCoordinates, int range) {
        return range > 0 && Vector2.Distance(player.Center, new Vector2(potCoordinates.X * 16f + 16f, potCoordinates.Y * 16f + 16f)) < range;
    }
}