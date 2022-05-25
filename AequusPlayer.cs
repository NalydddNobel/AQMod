using Aequus.Biomes;
using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Pets;
using Aequus.Common;
using Aequus.Common.Catalogues;
using Aequus.Common.Networking;
using Aequus.Common.Players;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Misc.Money;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
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
        public struct LifeSacrifice
        {
            public int time;
            public int amtTaken;
            public bool physicallyHitPlayer = false;
            public PlayerDeathReason reason = null;

            public LifeSacrifice(int amtTaken, int time = 0, bool hitPlayer = false, PlayerDeathReason reason = null)
            {
                this.amtTaken = amtTaken;
                this.time = time;
                physicallyHitPlayer = hitPlayer;
                this.reason = reason;
            }
        }

        public static int TeamContext;
        public static float? PlayerDrawScale;
        public static int? PlayerDrawForceDye;

        private static MethodInfo Player_ItemCheck_Shoot;

        public int projectileIdentity = -1;

        [SaveData("Scammer")]
        public bool permHasScammed;
        /// <summary>
        /// Enabled by <see cref="Items.Consumables.Moro"/>
        /// </summary>
        [SaveData("Moro")]
        public bool permMoro;

        /// <summary>
        /// Applied by <see cref="BlueFire"/>
        /// </summary>
        public bool debuffBlueFire;
        /// <summary>
        /// Applied by <see cref="PickBreak"/>
        /// </summary>
        public bool debuffPickBreak;

        /// <summary>
        /// Applied by <see cref="SpicyEelBuff"/>
        /// </summary>
        public bool buffSpicyEel;
        /// <summary>
        /// Applied by <see cref="FrostBuff"/>
        /// </summary>
        public bool buffResistHeat;

        /// <summary>
        /// Applied by <see cref="SpaceSquidBuff"/>
        /// </summary>
        public bool spaceSquidPet;
        /// <summary>
        /// Applied by <see cref="FamiliarBuff"/>
        /// </summary>
        public bool familiarPet;
        /// <summary>
        /// Applied by <see cref="OmegaStariteBuff"/>
        /// </summary>
        public bool omegaStaritePet;

        /// <summary>
        /// Whether or not the player is in the Gale Streams event. Updated using <see cref="CheckEventGaleStreams"/> in <see cref="PreUpdate"/>
        /// </summary>
        public bool eventGaleStreams;
        /// <summary>
        /// A point determining one of the close gore nests. Goes by on-spawn order.
        /// </summary>
        public Point eventDemonSiege;

        /// <summary>
        /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/> -> <see cref="CheckDanger"/>
        /// </summary>
        public int closestEnemy;
        public int closestEnemyOld;

        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Used by <see cref="Buffs.NoonBuff"/> and set to 1</para>
        /// </summary>
        public byte forceDaytime;

        /// <summary>
        /// A percentage chance for a successful scam, where you don't consume money. Values below or equal 0 mean no scams, Values above or equal 1 mean 100% scam rate. Used by <see cref="FaultyCoin"/>
        /// </summary>
        public float scamChance;
        /// <summary>
        /// A flat discount variable. Decreases shop prices by this amount. Used by <see cref="ForgedCard"/>
        /// </summary>
        public int flatScamDiscount;
        /// <summary>
        /// Rerolls luck (rounded down amt of luckRerolls) times, if there is a decimal left, then it has a (luckRerolls decimal) chance of rerolling again.
        /// <para>Used by <see cref="RabbitsFoot"/></para> 
        /// </summary>
        public float luckRerolls;
        /// <summary>
        /// Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.
        /// <para>Used by <see cref="GrandReward"/></para> 
        /// </summary>
        public float lootLuck;

        /// <summary>
        /// Applied by <see cref="SantankSentry"/>
        /// </summary>
        public bool accInheritTurrets;
        /// <summary>
        /// Applied by <see cref="FoolsGoldRing"/>
        /// </summary>
        public bool accFoolsGoldRing;
        /// <summary>
        /// Applied by <see cref="RitualisticSkull"/>
        /// </summary>
        public bool accMinionsToGhosts;
        /// <summary>
        /// Applied by <see cref="SentrySquid"/>
        /// </summary>
        public bool accAutoSentry;
        public ushort autoSentryCooldown;
        /// <summary>
        /// Gives all sentries and their projectiles a 1/6 chance to inflict the Frostburn debuff. Applied by <see cref="IcebergKraken"/>
        /// </summary>
        public bool accFrostburnSentry;
        /// <summary>
        /// All player owned projectiles also check this in order to decide if they should glow. Applied by <see cref="GlowCore"/>
        /// </summary>
        public byte accGlowCore;

        /// <summary>
        /// Set to true by <see cref="Items.Armor.PassiveSummon.DartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.SuperDartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.FlowerCrown"/>
        /// </summary>
        public bool wearingSummonHelmet;
        /// <summary>
        /// Used by summon helmets (<see cref="Items.Armor.PassiveSummon.DartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.SuperDartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.FlowerCrown"/>) to time projectile spawns and such.
        /// </summary>
        public int summonHelmetTimer;

        public bool hasSkeletonKey;
        public bool hasShadowKey;

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

        public int turretSlotCount;

        public int ghostSlotsMax;
        public int ghostSlotsOld;
        public int ghostSlots;

        public int ghostLifespan;

        public int hitTime;

        public List<LifeSacrifice> sacrifices;

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

        #region Hooks
        private static void LoadHooks()
        {
            On.Terraria.Player.RollLuck += Player_RollLuck;
            On.Terraria.Player.DropCoins += Player_DropCoins;
            On.Terraria.Player.GetItemExpectedPrice += Player_GetItemExpectedPrice;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += OnRenderPlayer;
        }

        private static int Player_RollLuck(On.Terraria.Player.orig_RollLuck orig, Player self, int range)
        {
            int rolled = orig(self, range);
            if (AequusHelpers.iterations == 0)
            {
                AequusHelpers.iterations++;
                try
                {
                    rolled = self.Aequus().RerollLuck(rolled, range);
                }
                catch
                {
                }
                AequusHelpers.iterations = 0;
            }
            return rolled;
        }
        public int RerollLuck(int rolledAmt, int range)
        {
            for (float luckLeft = lootLuck; luckLeft > 0f; luckLeft--)
            {
                if (luckLeft < 1f)
                {
                    if (Main.rand.NextFloat(1f) > luckLeft)
                    {
                        return rolledAmt;
                    }
                }
                int roll = Player.RollLuck(range);
                if (roll < rolledAmt)
                {
                    rolledAmt = roll;
                }
                if (rolledAmt <= 0)
                {
                    return 0;
                }
            }
            return rolledAmt;
        }

        private static int Player_DropCoins(On.Terraria.Player.orig_DropCoins orig, Player self)
        {
            if (self.Aequus().accFoolsGoldRing)
            {
                return FoolsGoldCoinCurse(self);
            }
            return orig(self);
        }
        public static int FoolsGoldCoinCurse(Player player)
        {
            for (int i = 0; i < 59; i++)
            {
                if (player.inventory[i].IsACoin)
                {
                    player.inventory[i].TurnToAir();
                }
                if (i == 58)
                {
                    Main.mouseItem = player.inventory[i].Clone();
                }
            }
            player.lostCoins = 0;
            player.lostCoinString = "";
            return 0;
        }

        private static void Player_GetItemExpectedPrice(On.Terraria.Player.orig_GetItemExpectedPrice orig, Player self, Item item, out int calcForSelling, out int calcForBuying)
        {
            orig(self, item, out calcForSelling, out calcForBuying);
            if (item.shopSpecialCurrency != -1)
            {
                return;
            }
            calcForBuying -= self.Aequus().flatScamDiscount;
        }

        private static void OnRenderPlayer(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
        {
            AdjustPlayerRender(PlayerDrawScale, PlayerDrawForceDye, ref drawinfo);

            drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PreDraw(ref drawinfo);
            orig(ref drawinfo);
            drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PostDraw(ref drawinfo);
        }
        public static void AdjustPlayerRender(float? drawScale, int? drawForceDye, ref PlayerDrawSet drawinfo)
        {
            if (drawScale != null)
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
            if (drawForceDye != null)
            {
                var drawPlayer = drawinfo.drawPlayer;
                for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
                {
                    DrawData data = drawinfo.DrawDataCache[i];
                    data.shader = PlayerDrawForceDye.Value;
                    drawinfo.DrawDataCache[i] = data;
                }
            }
        }
        #endregion

        public override void Unload()
        {
            Player_ItemCheck_Shoot = null;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (AequusPlayer)clientClone;
            clone.itemCombo = itemCombo;
            clone.itemSwitch = itemSwitch;
            clone.itemUsage = itemUsage;
            clone.itemCooldown = itemCooldown;
            clone.itemCooldownMax = itemCooldownMax;
            clone.hitTime = hitTime;
            clone.sacrifices = new List<LifeSacrifice>();
            foreach (var l in sacrifices)
            {
                clone.sacrifices.Add(l);
            }
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (AequusPlayer)clientPlayer;

            bool itemSync = (clone.itemCombo - itemCombo).Abs() > 10 ||
                (clone.itemSwitch - itemSwitch).Abs() > 10 ||
                (clone.itemUsage - itemUsage).Abs() > 10 ||
                (clone.itemCooldown - itemCooldown).Abs() > 10 ||
                clone.itemCooldownMax != itemCooldownMax;
            bool syncHitTime = (clone.hitTime - hitTime).Abs() > 10;

            if (itemSync || syncHitTime)
            {
                Sync(itemSync, syncHitTime);
            }
        }
        public void Sync(bool itemSync, bool syncHitTime)
        {
            PacketSender.Send((p) =>
            {
                if (itemSync)
                {
                    p.Write(true);
                    p.Write(itemCombo);
                    p.Write(itemSwitch);
                    p.Write(itemUsage);
                    p.Write(itemCooldown);
                    p.Write(itemCooldownMax);
                }
                else
                {
                    p.Write(false);
                }
            }, PacketType.SyncAequusPlayer);

        }

        public void RecieveChanges(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                itemCombo = reader.ReadUInt16();
                itemSwitch = reader.ReadUInt16();
                itemUsage = reader.ReadUInt16();
                itemCooldown = reader.ReadUInt16();
                itemCooldownMax = reader.ReadUInt16();
            }
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

            sacrifices = new List<LifeSacrifice>();
        }

        public override void PreUpdate()
        {
            projectileIdentity = -1;
            if (forceDaytime == 1)
            {
                AequusHelpers.Main_dayTime.StartCaching(true);
            }
            else if (forceDaytime == 2)
            {
                AequusHelpers.Main_dayTime.StartCaching(false);
            }

            eventGaleStreams = CheckEventGaleStreams();
            eventDemonSiege = FindDemonSiege();
            forceDaytime = 0;
        }
        /// <summary>
        /// Used to update <see cref="eventGaleStreams"/>
        /// </summary>
        /// <returns>Whether the Gale Streams event is currently active, and the player is in space</returns>
        public bool CheckEventGaleStreams()
        {
            return GaleStreamsInvasion.Status == InvasionStatus.Active && GaleStreamsInvasion.IsThisSpace(Player);
        }
        public Point FindDemonSiege()
        {
            foreach (var s in DemonSiegeInvasion.Sacrifices)
            {
                if (Player.Distance(new Vector2(s.Value.TileX * 16f + 24f, s.Value.TileY * 16f)) < s.Value.Range)
                {
                    return s.Key;
                }
            }
            return Point.Zero;
        }

        public override void UpdateDead()
        {
            hitTime = 0;
            accAutoSentry = false;
            autoSentryCooldown = 120;
            sacrifices.Clear();
        }

        public override void ResetEffects()
        {
            UpdateCooldowns();
            scamChance = 0f;
            flatScamDiscount = 0;

            accInheritTurrets = false;
            accFoolsGoldRing = false;
            accMinionsToGhosts = false;
            accFrostburnSentry = false;
            TeamContext = Player.team;

            buffSpicyEel = false;
            buffResistHeat = false;

            debuffBlueFire = false;
            debuffPickBreak = false;

            spaceSquidPet = false;
            familiarPet = false;
            omegaStaritePet = false;

            hasSkeletonKey = false;
            hasShadowKey = false;

            accAutoSentry = false;
            accGlowCore = 0;
            forceDaytime = 0;
            lootLuck = 0f;
            ghostSlotsMax = 1;
            ghostLifespan = 3600;
        }
        public void UpdateCooldowns()
        {
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
        }

        public override void PreUpdateBuffs()
        {
            if (!InDanger)
            {
                autoSentryCooldown = Math.Min(autoSentryCooldown, (ushort)240);
            }
            AequusHelpers.TickDown(ref autoSentryCooldown);
            hitTime++;
        }

        public override bool PreItemCheck()
        {
            if (AequusHelpers.Main_dayTime.IsCaching)
                AequusHelpers.Main_dayTime.RepairCachedStatic();
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
            UpdateBank(Player.bank, 0);
            UpdateBank(Player.bank2, 1);
            UpdateBank(Player.bank3, 2);
            UpdateBank(Player.bank4, 3);
            if (accGlowCore > 0)
            {
                GlowCore.AddLight(Player, accGlowCore);
            }
            if (accMinionsToGhosts)
            {
                ghostSlotsMax += Player.maxMinions - 1;
                Player.maxMinions = 1;
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                UpdateZombies();
            }
            ghostSlotsOld = ghostSlots;
            ghostSlots = 0;
            UpdateSacrifice();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="bankType">Types: 
        /// <list type="number">
        /// Piggy Bank
        /// <item>Safe</item>
        /// <item>Defender's Forge</item>
        /// <item>Void Bag</item>
        /// </list></param>
        public void UpdateBank(Chest bank, int bankType)
        {
            for (int i = 0; i < bank.item.Length; i++)
            {
                if (bank.item[i] != null && !bank.item[i].IsAir)
                {
                    bool update = false;
                    if (bank.item[i].type == ItemID.ShadowKey)
                    {
                        update = true;
                        hasShadowKey = true;
                    }
                    else if (bank.item[i].type == ItemID.DiscountCard && !Player.discount)
                    {
                        update = true;
                    }
                    else if (AequusItem.BankEquipFuncs.Contains(bank.item[i].type))
                    {
                        update = true;
                    }
                    else if (bank.item[i].ModItem is IUpdateBank b)
                    {
                        b.UpdateBank(Player, this, i, bankType);
                    }

                    if (update)
                    {
                        Player.VanillaUpdateEquip(bank.item[i]);
                        Player.ApplyEquipFunctional(bank.item[i], true); // Acts as a hidden accessory while in the bank.
                    }
                }
            }
        }
        public void UpdateSacrifice()
        {
            for (int i = 0; i < sacrifices.Count; i++)
            {
                var s = sacrifices[i];
                s.time--;
                if (s.time <= 0)
                {
                    var reason = s.reason ?? PlayerDeathReason.ByOther(4);
                    if (s.physicallyHitPlayer)
                    {
                        Player.Hurt(reason, s.amtTaken, -Player.direction);
                    }
                    else
                    {
                        Player.statLife -= s.amtTaken;
                        if (Player.statLife <= 0)
                        {
                            Player.KillMe(reason, s.amtTaken, -Player.direction);
                        }
                    }
                    sacrifices.RemoveAt(i);
                    i--;
                    continue;
                }
                sacrifices[i] = s;
            }
        }
        public void UpdateZombies()
        {
            if (ghostSlots > ghostSlotsMax)
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
                            int timeComparison = GetDespawnComparison(Main.npc[i], zombie, stats); // Prioritize to kill lower tier slaves
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
        public int GetDespawnComparison(NPC npc, NecromancyNPC zombie, GhostInfo stats)
        {
            float tiering = stats.PowerNeeded;
            if (npc.boss)
            {
                tiering += 10f;
            }
            if (npc.noGravity)
            {
                tiering *= 2f;
            }
            return ((int)(zombie.zombieTimer * tiering) + npc.lifeMax + npc.damage * 3 + npc.defense * 2) * stats.SlotsUsed.GetValueOrDefault(1);
        }

        public override void PostUpdate()
        {
            if (AequusHelpers.Main_dayTime.IsCaching)
            {
                AequusHelpers.Main_dayTime.EndCaching();
            }
            CheckDanger();
            if (accAutoSentry && autoSentryCooldown == 0)
            {
                UpdateAutoSentry();
            }
            TeamContext = 0;
        }
        /// <summary>
        /// Finds the closest enemy to the player, and caches its index in <see cref="Main.npc"/>
        /// </summary>
        public void CheckDanger()
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
        /// Attempts to place a sentry down near the <see cref="NPC"/> at <see cref="closestEnemy"/>'s index. Doesn't do anything if the index is -1, the enemy is not active, or the player has no turret slots. Runs after <see cref="CheckDanger"/>
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
                    SoundEngine.PlaySound(item.UseSound.Value, Main.npc[closestEnemy].Center);
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

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (CheckScam())
            {
                permHasScammed = true;
            }
            MoneyBack(vendor, shopInventory, item);
        }
        public bool CheckScam()
        {
            return scamChance > 0f || flatScamDiscount > 0;
        }
        public bool MoneyBack(NPC vendor, Item[] shopInventory, Item item)
        {
            if (Main.rand.NextFloat() < scamChance)
            {
                int oldStack = item.stack;
                item.stack = 1;
                Player.GetItemExpectedPrice(item, out int sellPrice, out int buyPrice);
                item.stack = oldStack;
                item.value = 0; // A janky way to prevent infinite money, although infinite money is still possible lol
                if (buyPrice > 0)
                {
                    AequusHelpers.DropMoney(new EntitySource_Gift(vendor, "Aequus:FaultyCoin"), Player.getRect(), buyPrice, quiet: false);
                    return true;
                }
            }
            return false;
        }

        public override void ModifyScreenPosition()
        {
            ModContent.GetInstance<GameCamera>().UpdateScreen();
            EffectsSystem.UpdateScreenPosition();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (buffResistHeat && HeatDamageTypes.HeatNPC.Contains(npc.netID))
            {
                damage = (int)(damage * 0.7f);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (buffResistHeat && HeatDamageTypes.HeatProjectile.Contains(proj.type))
            {
                damage = (int)(damage * 0.7f);
            }
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            hitTime = 0;
        }

        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            if (Main.rand.Next(50) <= Player.anglerQuestsFinished - 15)
            {
                if (Main.rand.NextBool())
                {
                    return;
                }

                for (int i = 0; i < rewardItems.Count; i++)
                {
                    if (rewardItems[i].type == ItemID.ApprenticeBait || rewardItems[i].type == ItemID.JourneymanBait || rewardItems[i].type == ItemID.MasterBait)
                    {
                        rewardItems.RemoveAt(i);
                        break;
                    }
                }

                var item = new Item();
                if (Main.rand.NextBool())
                {
                    item.SetDefaults(ModContent.ItemType<Omnibait>());
                }
                else
                {
                    item.SetDefaults(ModContent.ItemType<LegendberryBait>());
                }

                if (Main.rand.Next(25) <= Player.anglerQuestsFinished)
                {
                    item.stack++;
                }
                for (int i = 0; i < 5; i++)
                {
                    if (Main.rand.Next(50 + i * 50) <= Player.anglerQuestsFinished)
                    {
                        item.stack++;
                    }
                }

                rewardItems.Add(item);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadData(TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
        }

        public void PreDrawAllPlayers(LegacyPlayerRenderer playerRenderer, Camera camera, IEnumerable<Player> players)
        {
            if (Main.gameMenu)
            {
                return;
            }
        }

        public static void DrawLegacyAura(Vector2 location, float circumference, float opacity, Color color)
        {
            var texture = PlayerAssets.FocusAura.Value;
            var origin = texture.Size() / 2f;
            var drawCoords = (location - Main.screenPosition).Floor();
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

        public void SacrificeLife(int amt, int frames = 1, int separation = 1, bool hitPlayer = false, PlayerDeathReason reason = null)
        {
            if (amt < frames || frames < 2)
            {
                sacrifices.Add(new LifeSacrifice(amt, 0));
                return;
            }
            int lifeTaken = amt / frames;
            for (int i = 0; i < frames - 1; i++)
            {
                sacrifices.Add(new LifeSacrifice(lifeTaken, i * separation, hitPlayer, reason));
            }
            sacrifices.Add(new LifeSacrifice(lifeTaken + (amt - lifeTaken * frames), frames * separation, hitPlayer, reason));
        }

        public static Player SantankAccClone(Player basePlayer)
        {
            var p = (Player)basePlayer.clientClone();
            p.boneGloveItem = basePlayer.boneGloveItem?.Clone();
            p.boneGloveTimer = basePlayer.boneGloveTimer;
            p.volatileGelatin = basePlayer.volatileGelatin;
            p.volatileGelatinCounter = basePlayer.volatileGelatinCounter;
            return p;
        }

        public static List<Item> GetEquips(Player player, bool armor = true, bool accessories = true)
        {
            var l = new List<Item>();
            if (armor)
            {
                for (int i = 0; i < 3; i++)
                    l.Add(player.armor[i]);
            }
            if (accessories)
            {
                for (int i = 3; i < 10; i++)
                {
                    if (player.IsAValidEquipmentSlotForIteration(i))
                        l.Add(player.armor[i]);
                }
            }
            return l;
        }

        public int ProjectilesOwned_ConsiderProjectileIdentity(int type, bool extraThorough = false)
        {
            int count = 0;
            if (projectileIdentity != -1)
            {
                int myProj = AequusHelpers.FindProjectileIdentity(Player.whoAmI, projectileIdentity);
                if (myProj != -1)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].type == type 
                            && Main.projectile[i].Aequus().projectileOwnerIdentity == projectileIdentity)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
            count = Player.ownedProjectileCounts[type]; 
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].type == type
                    && Main.projectile[i].Aequus().projectileOwnerIdentity > 0)
                {
                    if (extraThorough && AequusHelpers.FindProjectileIdentity(Player.whoAmI, Main.projectile[i].Aequus().projectileOwnerIdentity) == -1)
                    {
                        break;
                    }
                    count--;
                }
            }
            return count;
        }
    }
}