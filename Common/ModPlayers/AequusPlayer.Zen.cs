using Terraria.ModLoader;

namespace Aequus {
    public partial class AequusPlayer : ModPlayer
    {
        public bool forceZen;

        public void ResetEffects_Zen()
        {
            forceZen = false;
        }
    }
}