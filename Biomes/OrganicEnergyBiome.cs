using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public class OrganicEnergyBiome : ModBiome
    {
        public static ConfiguredMusicData music { get; private set; }

        public override int Music => music.GetID();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => $"{Aequus.AssetsPath}UI/BestiaryIcons/JungleEvent";

        public override string BackgroundPath => $"{Aequus.VanillaTexture}MapBG9";
        public override string MapBackground => BackgroundPath;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.Temple, MusicID.OtherworldlyUnderworld);
            }
        }

        public override void Unload()
        {
            music = null;
        }
    }
}