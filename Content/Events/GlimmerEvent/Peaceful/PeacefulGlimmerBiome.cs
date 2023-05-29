using Aequus.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GlimmerEvent.Peaceful {
    public class PeacefulGlimmerBiome : ModBiome
    {
        public const ushort MaxTiles = 500;

        public static ConfiguredMusicData music { get; private set; }

        public static bool EventActive { get => TileLocationX != 0; }

        public static int TileLocationX { get; set; }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
        public override int Music => music.GetID();

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.Space, MusicID.OtherworldlySpace);
            }
        }

        public override void Unload()
        {
            music = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            return EventActive && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && CalcTiles(player) < MaxTiles;
        }

        public static int CalcTiles(Player player)
        {
            return (int)((player.position.X + player.width) / 16 - TileLocationX).Abs();
        }
    }
}