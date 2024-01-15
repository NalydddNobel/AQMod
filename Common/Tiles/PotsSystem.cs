using Aequus.Content.DataSets;
using Aequus.Core;
using System.Collections.Generic;
using System.Reflection;
using Terraria.Utilities;

namespace Aequus.Common.Tiles;

// TODO -- Make the angler lantern systems no longer need to use a global tile to get on-screen pots for optimization
public class PotsSystem : ModSystem {
    public record PotLootPreview(Texture2D Texture, Rectangle? Frame, int Stack, bool Dangerous) {
        public Color NPCColor;
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
                if (!Main.tile[preview.Key].HasTile || !TileSets.IsSmashablePot.Contains((int)Main.tile[preview.Key].TileType) || !InPotSightRange(Main.LocalPlayer, preview.Key, effectRange)) {
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

    public override void PostDrawTiles() {
        if (LootPreviews.Count > 0) {
            ModContent.GetInstance<PotsUI>().Activate();
        }
    }

    public static bool InPotSightRange(Player player, Point potCoordinates, int range) {
        return range > 0 && Vector2.Distance(player.Center, new Vector2(potCoordinates.X * 16f + 16f, potCoordinates.Y * 16f + 16f)) < range;
    }
}
