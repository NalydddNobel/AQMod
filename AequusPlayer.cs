using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Minion;
using Aequus.Common;
using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Consumables;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Misc.Fish.Legendary;
using Aequus.Items.Tools;
using Aequus.NPCs.Friendly;
using Aequus.Particles;
using Aequus.Projectiles.Misc;
using Aequus.Projectiles.Misc.GrapplingHooks;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus
{
    public class AequusPlayer : ModPlayer
    {
        public const int BoundBowRegenerationDelay = 60;
        public const float WeaknessDamageMultiplier = 0.8f;
        public const float FrostPotionDamageMultiplier = 0.7f;

        public static List<(int, Func<Player, bool>, Action<Dust>)> SpawnEnchantmentDusts_Custom { get; set; }

        public static int Team;
        public static float? PlayerDrawScale;
        public static int? PlayerDrawForceDye;

        private static MethodInfo Player_ItemCheck_Shoot;

        public int projectileIdentity = -1;

        [SaveData("Souls")]
        public int candleSouls;
        [SaveData("CursorDye")]
        public int cursorDye;

        [SaveData("Scammer")]
        [SaveDataAttribute.IsListedBoolean]
        public bool hasUsedRobsterScamItem;
        /// <summary>
        /// Enabled by <see cref="Moro"/>
        /// </summary>
        [SaveData("Moro")]
        [SaveDataAttribute.IsListedBoolean]
        public bool moroSummonerFruit;
        /// <summary>
        /// Enabled by <see cref="GhostlyGrave"/>
        /// </summary>
        [SaveData("GravesDisabled")]
        [SaveDataAttribute.IsListedBoolean]
        public bool ghostTombstones;

        public bool showPrices;

        public int equippedMask;
        public int cMask;
        public int equippedHat;
        public int cHat;
        public int equippedEyes;
        public int cEyes;
        public int equippedEars;
        public int cEars;

        public int leechHookNPC;

        /// <summary>
        /// Applied by <see cref="SpicyEelBuff"/>
        /// </summary>
        public bool buffSpicyEel;
        /// <summary>
        /// Applied by <see cref="FrostBuff"/>
        /// </summary>
        public bool buffResistHeat;

        public bool biomeCrabCrevice;
        /// <summary>
        /// Whether or not the player is in the Gale Streams event
        /// </summary>
        public bool EventGaleStreams => Player.InModBiome<GaleStreamsInvasion>();
        /// <summary>
        /// A point determining one of the close gore nests. Prioritized by their order in <see cref="DemonSiegeSystem.ActiveSacrifices"/>
        /// </summary>
        public Point eventDemonSiege;
        public bool nearGoreNest;

        /// <summary>
        /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/> -> <see cref="ClosestEnemy"/>
        /// </summary>
        public int closestEnemy;
        public int closestEnemyOld;

        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Used by <see cref="Buffs.NoonBuff"/> and set to 1</para>
        /// </summary>
        public byte forceDayState;

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
        public float grandRewardLuck;
        /// <summary>
        /// An amount of regen to add to the player
        /// </summary>
        public int increasedRegen;

        public bool accFrostburnTurretSquid;
        public float accBloodDiceDamage;
        public int accBloodDiceMoney;
        public bool accGrandReward;
        public int accBoneRing;

        public int accVial;
        public int vialDelay;

        public Item hyperCrystalItem;
        public bool hyperCrystalHidden;
        public int cHyperCrystal;
        public float hyperCrystalDamage;
        public float hyperCrystalDiameter;

        public Item setGravetender;
        public int setGravetenderCheck;
        public int setGravetenderGhost;

        public Item pandorasBoxItem;
        public int pandorasBoxSpawnChance;

        public Item turretSquidItem;
        public int turretSquidTimer;

        public Item healingMushroomItem;
        public int healingMushroomRegeneration;
        public int cHealingMushroom;
        public float mendshroomDiameter;

        public Item celesteTorusItem;
        public int cCelesteTorus;

        /// <summary>
        /// Set by <see cref="SantankSentry"/>
        /// </summary>
        public Item sentryInheritItem;
        public Item ammoBackpackItem;
        public Item mothmanMaskItem;
        public Item glowCoreItem;

        public bool hasExpertBoost;
        /// <summary>
        /// Set to true by <see cref="MechsSentry"/>
        /// </summary>
        public bool accExpertBoost;
        public int expertBoostWormScarfTimer;
        public bool expertBoostBoCProbesHurtSignal;
        public int expertBoostBoCProjDefense;
        public int expertBoostBoCTimer;
        public int expertBoostBoCDefense;

        /// <summary>
        /// Set by <see cref="FoolsGoldRing"/>
        /// </summary>
        public bool accFoolsGold;

        /// <summary>
        /// Set to true by <see cref="Items.Armor.PassiveSummon.DartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.SuperDartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.FlowerCrown"/>
        /// </summary>
        public bool wearingSummonHelmet;
        /// <summary>
        /// Used by summon helmets (<see cref="Items.Armor.PassiveSummon.DartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.SuperDartTrapHat"/>, <see cref="Items.Armor.PassiveSummon.FlowerCrown"/>) to time projectile spawns and such.
        /// </summary>
        public int summonHelmetTimer;

        /// <summary>
        /// Set by <see cref="SkeletonKey"/>
        /// </summary>
        public bool skeletonKey;
        /// <summary>
        /// Set by <see cref="ItemID.ShadowKey"/>
        /// </summary>
        public bool shadowKey;

        public int boundBowAmmo;
        public int boundBowAmmoTimer;

        public int itemHits;
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

        public int soulCandleLimit;

        public int turretSlotCount;

        public int ghostSlotsMax;
        public int ghostSlotsOld;
        public int ghostSlots;

        public int ghostProjExtraUpdates;
        public int ghostLifespan;

        public int timeSinceLastHit;
        public int idleTime;

        public bool MendshroomActive => idleTime >= 60;

        public bool ExpertBoost => hasExpertBoost || accExpertBoost;

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
            SpawnEnchantmentDusts_Custom = new List<(int, Func<Player, bool>, Action<Dust>)>();
            Player_ItemCheck_Shoot = typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override void Unload()
        {
            SpawnEnchantmentDusts_Custom = null;
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
            clone.timeSinceLastHit = timeSinceLastHit;
            clone.expertBoostBoCDefense = expertBoostBoCDefense;
            clone.increasedRegen = increasedRegen;
            clone.candleSouls = candleSouls;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (AequusPlayer)clientPlayer;

            PacketHandler.Send((p) =>
            {
                bool sentAnything = false;
                p.Write((byte)Player.whoAmI);
                PacketHandler.FlaggedSend(
                    (clone.itemCombo - itemCombo).Abs() > 10 ||
                    (clone.itemSwitch - itemSwitch).Abs() > 10 ||
                    (clone.itemUsage - itemUsage).Abs() > 10 ||
                    (clone.itemCooldown - itemCooldown).Abs() > 10 ||
                    clone.itemCooldownMax != itemCooldownMax,
                (p) =>
                    {
                        p.Write(itemCombo);
                        p.Write(itemSwitch);
                        p.Write(itemUsage);
                        p.Write(itemCooldown);
                        p.Write(itemCooldownMax);
                        sentAnything = true;
                    }, p);

                PacketHandler.FlaggedSend((clone.timeSinceLastHit - timeSinceLastHit).Abs() > 10,
                (p) =>
                    {
                        p.Write(timeSinceLastHit);
                        sentAnything = true;
                    }, p);

                PacketHandler.FlaggedSend(clone.candleSouls != candleSouls,
                (p) =>
                    {
                        p.Write(candleSouls);
                        sentAnything = true;
                    }, p);

                return sentAnything;

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
            if (reader.ReadBoolean())
            {
                timeSinceLastHit = reader.ReadInt32();
            }
            if (reader.ReadBoolean())
            {
                candleSouls = reader.ReadInt32();
            }
        }

        public override void Initialize()
        {
            boundBowAmmo = 10;
            boundBowAmmoTimer = 60;
            cursorDye = -1;
            candleSouls = 0;
            ghostTombstones = false;
            moroSummonerFruit = false;
            hasUsedRobsterScamItem = false;

            turretSquidTimer = 120;
            itemCooldown = 0;
            itemCooldownMax = 0;
            itemCombo = 0;
            itemSwitch = 0;
            interactionCooldown = 60;
            closestEnemyOld = -1;
            closestEnemy = -1;
        }

        public override void UpdateDead()
        {
            timeSinceLastHit = 0;
            hasExpertBoost = false;
            accExpertBoost = false;
        }

        public override void ResetEffects()
        {
            if (boundBowAmmoTimer > 0)
                boundBowAmmoTimer--;
            if (boundBowAmmoTimer == 0)
            {
                boundBowAmmo++;
                boundBowAmmoTimer = BoundBowRegenerationDelay;
            }
            if (boundBowAmmo >= 10)
            {
                boundBowAmmoTimer = BoundBowRegenerationDelay;
            }
                
            showPrices = false;

            equippedMask = 0;
            cMask = 0;
            equippedHat = 0;
            cHat = 0;
            equippedEyes = 0;
            cEyes = 0;
            equippedEars = 0;
            cEars = 0;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<LeechHookProj>()] <= 0)
                leechHookNPC = -1;

            soulCandleLimit = 0;

            if (Player.velocity.Length() < 1f)
            {
                idleTime++;
            }
            else
            {
                idleTime = 0;
            }

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

            if (expertBoostWormScarfTimer > 0)
            {
                expertBoostWormScarfTimer--;
            }
            expertBoostBoCProjDefense = expertBoostBoCDefense;

            setGravetender = null;

            pandorasBoxSpawnChance = 0;
            pandorasBoxItem = null;


            turretSquidItem = null;
            if (!InDanger)
            {
                turretSquidTimer = Math.Min(turretSquidTimer, (ushort)240);
            }
            if (turretSquidTimer > 0)
            {
                turretSquidTimer--;
            }

            accBloodDiceMoney = 0;
            accBloodDiceDamage = 0f;

            cHyperCrystal = 0;
            hyperCrystalDiameter = 0f;
            hyperCrystalDamage = 0f;
            hyperCrystalItem = null;

            healingMushroomItem = null;
            mendshroomDiameter = 0f;
            healingMushroomRegeneration = 0;
            cHealingMushroom = 0;

            celesteTorusItem = null;
            cCelesteTorus = 0;

            glowCoreItem = null;
            ammoBackpackItem = null;
            mothmanMaskItem = null;
            sentryInheritItem = null;

            scamChance = 0f;
            flatScamDiscount = 0;

            if (vialDelay > 0)
                vialDelay--;
            accVial = 0;
            accBoneRing = 0;
            grandRewardLuck = 0f;
            accGrandReward = false;
            accFoolsGold = false;

            hasExpertBoost = accExpertBoost;
            accExpertBoost = false;
            Team = Player.team;

            buffSpicyEel = false;
            buffResistHeat = false;

            skeletonKey = false;
            shadowKey = false;

            forceDayState = 0;
            ghostSlotsMax = 1;
            ghostProjExtraUpdates = 0;
            ghostLifespan = 3600;
        }

        public override void PreUpdate()
        {
            projectileIdentity = -1;
            if (forceDayState == 1)
            {
                AequusHelpers.Main_dayTime.StartCaching(true);
            }
            else if (forceDayState == 2)
            {
                AequusHelpers.Main_dayTime.StartCaching(false);
            }
            forceDayState = 0;

            eventDemonSiege = DemonSiegeSystem.FindDemonSiege(Player.Center);
            nearGoreNest = GoreNestTile.BiomeCount > 0;
        }

        public override void PreUpdateBuffs()
        {
            timeSinceLastHit++;
        }

        public override void PostUpdateEquips()
        {
            UpdateBank(Player.bank, 0);
            UpdateBank(Player.bank2, 1);
            UpdateBank(Player.bank3, 2);
            UpdateBank(Player.bank4, 3);
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
                        shadowKey = true;
                    }
                    else if (bank.item[i].type == ItemID.DiscountCard && !Player.discount)
                    {
                        update = true;
                    }
                    else if (AequusItem.BankEquipFuncs.Contains(bank.item[i].type))
                    {
                        update = true;
                    }
                    else if (bank.item[i].ModItem is Hooks.IUpdateBank b)
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
                itemHits = 0;
            }
            CountSentries();
        }

        public override void PostUpdate()
        {
            if (Player.HasBuff<BlueFire>())
            {
                if (Main.rand.NextBool(3))
                AequusEffects.AbovePlayers.Add(new MonoParticle(Main.rand.NextCircularFromRect(Player.getRect()) + new Vector2(0f, 10f), -Player.velocity * Main.rand.NextFloat(0.1f, 0.4f) + new Vector2(Main.rand.NextFloat(-5f, 5f), -Main.rand.NextFloat(2f, 14f)),
                    new Color(10, 20, Main.rand.Next(100, 255), 10), Main.rand.NextFloat(1.25f, 2f), Main.rand.NextFloat(MathHelper.TwoPi)));
                for (int i = 0; i < 3; i++)
                {
                    if (Main.rand.NextBool(3))
                        AequusEffects.AbovePlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(Player.getRect()) + new Vector2(0f, 10f), -Player.velocity * Main.rand.NextFloat(0.1f, 0.4f) + new Vector2(Main.rand.NextFloat(-3f, 3f), -Main.rand.NextFloat(2f, 12f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(15, 40, 80, 10), Main.rand.NextFloat(1.25f, 2f), Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextFloat(MathHelper.TwoPi)));
                }
            }

            if (glowCoreItem != null && glowCoreItem.ModItem is DyeableAccessory)
            {
                GlowCore.AddLight(Player.Center, Player, this);
            }

            if (hyperCrystalItem != null && !hyperCrystalHidden && ProjectilesOwned(ModContent.ProjectileType<HyperCrystalProj>()) <= 0)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(hyperCrystalItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<HyperCrystalProj>(),
                    0, 0f, Player.whoAmI, projectileIdentity + 1);
            }

            if (healingMushroomItem != null && healingMushroomItem.shoot > ProjectileID.None
                && MendshroomActive && ProjectilesOwned(healingMushroomItem.shoot) <= 0)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(healingMushroomItem), Player.Center, Vector2.Zero, healingMushroomItem.shoot,
                    0, 0f, Player.whoAmI, projectileIdentity + 1);
            }

            if (Main.myPlayer == Player.whoAmI)
            {
                UpdateMaxZombies();
            }

            ghostSlotsOld = ghostSlots;
            ghostSlots = 0;
            ClosestEnemy();
            Team = 0;

            if (setGravetender != null)
            {
                Gravetender(setGravetender);
            }
            else
            {
                setGravetenderCheck = 0;
                setGravetenderGhost = -1;
            }

            if (turretSquidItem != null && turretSquidTimer == 0)
            {
                UpdateSentrySquid(Player.Aequus().closestEnemy);
            }

            if (sentryInheritItem != null)
            {
                UpdateSantankSentry();
            }

            if (!accExpertBoost || Player.brainOfConfusionItem == null)
            {
                expertBoostBoCDefense = 0;
                expertBoostBoCTimer = 0;
            }

            if (AequusHelpers.Main_dayTime.IsCaching)
            {
                AequusHelpers.Main_dayTime.EndCaching();
            }
        }
        /// <summary>
        /// Finds the closest enemy to the player, and caches its index in <see cref="Main.npc"/>
        /// </summary>
        public void ClosestEnemy()
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

        public void Gravetender(Item gravetenderHood)
        {
            if (gravetenderHood.shoot > ProjectileID.None && Player.ownedProjectileCounts[setGravetender.shoot] <= 0)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(setGravetender), Player.Center, -Vector2.UnitY, setGravetender.shoot,
                    Player.GetWeaponDamage(setGravetender), Player.GetWeaponKnockback(setGravetender), Player.whoAmI);
            }
            if (gravetenderHood.buffType > 0)
            {
                Player.AddBuff(setGravetender.buffType, 2, quiet: true);
            }

            if (setGravetenderCheck > 0)
            {
                setGravetenderCheck--;
                if (setGravetenderGhost > -1 && !Main.npc[setGravetenderGhost].active)
                {
                    setGravetenderCheck = 0;
                }
            }

            if (setGravetenderCheck <= 0)
            {
                setGravetenderGhost = -1;
                setGravetenderCheck = 20;
                int power = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].friendly && Player.Distance(Main.npc[i].Center) < 2000f && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == Player.whoAmI && zombie.slotsConsumed > 0)
                    {
                        int npcPower = zombie.DespawnPriority(Main.npc[i]);
                        if (npcPower > power)
                        {
                            setGravetenderGhost = i;
                            power = npcPower;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to place a sentry down near the <see cref="NPC"/> at <see cref="closestEnemy"/>'s index. Doesn't do anything if the index is -1, the enemy is not active, or the player has no turret slots. Runs after <see cref="ClosestEnemy"/>
        /// </summary>
        public void UpdateSentrySquid(int closestEnemy)
        {
            if (closestEnemy == -1 || !Main.npc[closestEnemy].active || Player.maxTurrets <= 0)
            {
                turretSquidTimer = 30;
                return;
            }

            var item = SentrySquid_GetStaff();
            if (item == null)
            {
                turretSquidTimer = 30;
                return;
            }

            if (Player.Aequus().turretSlotCount >= Player.maxTurrets)
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
                turretSquidTimer = 30;
                return;
            }

            if (!SentrySquid.TurretStaffs.TryGetValue(item.type, out var sentryUsage))
            {
                sentryUsage = SentrySquid.TurretStaffUsage.Default;
            }
            if (sentryUsage.TrySummoningThisSentry(Player, item, Main.npc[closestEnemy]))
            {
                Player.UpdateMaxTurrets();
                if (Player.maxTurrets > 1)
                {
                    turretSquidTimer = 240;
                }
                else
                {
                    turretSquidTimer = 3000;
                }
                if (Main.netMode != NetmodeID.Server && item.UseSound != null)
                {
                    SoundEngine.PlaySound(item.UseSound.Value, Main.npc[closestEnemy].Center);
                }
            }
            else
            {
                turretSquidTimer = 30;
            }
        }
        /// <summary>
        /// Determines an item to use as a Sentry Staff for <see cref="UpdateSentrySquid"/>
        /// </summary>
        /// <returns></returns>
        public Item SentrySquid_GetStaff()
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

        /// <summary>
        /// If the player has too many zombies, it kills the oldest and least prioritized one.
        /// </summary>
        public void UpdateMaxZombies()
        {
            if (ghostSlots <= 0)
            {
                Player.ClearBuff(ModContent.BuffType<NecromancyOwnerBuff>());
                return;
            }
            int slot = Player.FindBuffIndex(ModContent.BuffType<NecromancyOwnerBuff>());
            if (slot != -1)
            {
                if (Player.buffTime[slot] <= 2)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == Player.whoAmI)
                        {
                            Main.npc[i].life = -1;
                            Main.npc[i].HitEffect();
                            Main.npc[i].active = false;
                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, 9999);
                            }
                        }
                    }
                    return;
                }
            }
            else
            {
                if (ghostSlots > 0)
                    Player.AddBuff(ModContent.BuffType<NecromancyOwnerBuff>(), 30);
            }
            if (ghostSlots > ghostSlotsMax)
            {
                int removeNPC = -1;
                int oldestTime = int.MaxValue;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie && zombie.zombieOwner == Player.whoAmI && zombie.slotsConsumed > 0)
                    {
                        int timeComparison = zombie.DespawnPriority(Main.npc[i]); // Prioritize to kill lower tier slaves
                        if (timeComparison < oldestTime)
                        {
                            removeNPC = i;
                            oldestTime = timeComparison;
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
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, removeNPC, 9999);

                        Aequus.Instance.Logger.Debug("NPC: " + Lang.GetNPCName(Main.npc[removeNPC].type) + ", WhoAmI: " + removeNPC + ", Tier:" + Main.npc[removeNPC].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier);
                    }
                }
            }
        }

        public void UpdateSantankSentry()
        {
            try
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].TryGetGlobalProjectile<SantankSentryProjectile>(out var sentry))
                    {
                        sentry.UpdateInheritance(Main.projectile[i]);
                    }
                }
            }
            catch
            {
            }
        }

        public override void UpdateLifeRegen()
        {
            Player.AddLifeRegen(increasedRegen);
            increasedRegen = 0;
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.HasBuff<BlueFire>())
                Player.AddLifeRegen(-6);
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (damage > 1000)
            {
                return true;
            }

            if (ExpertBoost && expertBoostBoCDefense > 60)
            {
                int def = expertBoostBoCDefense;
                expertBoostBoCDefense -= damage;
                if (expertBoostBoCDefense < 5)
                {
                    expertBoostBoCDefense = 5;
                    expertBoostBoCTimer = 0;
                    damage -= def;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.NPCHit4);
                    damage = 1;
                }
            }
            return true;
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (CheckScam())
            {
                hasUsedRobsterScamItem = true;
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
            ModContent.GetInstance<CameraFocus>().UpdateScreen();
            AequusEffects.UpdateScreenPosition();
            Main.screenPosition = Main.screenPosition.Floor();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (npc.HasBuff<Weakness>())
            {
                damage = (int)(damage * WeaknessDamageMultiplier);
            }

            if (buffResistHeat && npc.Aequus().heatDamage)
            {
                damage = (int)(damage * FrostPotionDamageMultiplier);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            var aequus = proj.Aequus();
            if (aequus.HasNPCOwner)
            {
                if (Main.npc[aequus.sourceNPC].HasBuff<Weakness>())
                {
                    damage = (int)(damage * WeaknessDamageMultiplier);
                }
            }
            if (buffResistHeat && proj.Aequus().heatDamage)
            {
                damage = (int)(damage * FrostPotionDamageMultiplier);
            }
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            timeSinceLastHit = 0;
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (target.type != NPCID.TargetDummy)
            {
                if (crit)
                {
                    CheckBloodDice(ref damage);
                }
            }
            HyperCrystalDamage(target.getRect(), ref damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.type != NPCID.TargetDummy)
            {
                if (crit)
                {
                    CheckBloodDice(ref damage);
                }
            }
            HyperCrystalDamage(target.getRect(), ref damage);
        }
        public void CheckBloodDice(ref int damage)
        {
            if (accBloodDiceDamage > 0f && Player.CanBuyItem(accBloodDiceMoney))
            {
                SoundEngine.PlaySound(SoundID.Coins);
                Player.BuyItem(accBloodDiceMoney);
                damage = (int)(damage * (1f + accBloodDiceDamage / 2f));
            }
        }
        public void HyperCrystalDamage(Rectangle targetRect, ref int damage)
        {
            if (hyperCrystalDiameter > 0f && Player.Distance(targetRect.ClosestDistance(Player.Center)) < hyperCrystalDiameter / 2f)
            {
                damage = (int)(damage * (1f + hyperCrystalDamage));
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (mothmanMaskItem != null && Player.statLife >= Player.statLifeMax2 && crit)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 300);
            }
            if (target.type != NPCID.TargetDummy)
                CheckLeechHook(target, damage);
            CheckBlackVial(target);
            CheckBoneRing(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (mothmanMaskItem != null && Player.statLife >= Player.statLifeMax2 && crit)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 300);
            }
            if (target.type != NPCID.TargetDummy)
                CheckLeechHook(target, damage);
            CheckBlackVial(target);
            CheckBoneRing(target);

            if (proj.DamageType == DamageClass.Summon || proj.minion || proj.sentry)
            {
                NecromancyHit(target, proj);
            }
        }
        public void CheckLeechHook(NPC target, int damage)
        {
            if (leechHookNPC == target.whoAmI && Player.statLife < Player.statLifeMax2)
            {
                int lifeHealed = Math.Min(Math.Max(damage / 5, 1), (int)Player.lifeSteal);
                if (lifeHealed + Player.statLife > Player.statLifeMax2)
                {
                    lifeHealed = Player.statLifeMax2 - Player.statLife;
                }
                if (lifeHealed > 0)
                {
                    Player.lifeSteal -= lifeHealed;
                    Player.statLife += lifeHealed;
                    Player.HealEffect(lifeHealed);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.PlayerHeal, -1, -1, null, Player.whoAmI, lifeHealed);
                        NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, Player.whoAmI, lifeHealed);
                    }
                }
            }
        }
        public void CheckBlackVial(NPC target)
        {
            if (vialDelay <= 0 && accVial > 0 && Main.rand.NextBool(accVial))
            {
                int buff = Main.rand.Next(BlackPhial.DebuffsAfflicted);
                if (!target.buffImmune[buff])
                {
                    vialDelay = 240;
                    target.AddBuff(buff, 300);
                }
            }
        }
        public void CheckBoneRing(NPC target)
        {
            if (accBoneRing > 0 && Main.rand.NextBool(accBoneRing))
            {
                target.AddBuff(ModContent.BuffType<Weakness>(), 360);
            }
        }
        /// <summary>
        /// Inflicts <see cref="SoulStolen"/> if the player is able to get more candle souls
        /// </summary>
        /// <param name="target"></param>
        /// <param name="proj"></param>
        public void NecromancyHit(NPC target, Projectile proj)
        {
            if (candleSouls < soulCandleLimit)
            {
                target.AddBuff(ModContent.BuffType<SoulStolen>(), 300);
            }
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

        public void RefreshJumpOption()
        {
            if (Player.hasJumpOption_Cloud && !Player.isPerformingJump_Cloud && !Player.canJumpAgain_Cloud)
            {
                Player.canJumpAgain_Cloud = true;
            }
            else if (Player.hasJumpOption_Blizzard && !Player.isPerformingJump_Blizzard && !Player.canJumpAgain_Blizzard)
            {
                Player.canJumpAgain_Blizzard = true;
            }
            else if (Player.hasJumpOption_Sandstorm && !Player.isPerformingJump_Sandstorm && !Player.canJumpAgain_Sandstorm)
            {
                Player.canJumpAgain_Sandstorm = true;
            }
            else if (Player.hasJumpOption_Fart && !Player.isPerformingJump_Fart && !Player.canJumpAgain_Fart)
            {
                Player.canJumpAgain_Fart = true;
            }
            else if (Player.hasJumpOption_Sail && !Player.isPerformingJump_Sail && !Player.canJumpAgain_Sail)
            {
                Player.canJumpAgain_Sail = true;
            }
            else if (Player.hasJumpOption_Basilisk && !Player.isPerformingJump_Basilisk && !Player.canJumpAgain_Basilisk)
            {
                Player.canJumpAgain_Basilisk = true;
            }
            else if (Player.hasJumpOption_Unicorn && !Player.isPerformingJump_Unicorn && !Player.canJumpAgain_Unicorn)
            {
                Player.canJumpAgain_Unicorn = true;
            }
            else if (Player.hasJumpOption_WallOfFleshGoat && !Player.isPerformingJump_WallOfFleshGoat && !Player.canJumpAgain_WallOfFleshGoat)
            {
                Player.canJumpAgain_WallOfFleshGoat = true;
            }
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

        public int ProjectilesOwned(int type)
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
                            && Main.projectile[i].Aequus().sourceProjIdentity == projectileIdentity)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
            if (Main.myPlayer != Player.whoAmI)
            {
                return count + 1;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].type == type
                    && Main.projectile[i].Aequus().sourceProjIdentity == -1)
                {
                    count++;
                }
            }
            return count;
        }

        public static bool CanScamNPC(NPC npc)
        {
            return npc.type != ModContent.NPCType<Exporter>();
        }

        public void UseAmmoBackpack(NPC npc, Item ammoBackpack)
        {
            var neededAmmoTypes = AmmoBackpack_GetAmmoTypesToSpawn(npc, ammoBackpack);
            if (neededAmmoTypes.Count > 0)
            {
                int chosenType = Main.rand.Next(neededAmmoTypes);
                int stack = AmmoBackpack_DetermineStack(chosenType, npc, ammoBackpack);
                //Main.NewText("Dropping " + AequusText.ItemText(chosenType) + "(" + stack + ") from " + npc.FullName, Color.LightBlue);
                //Main.NewText((int)(AmmoBackpack_StackMultiplier(chosenType, npc, ammoBackpack) * 10000) / 100f + "% NPC Value: " + npc.value + " Item Value: " + ContentSamples.ItemsByType[chosenType].value);
                //Main.NewText((int)(30 * AmmoBackpack_StackMultiplier(chosenType, npc, ammoBackpack)) + " is the max amount of ammo dropable.");
                Item.NewItem(Player.GetSource_Accessory(ammoBackpack), npc.getRect(), chosenType, stack);
            }
        }
        public int AmmoBackpack_DetermineStack(int itemToSpawn, NPC npc, Item ammoBackpack)
        {
            return (int)Math.Max((Main.rand.Next(30) + 1) * AmmoBackpack_StackMultiplier(itemToSpawn, npc, ammoBackpack), 1);
        }
        public float AmmoBackpack_StackMultiplier(int itemToSpawn, NPC npc, Item ammoBackpack)
        {
            return 1f - Math.Clamp(ContentSamples.ItemsByType[itemToSpawn].value / (Item.silver * (npc.value / (Item.silver * 5f))), 0f, 1f);
        }
        public List<int> AmmoBackpack_GetAmmoTypesToSpawn(NPC npc, Item ammoBackpack)
        {
            var l = new List<int>();
            bool fullSlots = !Player.inventory[Main.InventoryAmmoSlotsStart].IsAir && !Player.inventory[Main.InventoryAmmoSlotsStart + 1].IsAir
                && !Player.inventory[Main.InventoryAmmoSlotsStart + 2].IsAir && !Player.inventory[Main.InventoryAmmoSlotsStart + 3].IsAir;

            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                var item = Player.inventory[i];
                if (item.IsAir || !item.consumable || item.makeNPC > 0 || item.ammo <= ItemID.None || ContentSamples.ItemsByType[item.ammo].makeNPC > 0 || item.bait > 0)
                {
                    continue;
                }
                if ((!fullSlots || item.type == item.ammo) && !AmmoBackpack.AmmoBlacklist.Contains(item.ammo) && !l.Contains(item.ammo))
                    l.Add(item.ammo);
                if (item.stack < item.maxStack && !AmmoBackpack.AmmoBlacklist.Contains(item.type) && !l.Contains(item.type) && Main.rand.NextBool(3))
                    l.Add(item.type);
            }

            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                if (!Player.inventory[i].consumable)
                {
                    l.Remove(Player.inventory[i].ammo);
                    l.Remove(Player.inventory[i].type);
                }
            }
            return l;
        }

        public void Mendshroom()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(Player.Center) < mendshroomDiameter / 2f)
                {
                    mendshroomHeal(i);
                }
            }
        }
        public void mendshroomHeal(int i)
        {
            var bungus = Main.player[i].Aequus();
            if (bungus.increasedRegen < healingMushroomRegeneration)
            {
                bungus.increasedRegen = healingMushroomRegeneration;
                Main.player[i].AddBuff(healingMushroomItem.buffType, 4, quiet: true);
            }
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

            LegacyShootProj(player, item, source, location, velocity, projType, projDamage, projKB, setMousePos);
        }
        private static int LegacyShootProj(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 location, Vector2 velocity, int projType, int projDamage, float projKB, Vector2? setMousePos)
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

        public static Player ProjectileClone(Player basePlayer)
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

        public Vector2 GetRandomTombstoneVelocity(int hitDirection)
        {
            float num;
            for (num = Main.rand.Next(-35, 36) * 0.1f; num < 2f && num > -2f; num += Main.rand.Next(-30, 31) * 0.1f)
            {
            }
            return new Vector2(Main.rand.Next(10, 30) * 0.1f * hitDirection + num,
                Main.rand.Next(-40, -20) * 0.1f);
        }

        public void LegendaryFishRewards(NPC npc, Item item, int i)
        {
            int money = Main.rand.Next(Item.gold * 8, Item.gold * 10);
            var source = npc.GetSource_GiftOrReward();
            if (item.type == ModContent.ItemType<GoreFish>())
            {
                Player.QuickSpawnItem(source, ItemID.LavaFishingHook);
                if (NPC.downedBoss3 && Main.rand.NextBool())
                {
                    Player.QuickSpawnItem(source, Main.hardMode ? ItemID.LavaCrateHard : ItemID.LavaCrate);
                }
            }
            else if (item.type == ModContent.ItemType<ArgonFish>())
            {
                Player.QuickSpawnItem(source, ItemID.ArgonMoss, 15);
            }
            else if (item.type == ModContent.ItemType<KryptonFish>())
            {
                Player.QuickSpawnItem(source, ItemID.KryptonMoss, 15);
            }
            else if (item.type == ModContent.ItemType<XenonFish>())
            {
                Player.QuickSpawnItem(source, ItemID.XenonMoss, 15);
            }
            else if (item.type == ModContent.ItemType<RadonFish>())
            {
                Player.QuickSpawnItem(source, ItemID.StinkPotion, 5);
            }
            AequusHelpers.DropMoney(source, Player.getRect(), money, quiet: false);
        }

        #region Hooks
        private static void LoadHooks()
        {
            On.Terraria.Player.DropTombstone += Hook_ModifyTombstone;
            On.Terraria.NPC.NPCLoot_DropMoney += Hook_NoMoreMoney;
            On.Terraria.GameContent.ItemDropRules.ItemDropResolver.ResolveRule += Hook_RerollLoot;
            On.Terraria.Player.RollLuck += Hook_ModifyLuckRoll;
            On.Terraria.Player.DropCoins += Hook_DropCoinsOnDeath;
            On.Terraria.Player.GetItemExpectedPrice += Hook_GetItemPrice;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += Hook_OnRenderPlayer;
        }

        private static void Hook_ModifyTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, NetworkText deathText, int hitDirection)
        {
            var aequus = self.Aequus();
            if (aequus.ghostTombstones)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPCDirect(self.GetSource_Death("GhostlyGrave"), self.Center, NPCID.Ghost);
                }
                return;
            }

            var graves = CustomTombstones.ChooseTombstone(self, coinsOwned, deathText, hitDirection);
            if (graves != null && graves.Count > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(self.GetSource_Death(), self.Center, aequus.GetRandomTombstoneVelocity(hitDirection), Main.rand.Next(graves), 0, 0f, Main.myPlayer);
                Main.projectile[p].miscText = deathText.ToString();
                return;
            }

            orig(self, coinsOwned, deathText, hitDirection);
        }

        private static void Hook_NoMoreMoney(On.Terraria.NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.Aequus().accGrandReward)
            {
                return;
            }
            orig(self, closestPlayer);
        }

        private static ItemDropAttemptResult Hook_RerollLoot(On.Terraria.GameContent.ItemDropRules.ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            var result = orig(self, rule, info);
            if (info.player != null && result.State == ItemDropAttemptResultState.FailedRandomRoll)
            {
                if (AequusHelpers.iterations == 0)
                {
                    for (float luckLeft = info.player.Aequus().grandRewardLuck; luckLeft > 0f; luckLeft--)
                    {
                        if (luckLeft < 1f)
                        {
                            if (Main.rand.NextFloat(1f) > luckLeft)
                            {
                                return result;
                            }
                        }
                        var result2 = orig(self, rule, info);
                        AequusHelpers.iterations++;
                        if (result2.State != ItemDropAttemptResultState.FailedRandomRoll)
                        {
                            AequusHelpers.iterations = 0;
                            return result2;
                        }
                    }
                    AequusHelpers.iterations = 0;
                }
                else
                {
                    AequusHelpers.iterations++;
                }
            }
            return result;
        }

        private static int Hook_ModifyLuckRoll(On.Terraria.Player.orig_RollLuck orig, Player self, int range)
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
            for (float luckLeft = luckRerolls; luckLeft > 0f; luckLeft--)
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

        private static int Hook_DropCoinsOnDeath(On.Terraria.Player.orig_DropCoins orig, Player self)
        {
            if (self.Aequus().accFoolsGold)
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

        private static void Hook_GetItemPrice(On.Terraria.Player.orig_GetItemExpectedPrice orig, Player self, Item item, out int calcForSelling, out int calcForBuying)
        {
            orig(self, item, out calcForSelling, out calcForBuying);
            if (item.shopSpecialCurrency != -1 || self.talkNPC == -1)
            {
                return;
            }

            if (!CanScamNPC(Main.npc[self.talkNPC]))
            {
                return;
            }

            int min = item.shopCustomPrice.GetValueOrDefault(item.value) / 5;
            if (calcForBuying < min) // shrug
            {
                return;
            }
            calcForBuying = Math.Max(calcForBuying - self.Aequus().flatScamDiscount, min);
        }

        private static void Hook_OnRenderPlayer(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
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

        public static void SpawnEnchantmentDusts(Vector2 position, Vector2 velocity, Player player)
        {
            if (player.magmaStone)
            {
                var d = Dust.NewDustPerfect(position, DustID.Torch, velocity * 2f, Alpha: 100, Scale: 2.5f);
                d.noGravity = true;
            }
            switch (player.meleeEnchant) 
            {
                case FlasksID.Venom:
                    {
                        if (Main.rand.NextBool(3))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.Venom, velocity * 2f, Alpha: 100);
                            d.noGravity = true;
                            d.fadeIn = 1.5f;
                            d.velocity *= 0.25f;
                        }
                    }
                    break;

                case FlasksID.CursedInferno:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.CursedTorch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                            d.noGravity = true;
                            d.velocity *= 0.7f;
                            d.velocity.Y -= 0.5f;
                        }
                    }
                    break;

                case FlasksID.Fire:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.Torch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                            d.noGravity = true;
                            d.velocity *= 0.7f;
                            d.velocity.Y -= 0.5f;
                        }
                    }
                    break;

                case FlasksID.Midas:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.Enchanted_Gold, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                            d.noGravity = true;
                            d.velocity *= 0.7f;
                            d.velocity.Y -= 0.5f;
                        }
                    }
                    break;

                case FlasksID.Ichor:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.IchorTorch, velocity, Alpha: 100, Scale: 2.5f);
                            d.velocity.X += player.direction;
                            d.velocity.Y -= 0.2f;
                        }
                    }
                    break;

                case FlasksID.Nanites:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.IceTorch, velocity, Alpha: 100, Scale: 2.5f);
                            d.velocity.X += player.direction;
                            d.velocity.Y -= 0.2f;
                        }
                    }
                    break;

                case FlasksID.Party:
                    {
                        if (Main.rand.NextBool(40))
                        {
                            var g = Gore.NewGorePerfect(player.GetSource_ItemUse(player.HeldItem), position, velocity, Main.rand.Next(276, 283));
                            g.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            g.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            g.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                            g.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                            g.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        }
                        else if (Main.rand.NextBool(20))
                        {
                            var d = Dust.NewDustPerfect(position, Main.rand.Next(139, 143), velocity, Scale: 1.2f);
                            d.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            d.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            d.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                            d.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                            d.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                        }
                    }
                    break;

                case FlasksID.Poison:
                    {
                        if (Main.rand.NextBool(3))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.Poisoned, velocity * 2f, Alpha: 100);
                            d.noGravity = true;
                            d.fadeIn = 1.5f;
                            d.velocity *= 0.25f;
                        }
                    }
                    break;
            }
            foreach (var c in SpawnEnchantmentDusts_Custom)
            {
                if (c.Item2(player))
                {
                    c.Item3(Dust.NewDustPerfect(position, c.Item1, velocity));
                }
            }
        }
    }
}