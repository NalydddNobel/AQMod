using Aequus.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GaleStreams {
    public class GaleStreamsZone : ModBiome {
        public static bool EventActive => Aequus.MediumMode && Main.WindyEnoughForKiteDrops;

        public static ConfiguredMusicData music { get; private set; }

        public override int Music => music.GetID();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => AequusTextures.GaleStreamsBestiaryIcon.FullPath;

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG33";
        public override string MapBackground => BackgroundPath;

        public override void Load() {
            if (!Main.dedServ) {
                music = new ConfiguredMusicData(MusicID.Sandstorm, MusicID.OtherworldlyTowers);
            }
        }

        public override void Unload() {
            music = null;
        }

        public override bool IsBiomeActive(Player player) {
            if (!EventActive || !player.ZoneSkyHeight || !player.Center.InOuterX(3) || player.townNPCs >= 2f || player.ZonePeaceCandle) {
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
}