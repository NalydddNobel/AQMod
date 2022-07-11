using Aequus.Biomes.Glimmer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public class GlimmerBiome : ModBiome
    {
        public const ushort MaxTiles = 1650;
        public const ushort SuperStariteTile = 1200;
        public const ushort HyperStariteTile = 800;
        public const ushort UltraStariteTile = 500;
        public const float StariteSpawn = 1f;
        public const float SuperStariteSpawn = 0.75f;
        public const float HyperStariteSpawn = 0.4f;
        public const float UltraStariteSpawn = 0.2f;

        public static Color CosmicEnergyColor = new Color(200, 10, 255, 0);
        public static Color TextColor = new Color(238, 17, 68, 255);

        public static Point TileLocation { get; set; }


        public static int omegaStarite;

        public static bool EventActive => TileLocation != Point.Zero;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => Aequus.AssetsPath + "UI/BestiaryIcons/Glimmer";

        public override string BackgroundPath => Aequus.AssetsPath + "UI/MapBGs/Glimmer";
        public override string MapBackground => BackgroundPath;

        public override bool IsBiomeActive(Player player)
        {
            return EventActive && GlimmerSystem.CalcTiles(player) < MaxTiles;
        }
    }
}