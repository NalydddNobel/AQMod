using Aequus.Assets.Effects;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed class AequusPlayer : ModPlayer
    {
        public bool blueFire;
        public bool pickBreak;

        public override void ResetEffects()
        {
            blueFire = false;
            pickBreak = false;
        }

        public override void ModifyScreenPosition()
        {
            GameCamera.Instance.UpdateScreen();
            GameEffects.Instance.UpdateScreen();
        }
    }
}