using Aequus.Common;
using Aequus.Common.Catalogues;
using Aequus.Common.Players;
using Aequus.Common.Players.StatData;
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
        /// Applied by <see cref="Buffs.FrostBuff"/>
        /// </summary>
        public bool resistHeat;

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
        /// <para>Used by <see cref="Buffs.NoonBuff"/> and set to 1</para>
        /// </summary>
        public byte forceDaytime;
        /// <summary>
        /// Used to increase droprates.
        /// <para>Used by <see cref="Items.Accessories.GrandReward"/></para> 
        /// </summary>
        public float lootLuck;

        /// <summary>
        /// Tracks <see cref="Player.selectedItem"/>, updated in <see cref="PostItemCheck"/>
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
        /// Helper for whether or not the player currently has a cooldown.
        /// </summary>
        public bool HasCooldown => itemCooldown > 0;

        public override void Load()
        {
            LoadHooks();
        }

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
            eventGaleStreams = CheckEventGaleStreams();
            forceDaytime = 0;
        }
        private bool CheckEventGaleStreams()
        {
            return GaleStreams.Status == InvasionStatus.Active && GaleStreams.IsThisSpace(Player);
        }

        public override void ResetEffects()
        {
            blueFire = false;
            pickBreak = false;

            familiarPet = false;
            omegaStaritePet = false;

            resistHeat = false;

            forceDaytime = 0;
            lootLuck = 0f;
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

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (resistHeat && HeatDamageCatalogue.HeatNPC.Contains(npc.netID))
            {
                damage = (int)(damage * 0.7f);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (resistHeat && HeatDamageCatalogue.HeatProjectile.Contains(proj.type))
            {
                damage = (int)(damage * 0.7f);
            }
        }

        public void PreDrawAllPlayers(LegacyPlayerRenderer playerRenderer, Camera camera, IEnumerable<Player> players)
        {
            if (Main.gameMenu)
            {
                return;
            }
            RenderBungusAura();
            RenderFocusCrystalAura();
        }
        private void RenderBungusAura()
        {
            var stat = Player.GetModPlayer<MendshroomPlayer>();
            if (stat._circumferenceForVFX > 0f)
            {
                HelpBeginSpriteBatch(Main.spriteBatch);
                HelpDrawAura(stat._circumferenceForVFX, 1f, new Color(10, 128, 10, 0));
                Main.spriteBatch.End();
            }
        }
        private void RenderFocusCrystalAura()
        {
            var hyperCrystal = Player.GetModPlayer<HyperCrystalPlayer>();
            if (hyperCrystal._accFocusCrystalCircumference > 0f && !hyperCrystal.hideVisual)
            {
                if (inDanger)
                {
                    hyperCrystal._accFocusCrystalOpacity = MathHelper.Lerp(hyperCrystal._accFocusCrystalOpacity, 1f, 0.1f);
                }
                else
                {
                    hyperCrystal._accFocusCrystalOpacity = MathHelper.Lerp(hyperCrystal._accFocusCrystalOpacity, 0.2f, 0.1f);
                }

                HelpBeginSpriteBatch(Main.spriteBatch);
                HelpDrawAura(hyperCrystal._accFocusCrystalCircumference, hyperCrystal._accFocusCrystalOpacity, new Color(128, 10, 10, 0));
                Main.spriteBatch.End();
            }
            else
            {
                hyperCrystal._accFocusCrystalOpacity = 0f;
            }
        }
        private void HelpDrawAura(float circumference, float opacity, Color color)
        {
            var texture = PlayerAssets.FocusAura.Value;
            var origin = texture.Size() / 2f;
            var drawCoords = (Player.Center - Main.screenPosition).Floor();
            float scale = circumference / texture.Width;
            opacity = Math.Min(opacity * scale, 1f);

            Main.spriteBatch.Draw(texture, drawCoords, null,
                color * 0.5f * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            texture = PlayerAssets.FocusCircle.Value;

            foreach (var v in AequusHelpers.CircularVector(8))
            {
                Main.spriteBatch.Draw(texture, drawCoords + v * 2f * scale, null,
                    color * 0.66f * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            foreach (var v in AequusHelpers.CircularVector(4))
            {
                Main.spriteBatch.Draw(texture, drawCoords + v * scale, null,
                    color * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawCoords, null,
                color * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
        }
        private void HelpBeginSpriteBatch(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
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