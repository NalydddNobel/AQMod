using Aequus.Assets.Effects;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed class AequusPlayer : ModPlayer
    {
        /// <summary>
        /// A value of 1 forces it to be day while updating this player.<para>A value of 2 forces it to be night while updating this player.</para>
        /// <para>Used by <see cref="Buffs.NoonBuff"/></para>
        /// </summary>
        public byte forceDaytime;

        public bool blueFire;
        public bool pickBreak;

        public bool familiarPet;

        public ushort itemCooldownMax;
        public ushort itemCooldown;
        public ushort itemCombo;
        public ushort itemSwitch;
        public int lastSelectedItem = -1;
        public uint interactionCooldown;

        public override void Initialize()
        {
            itemCooldown = 0;
            itemCooldownMax = 0;
            itemCombo = 0;
            itemSwitch = 0;
            interactionCooldown = 60;
        }

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
            if (itemCombo > 0)
            {
                itemCombo--;
            }
            if (itemSwitch > 0)
            {
                itemSwitch--;
            }
            if (itemCooldown > 0)
            {
                if (itemCooldownMax == 0)
                {
                    itemCooldown = 0;
                    itemCooldownMax = 0;
                }
                else
                {
                    itemCooldown--;
                    if (itemCooldown == 0)
                    {
                        itemCooldownMax = 0;
                    }
                }
                Player.manaRegen = 0;
                Player.manaRegenDelay = (int)Player.maxRegenDelay;
            }
            if (interactionCooldown > 0)
            {
                interactionCooldown--;
            }

            forceDaytime = 0;

            blueFire = false;
            pickBreak = false;

            familiarPet = false;
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