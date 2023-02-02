using Terraria.ModLoader;

namespace Aequus.Common.ModPlayers
{
    public partial class AequusPlayer : ModPlayer
    {
        public bool forceZen;

        public void ResetEffects_Zen()
        {
            forceZen = false;
        }
    }
}