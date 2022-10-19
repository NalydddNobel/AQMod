using Terraria.ModLoader;

namespace Aequus.Common.ModPlayers
{
    public class ZenPlayer : ModPlayer
    {
        public bool forceZen;

        public override void ResetEffects()
        {
            forceZen = false;
        }
    }
}