namespace Aequus.Content.Events.GaleStreams;

public class GaleStreamsZone : ModBiome {
    public static bool EventActive => AequusSystem.HardmodeTier && Main.WindyEnoughForKiteDrops;

    public override int Music => MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/GaleStreamsOld");

    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override string BestiaryIcon => AequusTextures.GaleStreamsBestiaryIcon.Path;

    public override string BackgroundPath => AequusTextures.MapBG(33);
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