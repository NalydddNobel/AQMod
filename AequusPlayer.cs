using Aequus.Assets.Effects;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed class AequusPlayer : ModPlayer
    {
        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Applied by <see cref="Buffs.NoonBuff"/></para>
        /// </summary>
        public byte forceDaytime;

        /// <summary>
        /// Applied by <see cref="Buffs.Debuffs.BlueFire"/>
        /// </summary>
        public bool blueFire;
        /// <summary>
        /// Applied by <see cref="Buffs.Debuffs.PickBreak"/>
        /// </summary>
        public bool pickBreak;

        /// <summary>
        /// Applied by <see cref="Buffs.Pets.FamiliarBuff"/>
        /// </summary>
        public bool familiarPet;

        /// <summary>
        /// Applied by <see cref="Buffs.Pets.OmegaStariteBuff"/>
        /// </summary>
        public bool omegaStaritePet;

        /// <summary>
        /// Tracks <see cref="Terraria.Player.selectedItem"/>, reset in <see cref="PostItemCheck"/>
        /// </summary>
        public int lastSelectedItem = -1;

        public override void PreUpdate()
        {
            if (forceDaytime == 1)
            {
                Aequus.DayTimeManipulator.TemporarilySet(true);
            }
            else if (forceDaytime == 2)
            {
                Aequus.DayTimeManipulator.TemporarilySet(false);
            }
            forceDaytime = 0;
        }

        public override void ResetEffects()
        {
            forceDaytime = 0;

            blueFire = false;
            pickBreak = false;

            familiarPet = false;
            omegaStaritePet = false;
        }

        public override void PostItemCheck()
        {
            lastSelectedItem = Player.selectedItem;
        }

        public override void PostUpdate()
        {
            if (Aequus.DayTimeManipulator.Caching)
            {
                Aequus.DayTimeManipulator.Clear();
            }
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