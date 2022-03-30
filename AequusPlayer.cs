using Aequus.Assets.Effects;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed class AequusPlayer : ModPlayer
    {
        public bool blueFire;
        public bool pickBreak;

        public bool familiarPet;

        public override void ResetEffects()
        {
            blueFire = false;
            pickBreak = false;

            familiarPet = false;
        }

        public override void ModifyScreenPosition()
        {
            GameCamera.Instance.UpdateScreen();
            GameEffects.Instance.UpdateScreen();
        }

        /// <summary>
        /// Called right before all player layers have been drawn
        /// </summary>
        /// <param name="info"></param>
        public void PreDraw(ref PlayerDrawSet info)
        {
        }

        /// <summary>
        /// Called right after all player layers have been drawn
        /// </summary>
        /// <param name="info"></param>
        public void PostDraw(ref PlayerDrawSet info)
        {

        }
    }
}