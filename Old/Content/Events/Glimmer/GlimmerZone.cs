using Aequus.Old.Common.EventBars;

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

    public override void Unload() {
    }

    public override bool IsBiomeActive(Player player) {
        return EventActive && (Main.remixWorld ? player.position.Y >= Main.UnderworldLayer : player.position.Y < Main.worldSurface * 16f) && GlimmerSystem.GetTileDistance(player) < MaxTiles;
    }
}