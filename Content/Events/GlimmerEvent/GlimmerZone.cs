using Aequus.Common;
using Aequus.Common.UI.EventBars;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GlimmerEvent {
    public class GlimmerZone : ModBiome {
        public const ushort MaxTiles = 1650;
        public const ushort SuperStariteTile = 1200;
        public const ushort HyperStariteTile = 800;
        public const ushort UltraStariteTile = 500;
        public const float StariteSpawn = 1f;
        public const float SuperStariteSpawn = 0.75f;
        public const float HyperStariteSpawn = 0.4f;
        public const float UltraStariteSpawn = 0.2f;

        public static Color CosmicEnergyColor = new Color(200, 10, 255, 0);
        public static ConfiguredMusicData music { get; private set; }

        public static Point TileLocation { get; set; }

        public static int omegaStarite;

        public static bool EventTechnicallyActive => TileLocation != Point.Zero;
        public static bool EventActive => EventTechnicallyActive && GlimmerSystem.EndEventDelay <= 0;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => AequusTextures.GlimmerBestiaryIcon.Path;

        public override string BackgroundPath => AequusTextures.GlimmerMapBackground.Path;
        public override string MapBackground => BackgroundPath;

        public override int Music => music.GetID();

        public override void Load() {
            if (!Main.dedServ) {
                music = new(MusicID.MartianMadness, MusicID.OtherworldlyEerie);
                AequusEventBarLoader.AddBar(new GlimmerBar() {
                    DisplayName = DisplayName,
                    Icon = AequusTextures.GlimmerEventIcon,
                    backgroundColor = new Color(20, 75, 180, 128),
                });
            }
        }

        public override void Unload() {
            music = null;
        }

        public override bool IsBiomeActive(Player player) {
            return EventActive && (Main.remixWorld ? player.position.Y >= Main.UnderworldLayer : player.position.Y < Main.worldSurface * 16f) && GlimmerSystem.GetTileDistance(player) < MaxTiles;
        }
    }
}