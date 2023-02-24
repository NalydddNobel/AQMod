using Aequus.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes.CrabCrevice
{
    public class CrabCreviceBiome : ModBiome
    {
        public static ConfiguredMusicData music { get; private set; }

        public override string BestiaryIcon => "Aequus/Assets/UI/BestiaryIcons/CrabCrevice";

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG11";
        public override string MapBackground => BackgroundPath;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrabCreviceWater>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrabCreviceSurfaceBackground>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CrabCreviceUGBackground>();

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music => music.GetID();

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.OceanNight, MusicID.OtherworldlyOcean);
            }
        }

        public override void Unload()
        {
            music = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            //Main.NewText(Main.bgStyle);
            if (SedimentaryRockTile.BiomeCount > 150)
                return true;

            var loc = player.Center.ToTileCoordinates();
            return WorldGen.InWorld(loc.X, loc.Y, 10) && Main.tile[loc].WallType == ModContent.WallType<SedimentaryRockWallWall>();
        }
    }
}