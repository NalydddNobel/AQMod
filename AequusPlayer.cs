using Aequus.Common;
using Aequus.Common.Players;
using Aequus.Content;
using Aequus.Content.Invasions;
using Aequus.Effects;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed class AequusPlayer : ModPlayer, ITemperatureEntity
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
        /// Applied by <see cref="Aequus.Items.Equipment.Accessories.FrigidTalisman"/>
        /// </summary>
        public bool accGSFreezeResist;

        /// <summary>
        /// Tracks <see cref="Terraria.Player.selectedItem"/>, reset in <see cref="PostItemCheck"/>
        /// </summary>
        public int lastSelectedItem = -1;
        /// <summary>
        /// When a new cooldown is applied, this gets set to the duration of the cooldown. Does not tick down unlike <see cref="itemCooldown"/>
        /// </summary>
        public ushort itemCooldownMax;
        /// <summary>
        /// When above 0, the cooldown is active. Ticks down by 1 every player update.
        /// </summary>
        public ushort itemCooldown;
        /// <summary>
        /// When above 0, you are in a combo. Ticks down by 1 every player update.
        /// <para>Item "combos" are used for determining what type of item action to use.</para>
        /// <para>A usage example would be a weapon with a 3 swing pattern. Each swing will increase the combo meter by 60, and when it becomes greater than 120, reset to 0.</para>
        /// </summary>
        public ushort itemCombo;
        /// <summary>
        /// Increments when the player uses an item. Does not increment when the player is using the alt function of an item.
        /// </summary>
        public ushort itemUsage;
        /// <summary>
        /// A short lived timer which gets set to 30 when the player has a different selected item.
        /// </summary>
        public ushort itemSwitch;
        /// <summary>
        /// Used to prevent players from spam interacting with special objects which may have important networking actions which need to be awaited. Ticks down by 1 every player update.
        /// </summary>
        public uint interactionCooldown;

        /// <summary>
        /// Whether or not the player is in the Gale Streams event. This is set to true when <see cref="GaleStreams.Status"/> equals <see cref="InvasionStatus.Active"/> and the <see cref="GaleStreams.IsThisSpace(Terraria.Player)"/> returns true in <see cref="PreUpdate"/>. Otherwise, this is false.
        /// </summary>
        public bool eventGaleStreams;

        /// <summary>
        /// The player's current temperature. Ticks down by 1 every frame
        /// </summary>
        public int temperature;
        /// <summary>
        /// Clamps <see cref="currentTemperature"/> using this as the maximum
        /// </summary>
        public int maxTemperature;
        /// <summary>
        /// Clamps <see cref="currentTemperature"/> using this as the minimum
        /// </summary>
        public int minTemperature;
        /// <summary>
        /// Whenever the <see cref="currentTemperature"/> decrements, it uses this value
        /// </summary>
        public int temperatureRegen;

        public bool inDanger;
        /// <summary>
        /// Helper for whether or not the player currently has a cooldown.
        /// </summary>
        public bool HasCooldown => itemCooldown > 0;

        int ITemperatureEntity.Temperature { get => temperature; set => temperature = value; }

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
            if (GaleStreams.Status == InvasionStatus.Active && GaleStreams.IsThisSpace(Player))
            {
                eventGaleStreams = true;
            }
            else
            {
                eventGaleStreams = false;
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

        public override void PostUpdateEquips()
        {
        }

        public override bool PreItemCheck()
        {
            if (Aequus.DayTimeManipulator.Caching)
                Aequus.DayTimeManipulator.Repair();
            if (itemCombo > 0)
            {
                itemCombo--;
            }
            if (itemSwitch > 0)
            {
                itemUsage = 0;
                itemSwitch--;
            }
            else if (Player.itemTime > 0)
            {
                itemUsage++;
            }
            else
            {
                itemUsage = 0;
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
            return true;
        }

        public override void PostItemCheck()
        {
            if (Aequus.DayTimeManipulator.Caching)
                Aequus.DayTimeManipulator.Disrepair();
            if (Player.selectedItem != lastSelectedItem)
            {
                lastSelectedItem = Player.selectedItem;
                itemSwitch = 30;
                itemUsage = 0;
            }
        }

        public override void PostUpdate()
        {
            if (Aequus.DayTimeManipulator.Caching)
            {
                Aequus.DayTimeManipulator.Clear();
            }
            CheckDanger();
        }
        public void CheckDanger()
        {
            inDanger = false;
            var checkTangle = new Rectangle((int)Player.position.X + Player.width / 2 - 1000, (int)Player.position.X + Player.head / 2 - 500, 2000, 1000);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5)
                {
                    if (Main.npc[i].getRect().Intersects(checkTangle))
                    {
                        inDanger = true;
                        return;
                    }
                }
            }
        }


        public override void ModifyScreenPosition()
        {
            ModContent.GetInstance<GameCamera>().UpdateScreen();
            EffectsSystem.UpdateScreenPosition();
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

        /// <summary>
        /// Sets a cooldown for the player. If the cooldown value provided is less than the player's currently active cooldown, this does nothing.
        /// <para>Use in combination with <see cref="HasCooldown"/></para>
        /// </summary>
        /// <param name="cooldown">The amount of time the cooldown lasts in game ticks.</param>
        /// <param name="ignoreStats">Whether or not to ignore cooldown stats and effects. Setting this to true will prevent them from effecting this cooldown</param>
        public void SetCooldown(int cooldown, bool ignoreStats = false)
        {
            if (cooldown < itemCooldown)
            {
                return;
            }

            itemCooldownMax = (ushort)cooldown;
            itemCooldown = (ushort)cooldown;
        }

        public void ModifyTemperatureApplication(ref int temperature, ref int minTemperature, ref int maxTemperature)
        {
        }
    }
}