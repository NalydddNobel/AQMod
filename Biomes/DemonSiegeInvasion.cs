using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public sealed class DemonSiegeInvasion : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override int Music => MusicData.DemonSiegeEvent.GetID();

        public override string BestiaryIcon => "Assets/UI/BestiaryIcons/DemonSiege";

        public override bool IsBiomeActive(Player player)
        {
            return false;
        }
    }
}