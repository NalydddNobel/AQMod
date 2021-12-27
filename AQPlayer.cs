using Terraria.ModLoader;

namespace AQMod
{
    public class AQPlayer : ModPlayer
    {
        public bool blueFire;

        public override void ResetEffects()
        {
            blueFire = false;
        }
    }
}