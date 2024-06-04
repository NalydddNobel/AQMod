using Aequus.Content.Items.Accessories.EventPrevention;
using Aequus.Old.Common.EventBars;
using Aequus.Old.Content.Bosses.UltraStarite;
using Aequus.Old.Content.Critters;
using Aequus.Old.Content.Enemies.Glimmer.Hyper;
using Aequus.Old.Content.Enemies.Glimmer.Proto;
using Aequus.Old.Content.Enemies.Glimmer.Super;
using System.Collections.Generic;

namespace Aequus.Old.Content.Events.Glimmer;

public class GlimmerZone : ModBiome {
    public static int MaxTiles => (int)(Main.maxTilesX * 0.4f);
    public static int SuperStariteTile => (int)(Main.maxTilesX * 0.3f);
    public static int HyperStariteTile => (int)(Main.maxTilesX * 0.2f);
    public static int UltraStariteTile => (int)(Main.maxTilesX * 0.1f);

    public const float StariteSpawn = 1f;
    public const float SuperStariteSpawn = 0.75f;
    public const float HyperStariteSpawn = 0.4f;
    public const float UltraStariteSpawn = 0.2f;

    public static Point TileLocation { get; set; }

    public static int omegaStarite { get; set; }

    public static bool EventTechnicallyActive => TileLocation != Point.Zero;
    public static bool EventActive => EventTechnicallyActive && GlimmerSystem.EndEventDelay <= 0;

    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override string BestiaryIcon => AequusTextures.GlimmerBestiaryIcon.Path;

    public override string BackgroundPath => AequusTextures.GlimmerMapBackground.Path;
    public override string MapBackground => BackgroundPath;

    public override int Music => MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/Glimmer");

    public override void Load() {
        if (!Main.dedServ) {
            AequusEventBarLoader.AddBar(new GlimmerBar() {
                DisplayName = DisplayName,
                Icon = AequusTextures.GlimmerEventIcon,
                backgroundColor = new Color(20, 75, 180, 128),
            });
        }
    }

    public override bool IsBiomeActive(Player player) {
        return EventActive && (Main.remixWorld ? player.position.Y >= Main.UnderworldLayer : player.position.Y < Main.worldSurface * 16f) && GlimmerSystem.GetTileDistance(player) < MaxTiles && !player.GetModPlayer<EventDeactivatorPlayer>().accDisableGlimmer;
    }

    public static void AddEnemies(int tiles, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (tiles < SuperStariteTile) {
            pool.Clear();
        }

        int starite = ModContent.NPCType<Starite>();
        int superStarite = ModContent.NPCType<SuperStarite>();
        int hyperStarite = ModContent.NPCType<HyperStarite>();
        int ultraStarite = ModContent.NPCType<UltraStarite>();

        int maxHyper = tiles < UltraStariteTile ? 2 : 1;
        int maxUltra = 1;

        if (Main.getGoodWorld) {
            maxHyper = int.MaxValue;
            maxUltra = int.MaxValue;
        }

        pool.Add(ModContent.NPCType<DwarfStarite>(), StariteSpawn);
        pool.Add(starite, StariteSpawn);

        if (CanSpawnGlimmerEnemies(spawnInfo.Player)) {
            if (tiles < SuperStariteTile) {
                pool.Add(superStarite, SuperStariteSpawn);
            }

            if (tiles < HyperStariteTile) {
                pool[starite] *= 0.5f;
                pool[superStarite] *= 0.75f;

                if (NPC.CountNPCS(hyperStarite) < maxHyper) {
                    pool.Add(hyperStarite, HyperStariteSpawn);
                }
            }

            if (tiles < UltraStariteTile) {
                pool[starite] *= 0.33f;
                pool[superStarite] *= 0.5f;

                if (NPC.CountNPCS(ultraStarite) < maxUltra) {
                    pool.Add(ultraStarite, UltraStariteSpawn * (AequusSystem.downedUltraStarite ? 1f : 2f));
                }
            }
        }
    }

    private static bool CanSpawnGlimmerEnemies(Player player) {
        return player.InModBiome<GlimmerZone>() && player.townNPCs < 2f && GlimmerSystem.GetTileDistance(player) > 100;
    }
}