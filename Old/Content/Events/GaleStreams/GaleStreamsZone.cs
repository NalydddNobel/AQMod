namespace Aequu2.Content.Events.GaleStreams;

public class GaleStreamsZone : ModBiome {
    public static bool EventActive => Aequu2System.HardmodeTier && Main.WindyEnoughForKiteDrops;

    public override int Music => MusicLoader.GetMusicSlot("Aequu2Music/Assets/Music/GaleStreamsOld");

    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override string BestiaryIcon => Aequu2Textures.GaleStreamsBestiaryIcon.Path;

    public override string BackgroundPath => Aequu2Textures.MapBG(33);
    public override string MapBackground => BackgroundPath;

    public override bool IsBiomeActive(Player player) {
        if (!EventActive || !player.ZoneSkyHeight || player.townNPCs >= 2f || player.ZonePeaceCandle) {
            return false;
        }

        if (player.behindBackWall) {
            var p = player.Center.ToTileCoordinates();
            if (Main.tile[p].WallType >= TileID.Count) {
                return false;
            }
        }

        return true;
    }
}