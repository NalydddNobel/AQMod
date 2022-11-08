using Aequus.Biomes.CrabCrevice;
using Aequus.Sounds;
using Aequus.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public class CrabCreviceBiome : ModBiome
    {
        public static ConfiguredMusicData music { get; private set; }

        public override string BestiaryIcon => "Aequus/Assets/UI/BestiaryIcons/CrabCrevice";

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG11";
        public override string MapBackground => BackgroundPath;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrabCreviceWater>();

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CrabCreviceUGBackground>();

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music => music.GetID();

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.OceanNight);
            }
        }

        public override void Unload()
        {
            music = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            if (SedimentaryRockTile.BiomeCount > 150)
                return true;

            var loc = player.Center.ToTileCoordinates();
            return WorldGen.InWorld(loc.X, loc.Y, 10) && Main.tile[loc].WallType == ModContent.WallType<SedimentaryRockWallWall>();
        }
    }
}