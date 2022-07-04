using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public class GlimmerInvasion : ModBiome
    {
        public static readonly Color CosmicEnergyColor = new Color(200, 10, 255, 0);

        public static InvasionStatus Status { get; set; }
        public static int omegaStarite;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => Aequus.AssetsPath + "UI/BestiaryIcons/Glimmer";

        public override string BackgroundPath => Aequus.AssetsPath + "UI/MapBGs/Glimmer";
        public override string MapBackground => BackgroundPath;
    }
}