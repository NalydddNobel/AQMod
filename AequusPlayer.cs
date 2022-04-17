using Aequus.Common;
using Aequus.Common.Players;
using Aequus.Common.Players.Stats;
using Aequus.Content.Invasions;
using Aequus.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed partial class AequusPlayer : ModPlayer
    {
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
        /// Applied by <see cref="Items.Accessories.FrigidTalisman"/>
        /// </summary>
        public bool accGSFreezeResist;

        /// <summary>
        /// Whether or not the player is in the Gale Streams event. This is set to true when <see cref="GaleStreams.Status"/> equals <see cref="InvasionStatus.Active"/> and the <see cref="GaleStreams.IsThisSpace(Terraria.Player)"/> returns true in <see cref="PreUpdate"/>. Otherwise, this is false.
        /// </summary>
        public bool eventGaleStreams;

        /// <summary>
        /// Whether or not the player is 'in danger'. Updated in <see cref="PostUpdate"/> / <see cref="PostUpdate_CheckDanger"/>
        /// </summary>
        public bool inDanger;

        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Applied by <see cref="Buffs.NoonBuff"/></para>
        /// </summary>
        public byte forceDaytime;

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

        public CustomStatsManager Stats { get; private set; }

        /// <summary>
        /// Helper for whether or not the player currently has a cooldown.
        /// </summary>
        public bool HasCooldown => itemCooldown > 0;

        public override void Load()
        {
            LoadHooks();
        }

        public override void Initialize()
        {
            Stats = new CustomStatsManager();
            Stats.Initialize(this);
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
            eventGaleStreams = CheckEventGaleStreams();
            forceDaytime = 0;
        }
        private bool CheckEventGaleStreams()
        {
            return GaleStreams.Status == InvasionStatus.Active && GaleStreams.IsThisSpace(Player);
        }

        public override void ResetEffects()
        {
            Stats.ResetEffects(this);
            blueFire = false;
            pickBreak = false;

            familiarPet = false;
            omegaStaritePet = false;

            forceDaytime = 0;
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
            PostUpdate_CheckDanger();
        }
        private void PostUpdate_CheckDanger()
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

        private float NPCDamageMultiplier(NPC target, int damage, float knockback, bool crit)
        {
            float multiplier = 1f;
            var stat = this.GetStat<FocusCrystalStat>();
            if (stat.effectCircumference > 0f && Player.Distance(target.getRect().ClosestDistance(Player.Center)) < (stat.effectCircumference / 2f))
            {
                multiplier += stat.damageMultiplier;
            }
            return multiplier;
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            float multiplier = NPCDamageMultiplier(target, damage, knockback, crit);
            if (multiplier != 1f)
            {
                damage = (int)(damage * multiplier);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float multiplier = NPCDamageMultiplier(target, damage, knockback, crit);
            if (multiplier != 1f)
            {
                damage = (int)(damage * multiplier);
            }
        }

        public void PreDrawAllPlayers(LegacyPlayerRenderer playerRenderer, Camera camera, IEnumerable<Player> players)
        {
            if (Main.gameMenu)
            {
                return;
            }
            bool end = false;
            var stat = this.GetStat<FocusCrystalStat>();
            if (stat._accFocusCrystalCircumference > 0f && !stat.hideVisual)
            {
                if (inDanger)
                {
                    stat._accFocusCrystalOpacity = MathHelper.Lerp(stat._accFocusCrystalOpacity, 1f, 0.1f);
                }
                else
                {
                    stat._accFocusCrystalOpacity = MathHelper.Lerp(stat._accFocusCrystalOpacity, 0.2f, 0.1f);
                }

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                end = true;
                var texture = PlayerAssets.FocusAura.Value;
                var origin = texture.Size() / 2f;
                var drawCoords = (Player.Center - Main.screenPosition).Floor();
                float scale = stat._accFocusCrystalCircumference / texture.Width;
                float opacity = Math.Min(stat._accFocusCrystalOpacity * scale, 1f);

                Main.spriteBatch.Draw(texture, drawCoords, null,
                    new Color(60, 4, 4, 0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
                texture = PlayerAssets.FocusCircle.Value;

                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(texture, drawCoords + (MathHelper.PiOver4 * i).ToRotationVector2() * 2f * scale, null,
                        new Color(80, 6, 6, 0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
                }

                for (int i = 0; i < 4; i++)
                {
                    Main.spriteBatch.Draw(texture, drawCoords + (MathHelper.PiOver2 * i).ToRotationVector2() * scale, null,
                        new Color(128, 6, 6, 0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
                }

                Main.spriteBatch.Draw(texture, drawCoords, null,
                    new Color(128, 10, 10, 0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            else
            {
                stat._accFocusCrystalOpacity = 0f;
            }
            if (end)
            {
                Main.spriteBatch.End();
            }
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
        public void SetCooldown(int cooldown, bool ignoreStats = false, Item itemReference = null)
        {
            if (cooldown < itemCooldown)
            {
                return;
            }

            itemCooldownMax = (ushort)cooldown;
            itemCooldown = (ushort)cooldown;
        }
    }
}