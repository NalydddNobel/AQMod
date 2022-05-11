using Aequus.Common.Catalogues;
using Aequus.Common.Players;
using Aequus.Content.Invasions;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus
{
    public class AequusPlayer : ModPlayer
    {
        public static int teamContext;
        public static float? PlayerDrawScale { get; set; }
        public static int? PlayerDrawForceDye { get; set; }

        private static MethodInfo Player_ItemCheck_Shoot;

        /// <summary>
        /// Enabled by <see cref="Items.Consumables.Moro"/>
        /// </summary>
        public bool permMoro;

        /// <summary>
        /// Applied by <see cref="Buffs.Debuffs.BlueFire"/>
        /// </summary>
        public bool blueFire;
        /// <summary>
        /// Applied by <see cref="Buffs.Debuffs.PickBreak"/>
        /// </summary>
        public bool pickBreak;

        /// <summary>
        /// Applied by <see cref="Buffs.Pets.SpaceSquidBuff"/>
        /// </summary>
        public bool spaceSquidPet;
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
        /// Whether or not the player is in the Gale Streams event. Updated using <see cref="CheckEventGaleStreams"/> in <see cref="PreUpdate"/>
        /// </summary>
        public bool eventGaleStreams;

        /// <summary>
        /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/> / <see cref="PostUpdate_CheckDanger"/>
        /// </summary>
        public int closestEnemy;
        public int closestEnemyOld;

        public bool dreamMask;
        /// <summary>
        /// Applied by <see cref="RitualisticSkull"/>
        /// </summary>
        public bool necromancyMinionSlotConvert;
        /// <summary>
        /// Applied by <see cref="SentrySquid"/>
        /// </summary>
        public bool autoSentry;
        public ushort autoSentryCooldown;
        /// <summary>
        /// Used by <see cref="IcebergKraken"/>. Gives all sentries and their projectiles a 1/6 chance to inflict the Frostburn debuff
        /// </summary>
        public bool frostburnSentry;
        /// <summary>
        /// Used by <see cref="GlowCore"/>. All player owned projectiles also check this in order to decide if they should glow.
        /// </summary>
        public byte glowCore;
        /// <summary>
        /// Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.
        /// <para>Used by <see cref="GrandReward"/></para> 
        /// </summary>
        public float lootLuck;
        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Used by <see cref="Buffs.NoonBuff"/> and set to 1</para>
        /// </summary>
        public byte forceDaytime;

        /// <summary>
        /// Set to true by <see cref="Items.Armor.PassiveSummon.DartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.SuperDartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.FlowerCrown"/>
        /// </summary>
        public bool wearingSummonHelmet;
        /// <summary>
        /// Used by summon helmets (<see cref="Items.Armor.PassiveSummon.DartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.SuperDartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.FlowerCrown"/>) to time projectile spawns and such.
        /// </summary>
        public int summonHelmetTimer;

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
        public int revenantScepterZombie;

        public int turretSlotCount;
        public int necromancySlotUsed;
        public int necromancySlots;
        public int necromancyTime;

        /// <summary>
        /// Helper for whether or not the player currently has a cooldown
        /// </summary>
        public bool HasCooldown => itemCooldown > 0;
        /// <summary>
        /// Helper for whether or not the player is in danger
        /// </summary>
        public bool InDanger => closestEnemy != -1;

        public override void Load()
        {
            LoadHooks();
            Player_ItemCheck_Shoot = typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        private void LoadHooks()
        {
            On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.DrawPlayers += LegacyPlayerRenderer_DrawPlayers;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += OnRenderPlayer;
        }

        public override void Unload()
        {
            Player_ItemCheck_Shoot = null;
        }

        public override void Initialize()
        {
            permMoro = false;

            itemCooldown = 0;
            itemCooldownMax = 0;
            itemCombo = 0;
            itemSwitch = 0;
            interactionCooldown = 60;
            closestEnemyOld = -1;
            closestEnemy = -1;
            autoSentryCooldown = 120;
        }

        public override void PreUpdate()
        {
            if (forceDaytime == 1)
            {
                AequusHelpers.Main_dayTime.StartCaching(true);
            }
            else if (forceDaytime == 2)
            {
                AequusHelpers.Main_dayTime.StartCaching(false);
            }

            eventGaleStreams = CheckEventGaleStreams();
            forceDaytime = 0;
        }
        /// <summary>
        /// Used to update <see cref="eventGaleStreams"/>
        /// </summary>
        /// <returns>Whether the Gale Streams event is currently active, and the player is in space</returns>
        public bool CheckEventGaleStreams()
        {
            return GaleStreams.Status == InvasionStatus.Active && GaleStreams.IsThisSpace(Player);
        }

        public override void UpdateDead()
        {
            autoSentry = false;
            autoSentryCooldown = 120;
        }

        public override void ResetEffects()
        {
            dreamMask = false;
            necromancyMinionSlotConvert = false;
            frostburnSentry = false;
            teamContext = Player.team;
            blueFire = false;
            pickBreak = false;

            spaceSquidPet = false;
            familiarPet = false;
            omegaStaritePet = false;

            resistHeat = false;

            autoSentry = false;
            glowCore = 0;
            forceDaytime = 0;
            lootLuck = 0f;
            necromancySlots = 1;
            necromancyTime = 3600;
        }

        public override void PreUpdateBuffs()
        {
            if (!InDanger)
            {
                autoSentryCooldown = Math.Min(autoSentryCooldown, (ushort)240);
            }
            AequusHelpers.TickDown(ref autoSentryCooldown);
        }

        public override bool PreItemCheck()
        {
            if (AequusHelpers.Main_dayTime.IsCaching)
                AequusHelpers.Main_dayTime.RepairCachedStatic();
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
            if (AequusHelpers.Main_dayTime.IsCaching)
                AequusHelpers.Main_dayTime.DisrepairCachedStatic();
            if (Player.selectedItem != lastSelectedItem)
            {
                lastSelectedItem = Player.selectedItem;
                itemSwitch = 30;
                itemUsage = 0;
            }
        }

        public override void PostUpdateEquips()
        {
            if (glowCore > 0)
            {
                GlowCore.AddLight(Player, glowCore);
            }
            if (necromancyMinionSlotConvert)
            {
                necromancySlots += Player.maxMinions - 1;
                Player.maxMinions = 1;
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                UpdateZombies();
            }
            necromancySlotUsed = 0;
        }
        public void UpdateZombies()
        {
            if (necromancySlotUsed > necromancySlots)
            {
                int removeNPC = -1;
                int oldestTime = int.MaxValue;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].GetGlobalNPC<NecromancyNPC>().isZombie && Main.npc[i].GetGlobalNPC<NecromancyNPC>().zombieOwner == Player.whoAmI)
                    {
                        var stats = NecromancyDatabase.GetByNetID(Main.npc[i]);
                        if (stats.SlotsUsed == null || stats.SlotsUsed > 0)
                        {
                            var zombie = Main.npc[i].GetGlobalNPC<NecromancyNPC>();
                            int timeComparison = GetDespawnComparison(Main.npc[i], zombie); // Prioritize to kill lower tier slaves
                            if (timeComparison < oldestTime)
                            {
                                removeNPC = i;
                                oldestTime = timeComparison;
                            }
                        }
                    }
                }
                if (removeNPC != -1)
                {
                    Main.npc[removeNPC].life = -1;
                    Main.npc[removeNPC].HitEffect();
                    Main.npc[removeNPC].active = false;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, removeNPC, 9999);
#pragma warning restore CS0618 // Type or member is obsolete

                        Aequus.Instance.Logger.Debug("NPC: " + Lang.GetNPCName(Main.npc[removeNPC].type) + ", WhoAmI: " + removeNPC + ", Tier:" + Main.npc[removeNPC].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier);
                    }
                }
            }
        }
        public int GetDespawnComparison(NPC npc, NecromancyNPC zombie)
        {
            float tiering = 999f;
            if (NecromancyDatabase.TryGetByNetID(npc, out var value))
            {
                tiering = value.PowerNeeded;
            }
            if (npc.boss)
            {
                tiering += 10f;
            }
            if (npc.noGravity)
            {
                tiering *= 2f;
            }
            return (int)(zombie.zombieTimer * tiering) + npc.lifeMax + npc.damage * 3 + npc.defense * 2;
        }

        public override void PostUpdate()
        {
            if (AequusHelpers.Main_dayTime.IsCaching)
            {
                AequusHelpers.Main_dayTime.EndCaching();
            }
            PostUpdate_CheckDanger();
            if (autoSentry && autoSentryCooldown == 0)
            {
                UpdateAutoSentry();
            }
            teamContext = 0;
        }

        /// <summary>
        /// Finds the closest enemy to the player, and caches its index in <see cref="Main.npc"/>
        /// </summary>
        public void PostUpdate_CheckDanger()
        {
            closestEnemyOld = closestEnemy;
            closestEnemy = -1;

            var center = Player.Center;
            var checkTangle = new Rectangle((int)Player.position.X + Player.width / 2 - 1000, (int)Player.position.Y + Player.height / 2 - 500, 2000, 1000);
            float distance = 2000f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy && Main.npc[i].CanBeChasedBy(Player) && !Main.npc[i].IsProbablyACritter())
                {
                    if (Main.npc[i].getRect().Intersects(checkTangle))
                    {
                        float d = Main.npc[i].Distance(center);
                        if (d < distance)
                        {
                            distance = d;
                            closestEnemy = i;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to place a sentry down near the <see cref="NPC"/> at <see cref="closestEnemy"/>'s index. Doesn't do anything if the index is -1, the enemy is not active, or the player has no turret slots. Runs after <see cref="PostUpdate_CheckDanger"/>
        /// </summary>
        public void UpdateAutoSentry()
        {
            if (closestEnemy == -1 || !Main.npc[closestEnemy].active || Player.maxTurrets <= 0)
            {
                autoSentryCooldown = 30;
                return;
            }

            var item = AutoSentry_GetUsableSentryStaff();
            if (item == null)
            {
                autoSentryCooldown = 30;
                return;
            }

            CountSentries();
            if (turretSlotCount >= Player.maxTurrets)
            {
                int oldestSentry = -1;
                int time = int.MaxValue;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].WipableTurret)
                    {
                        if (Main.projectile[i].timeLeft < time)
                        {
                            oldestSentry = i;
                            time = Main.projectile[i].timeLeft;
                        }
                    }
                }
                if (oldestSentry != -1)
                {
                    Main.projectile[oldestSentry].timeLeft = Math.Min(Main.projectile[oldestSentry].timeLeft, 30);
                }
                autoSentryCooldown = 30;
                return;
            }

            if (!ItemsCatalogue.SentryUsage.TryGetValue(item.type, out var sentryUsage))
            {
                sentryUsage = ItemsCatalogue.SentryStaffUsage.Default;
            }
            if (sentryUsage.TrySummoningThisSentry(Player, item, Main.npc[closestEnemy]))
            {
                Player.UpdateMaxTurrets();
                if (Player.maxTurrets > 1)
                {
                    autoSentryCooldown = 240;
                }
                else
                {
                    autoSentryCooldown = 3000;
                }
                if (Main.netMode != NetmodeID.Server && item.UseSound != null)
                {
                    SoundEngine.PlaySound(item.UseSound, Main.npc[closestEnemy].Center);
                }
            }
            else
            {
                autoSentryCooldown = 30;
            }
        }
        /// <summary>
        /// Determines an item to use as a Sentry Staff for <see cref="UpdateAutoSentry"/>
        /// </summary>
        /// <returns></returns>
        public Item AutoSentry_GetUsableSentryStaff()
        {
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                // A very small check which doesn't care about checking damage and such, so this could be easily manipulated.
                if (!Player.inventory[i].IsAir && Player.inventory[i].sentry && Player.inventory[i].shoot > ProjectileID.None && (!Player.inventory[i].DD2Summon || !DD2Event.Ongoing)
                    && ItemLoader.CanUseItem(Player.inventory[i], Player))
                {
                    return Player.inventory[i];
                }
            }
            return null;
        }

        public override void ModifyScreenPosition()
        {
            ModContent.GetInstance<GameCamera>().UpdateScreen();
            EffectsSystem.UpdateScreenPosition();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (resistHeat && HeatDamageTypes.HeatNPC.Contains(npc.netID))
            {
                damage = (int)(damage * 0.7f);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (resistHeat && HeatDamageTypes.HeatProjectile.Contains(proj.type))
            {
                damage = (int)(damage * 0.7f);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Moro"] = permMoro;
        }

        public override void LoadData(TagCompound tag)
        {
            permMoro = tag.GetBool("Moro");
        }

        public void PreDrawAllPlayers(LegacyPlayerRenderer playerRenderer, Camera camera, IEnumerable<Player> players)
        {
            if (Main.gameMenu)
            {
                return;
            }
            RenderMendshroomAura();
            RenderFocusCrystalAura();
        }
        public void RenderMendshroomAura()
        {
            var stat = Player.GetModPlayer<MendshroomPlayer>();
            if (stat._circumferenceForVFX > 0f)
            {
                DrawBasicAura(stat._circumferenceForVFX, 1f, new Color(10, 128, 10, 0));
            }
        }
        public void RenderFocusCrystalAura()
        {
            var hyperCrystal = Player.GetModPlayer<HyperCrystalPlayer>();
            if (hyperCrystal._accFocusCrystalCircumference > 0f && !hyperCrystal.hideVisual)
            {
                if (InDanger)
                {
                    hyperCrystal._accFocusCrystalOpacity = MathHelper.Lerp(hyperCrystal._accFocusCrystalOpacity, 1f, 0.1f);
                }
                else
                {
                    hyperCrystal._accFocusCrystalOpacity = MathHelper.Lerp(hyperCrystal._accFocusCrystalOpacity, 0.2f, 0.1f);
                }

                DrawBasicAura(hyperCrystal._accFocusCrystalCircumference, hyperCrystal._accFocusCrystalOpacity, new Color(128, 10, 10, 0));
            }
            else
            {
                hyperCrystal._accFocusCrystalOpacity = 0f;
            }
        }
        public void DrawBasicAura(float circumference, float opacity, Color color)
        {
            BeginSpriteBatch(Main.spriteBatch);

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

            Main.spriteBatch.End();
        }
        private void BeginSpriteBatch(SpriteBatch spriteBatch)
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
        /// <param name="itemReference"></param>
        public void SetCooldown(int cooldown, bool ignoreStats = false, Item itemReference = null)
        {
            if (cooldown < itemCooldown)
            {
                return;
            }

            itemCooldownMax = (ushort)cooldown;
            itemCooldown = (ushort)cooldown;
        }

        public void CountSentries()
        {
            turretSlotCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].WipableTurret)
                {
                    turretSlotCount++;
                }
            }
        }

        public static void ShootProj(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 location, Vector2 velocity, int projType, int projDamage, float projKB, Vector2? setMousePos)
        {
            if (Player_ItemCheck_Shoot != null)
            {
                int mouseX = Main.mouseX;
                int mouseY = Main.mouseY;

                if (setMousePos != null)
                {
                    var mousePos = setMousePos.Value - Main.screenPosition;
                    Main.mouseX = (int)mousePos.X;
                    Main.mouseX = (int)mousePos.Y;
                }

                Player_ItemCheck_Shoot.Invoke(player, new object[] { player.whoAmI, item, player.GetWeaponDamage(item), });

                Main.mouseX = mouseX;
                Main.mouseY = mouseY;
                return;
            }

            LegacySudoShootProj(player, item, source, location, velocity, projType, projDamage, projKB, setMousePos);
        }
        private static int LegacySudoShootProj(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 location, Vector2 velocity, int projType, int projDamage, float projKB, Vector2? setMousePos)
        {
            int mouseX = Main.mouseX;
            int mouseY = Main.mouseY;

            if (source == null)
            {
                source = new EntitySource_ItemUse_WithAmmo(player, item, 0);
            }

            if (setMousePos != null)
            {
                var mousePos = setMousePos.Value - Main.screenPosition;
                Main.mouseX = (int)mousePos.X;
                Main.mouseX = (int)mousePos.Y;
            }

            CombinedHooks.ModifyShootStats(player, item, ref location, ref velocity, ref projType, ref projDamage, ref projKB);

            int result;
            if (CombinedHooks.Shoot(player, item, source, location, velocity, projType, projDamage, projKB))
            {
                result = Projectile.NewProjectile(source, location, velocity, projType, projDamage, projKB, player.whoAmI);
            }
            else
            {
                result = -2;
            }

            Main.mouseX = mouseX;
            Main.mouseY = mouseY;
            return result;
        }

        private static void LegacyPlayerRenderer_DrawPlayers(On.Terraria.Graphics.Renderers.LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players)
        {
            var aequusPlayers = new List<AequusPlayer>();
            foreach (var p in players)
            {
                aequusPlayers.Add(p.GetModPlayer<AequusPlayer>());
            }
            foreach (var aequus in aequusPlayers)
            {
                aequus.PreDrawAllPlayers(self, camera, players);
            }
            orig(self, camera, players);
            //foreach (var p in active)
            //{
            //    p.PostDrawAllPlayers(self);
            //}
        }

        private static void OnRenderPlayer(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
        {
            if (PlayerDrawScale != null)
            {
                var drawPlayer = drawinfo.drawPlayer;
                var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
                to -= Main.screenPosition;
                for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
                {
                    DrawData data = drawinfo.DrawDataCache[i];
                    data.position -= (data.position - to) * (1f - PlayerDrawScale.Value);
                    data.scale *= PlayerDrawScale.Value;
                    drawinfo.DrawDataCache[i] = data;
                }
            }
            if (PlayerDrawForceDye != null)
            {
                var drawPlayer = drawinfo.drawPlayer;
                for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
                {
                    DrawData data = drawinfo.DrawDataCache[i];
                    data.shader = PlayerDrawForceDye.Value;
                    drawinfo.DrawDataCache[i] = data;
                }
            }
            drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PreDraw(ref drawinfo);
            orig(ref drawinfo);
            drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PostDraw(ref drawinfo);
        }
    }
}