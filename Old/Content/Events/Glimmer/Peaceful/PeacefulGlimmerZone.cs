using System;

namespace Aequu2.Old.Content.Events.Glimmer.Peaceful;
public class PeacefulGlimmerZone : ModBiome {
    public const ushort MaxTiles = 500;

    public static bool EventActive => TileLocationX != 0;

    public static int TileLocationX { get; set; }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
    public override int Music => MusicID.Space;

    public override bool IsBiomeActive(Player player) {
        return EventActive && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && CalcTiles(player) < MaxTiles;
    }

    public static int CalcTiles(Player player) {
        return (int)Math.Abs((player.position.X + player.width) / 16 - TileLocationX);
    }
}