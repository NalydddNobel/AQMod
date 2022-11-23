using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Misc;
using Aequus.Common;
using Aequus.Common.Players;
using Aequus.Common.Utilities;
using Aequus.Content;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Aequus.Graphics;
using Aequus.Graphics.PlayerLayers;
using Aequus.Graphics.Primitives;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Accessories.Vanity;
using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Consumables;
using Aequus.Items.Misc;
using Aequus.Items.Tools.Misc;
using Aequus.Items.Weapons.Ranged;
using Aequus.NPCs.Friendly.Town;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles;
using Aequus.Projectiles.GlobalProjs;
using Aequus.Projectiles.Misc.Bobbers;
using Aequus.Projectiles.Misc.Friendly;
using Aequus.Projectiles.Misc.GrapplingHooks;
using Aequus.Tiles;
using Aequus.Tiles.PhysicistBlocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameInput;
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
        public const int BoundBowMaxAmmo = 15;
        public const int BoundBowRegenerationDelay = 50;
        public const float WeaknessDamageMultiplier = 0.8f;
        public const float FrostPotionDamageMultiplier = 0.7f;

        public static int PlayerContext;

        public static List<(int, Func<Player, bool>, Action<Dust>)> SpawnEnchantmentDusts_Custom { get; set; }
        public static ModKeybind KeybindSetbonusAlt { get; private set; }

        public static int Team;
        public float? CustomDrawShadow;
        public float? DrawScale;
        public int? DrawForceDye;

        private static MethodInfo Player_ItemCheck_Shoot;

        public int projectileIdentity = -1;

        public int cursorDye;
        public int cursorDyeOverride;

        public int CursorDye { get => cursorDyeOverride > 0 ? cursorDyeOverride : cursorDye; set => cursorDye = value; }

        /// <summary>
        /// Enabled by <see cref="VictorsReward"/>
        /// </summary>
        [SaveData("MaxLifeRespawn")]
        [SaveDataAttribute.IsListedBoolean]
        public bool maxLifeRespawnReward;
        [SaveData("Scammer")]
        [SaveDataAttribute.IsListedBoolean]
        public bool hasUsedRobsterScamItem;
        /// <summary>
        /// Enabled by <see cref="WhitePhial"/>
        /// </summary>
        [SaveData("WhitePhial")]
        [SaveDataAttribute.IsListedBoolean]
        public bool whitePhial;
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

        //public ShatteringVenus.ItemInfo shatteringVenus;

        public float pickTileDamage;

        public sbyte gravityTile;

        public float darkness;

        public bool accHyperJet;
        public bool accShowQuestFish;
        public bool accPriceMonocle;

        public int equippedMask;
        public int cMask;
        public int equippedHat;
        public int cHat;
        public int equippedEyes;
        public int cEyes;
        public int equippedEars;
        public int cEars;

        public int leechHookNPC;

        public int accArmFloaties;

        public byte omniPaint;
        public bool omnibait; // To Do: Make this flag force ALL mod biomes to randomly be toggled on/off or something.

        public bool ZoneCrabCrevice => Player.InModBiome<CrabCreviceBiome>();
        public bool ZoneGaleStreams => Player.InModBiome<GaleStreamsBiome>();
        public bool ZoneGlimmer => Player.InModBiome<GlimmerBiome>();
        public bool ZonePeacefulGlimmer => Player.InModBiome<PeacefulGlimmerBiome>();
        public bool ZoneDemonSiege => Player.InModBiome<DemonSiegeBiome>();
        public bool ZoneGoreNest => Player.InModBiome<GoreNestBiome>();

        /// <summary>
        /// A point determining one of the close gore nests. Prioritized by their order in <see cref="DemonSiegeSystem.ActiveSacrifices"/>
        /// </summary>
        public Point eventDemonSiege;

        public bool hurt;
        public bool grounded;

        /// <summary>
        /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/> -> <see cref="ClosestEnemy"/>
        /// </summary>
        public int closestEnemy;
        public int closestEnemyOld;

        private DebuffInflictionStats debuffs;
        public ref DebuffInflictionStats DebuffsInfliction => ref debuffs;
        public float buffDuration;
        public float debuffDuration;

        public bool accSentrySlot;
        public Item accNeonFish;
        public int accWarHorn;

        public int instaShieldTime;
        public int instaShieldTimeMax;
        public int instaShieldCooldown;
        public float instaShieldAlpha;

        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Used by <see cref="Buffs.NoonBuff"/> and set to 1</para>
        /// </summary>
        public byte forceDayState;

        /// <summary>
        /// A percentage chance for a successful scam, where you don't consume money. Values below or equal 0 mean no scams, Values above or equal 1 mean 100% scam rate. Used by <see cref="FaultyCoin"/>
        /// </summary>
        public float accFaultyCoin;
        /// <summary>
        /// A flat discount variable. Decreases shop prices by this amount. Used by <see cref="ForgedCard"/>
        /// </summary>
        public int accForgedCard;
        /// <summary>
        /// Rerolls luck (rounded down amt of luckRerolls) times, if there is a decimal left, then it has a (luckRerolls decimal) chance of rerolling again.
        /// <para>Used by <see cref="RabbitsFoot"/></para> 
        /// </summary>
        public float luckRerolls;
        /// <summary>
        /// Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.
        /// <para>Used by <see cref="GrandReward"/></para> 
        /// </summary>
        public float dropRerolls;
        /// <summary>
        /// An amount of regen to add to the player in <see cref="UpdateLifeRegen"/>
        /// </summary>
        public int increasedRegen;

        public float accPreciseCrits;
        public Item accDavyJonesAnchor;
        public int davyJonesAnchorStack;

        public int accLittleInferno;

        public int accGroundCrownCrit;
        public float accDarknessCrownDamage;

        public int accBloodCrownSlot;

        public float antiGravityItemRadius;

        public int accFrostburnTurretSquid;
        public float bloodDiceDamage;
        public int bloodDiceMoney;
        public bool accGrandReward;
        public int accBoneBurningRing;
        public int accBoneRing;

        public int accBlackPhial;
        public int cdBlackPhial;

        public bool accDevilsTongue;

        public Item accRamishroom;

        public Item accHyperCrystal;
        public int cHyperCrystal;
        public int hyperCrystalCooldown;
        public int hyperCrystalCooldownMelee;
        public int hyperCrystalCooldownMax;

        public Item setSeraphim;

        public Item setGravetender;
        public int gravetenderGhost;

        public float zombieDebuffMultiplier;

        public Item accSentrySquid;
        public int sentrySquidTimer;

        public Item accMendshroom;
        public int cMendshroom;

        public Item accCelesteTorus;
        public float celesteTorusDamage;
        public int cCelesteTorus;

        /// <summary>
        /// Set by <see cref="SantankSentry"/> and <see cref="MechsSentry"/>
        /// </summary>
        public Item accSentryInheritence;
        public Item accAmmoRenewalPack;
        public Item accMothmanMask;

        public int accGlowCore;
        public int cGlowCore;
        public bool hasExpertBoost;
        /// <summary>
        /// Set to true by <see cref="TheReconstruction"/>
        /// </summary>
        public bool accExpertBoost;
        public int expertBoostWormScarfTimer;
        public bool expertBoostBoCProbesHurtSignal;
        public int expertBoostBoCProjDefense;
        public int expertBoostBoCTimer;
        public int expertBoostBoCDefense;

        public bool accRitualSkull;
        /// <summary>
        /// Set by <see cref="FoolsGoldRing"/>
        /// </summary>
        public int accFoolsGoldRing;

        /// <summary>
        /// Set to true by <see cref="Items.Armor.Passive.DartTrapHat"/>, <see cref="Items.Armor.Passive.SuperDartTrapHat"/>, <see cref="Items.Armor.Passive.FlowerCrown"/>
        /// </summary>
        public bool wearingSummonHelmet;
        /// <summary>
        /// Used by summon helmets (<see cref="Items.Armor.Passive.DartTrapHat"/>, <see cref="Items.Armor.Passive.SuperDartTrapHat"/>, <see cref="Items.Armor.Passive.FlowerCrown"/>) to time projectile spawns and such.
        /// </summary>
        public int summonHelmetTimer;

        public bool hasSkeletonKey => Player.HasItemInInvOrVoidBag(ModContent.ItemType<SkeletonKey>());

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
        public uint netInteractionCooldown;

        public int soulLimit;

        public int turretSlotCount;

        public float ghostHealthDR;
        public int ghostShadowDash;
        public int ghostChains;

        public int ghostSlotsMax;
        public int ghostSlotsOld;
        public int ghostSlots;
        public int ghostProjExtraUpdates;
        public int ghostLifespan;

        public int timeSinceLastHit;
        public int idleTime;

        public List<int> boundedPotionIDs;

        public bool ExpertBoost => hasExpertBoost || accExpertBoost;
        public bool MaxLife => Player.statLife >= Player.statLifeMax2;
        public float LifeRatio => Player.statLife / (float)Player.statLifeMax2;

        /// <summary>
        /// Helper for whether or not the player currently has a cooldown
        /// </summary>
        public bool HasCooldown => itemCooldown > 0;
        /// <summary>
        /// Helper for whether or not the player is in danger
        /// </summary>
        public bool InDanger => closestEnemy != -1;
        public bool InDarkness => darkness > 0.8f;

        public override void Load()
        {
            LoadHooks();
            SpawnEnchantmentDusts_Custom = new List<(int, Func<Player, bool>, Action<Dust>)>();
            Player_ItemCheck_Shoot = typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
            KeybindSetbonusAlt = KeybindLoader.RegisterKeybind(Mod, "Armor Setbonus Alt", Keys.V);
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
            clone.boundedPotionIDs = new List<int>(boundedPotionIDs);
            clone.darkness = darkness;
            clone.gravetenderGhost = gravetenderGhost;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var aequus = (AequusPlayer)clientPlayer;

            var bb = new BitsByte(
                darkness != aequus.darkness,
                timeSinceLastHit != aequus.timeSinceLastHit,
                gravetenderGhost != aequus.gravetenderGhost,
                omniPaint != aequus.omniPaint,
                (aequus.itemCombo - itemCombo).Abs() > 3 || (aequus.itemSwitch - itemSwitch).Abs() > 3 || (aequus.itemUsage - itemUsage).Abs() > 3 || (aequus.itemCooldown - itemCooldown).Abs() > 3 || aequus.itemCooldownMax != itemCooldownMax,
                aequus.instaShieldTime != instaShieldTime,
                boundedPotionIDs.IsTheSameAs(aequus.boundedPotionIDs),
                boundBowAmmo != aequus.boundBowAmmo || boundBowAmmoTimer != aequus.boundBowAmmoTimer);

            var bb2 = new BitsByte(
                summonHelmetTimer != aequus.summonHelmetTimer);

            if (bb > 0 || bb2 > 0)
            {
                var p = Aequus.GetPacket(PacketType.SyncAequusPlayer);
                p.Write((byte)Player.whoAmI);
                p.Write(bb);
                p.Write(bb2);
                if (bb[0])
                {
                    p.Write(darkness);
                }
                if (bb[1])
                {
                    p.Write(timeSinceLastHit);
                }
                if (bb[2])
                {
                    p.Write(gravetenderGhost);
                }
                if (bb[3])
                {
                    p.Write(omniPaint);
                }
                if (bb[4])
                {
                    p.Write(itemCombo);
                    p.Write(itemSwitch);
                    p.Write(itemUsage);
                    p.Write(itemCooldown);
                    p.Write(itemCooldownMax);
                }
                if (bb[5])
                {
                    p.Write(instaShieldTime);
                }
                if (bb[6])
                {
                    p.Write(boundedPotionIDs.Count);
                    for (int i = 0; i < boundedPotionIDs.Count; i++)
                    {
                        p.Write(boundedPotionIDs[i]);
                    }
                }
                if (bb[7])
                {
                    p.Write(boundBowAmmo);
                    p.Write(boundBowAmmoTimer);
                }
                if (bb2[0])
                {
                    p.Write(summonHelmetTimer);
                }
                p.Send();
            }
        }

        public void RecieveChanges(BinaryReader reader)
        {
            var bb = (BitsByte)reader.ReadByte();
            var bb2 = (BitsByte)reader.ReadByte();
            if (bb[0])
            {
                darkness = reader.ReadSingle();
            }
            if (bb[1])
            {
                timeSinceLastHit = reader.ReadInt32();
            }
            if (bb[2])
            {
            }
            if (bb[3])
            {
                omniPaint = reader.ReadByte();
            }
            if (bb[4])
            {
                itemCombo = reader.ReadUInt16();
                itemSwitch = reader.ReadUInt16();
                itemUsage = reader.ReadUInt16();
                itemCooldown = reader.ReadUInt16();
                itemCooldownMax = reader.ReadUInt16();
            }
            if (bb[5])
            {
                instaShieldTime = reader.ReadInt32();
            }
            if (bb[6])
            {
                boundedPotionIDs.Clear();
                int count = reader.ReadInt32();
                for (int i = 0; i < Main.maxBuffTypes; i++)
                {
                    boundedPotionIDs.Add(reader.ReadInt32());
                }
            }
            if (bb[7])
            {
                boundBowAmmo = reader.ReadInt32();
                boundBowAmmoTimer = reader.ReadInt32();
            }
            if (bb2[0])
            {
                summonHelmetTimer = reader.ReadInt32();
            }
        }

        public override void Initialize()
        {
            boundedPotionIDs = new List<int>();
            accBloodCrownSlot = -1;
            debuffs = new DebuffInflictionStats(0);
            //shatteringVenus = new ShatteringVenus.ItemInfo();
            accGlowCore = 0;
            cGlowCore = -1;
            instaShieldAlpha = 0f;
            gravityTile = 0;
            boundBowAmmo = BoundBowMaxAmmo;
            boundBowAmmoTimer = 60;
            CursorDye = -1;
            ghostTombstones = false;
            moroSummonerFruit = false;
            hasUsedRobsterScamItem = false;

            sentrySquidTimer = 120;
            itemCooldown = 0;
            itemCooldownMax = 0;
            itemCombo = 0;
            itemSwitch = 0;
            netInteractionCooldown = 60;
            closestEnemyOld = -1;
            closestEnemy = -1;
        }

        public override void UpdateDead()
        {
            timeSinceLastHit = 0;
            hasExpertBoost = false;
            accExpertBoost = false;
            foreach (var buff in boundedPotionIDs)
            {
                Main.persistentBuff[buff] = true;
            }
        }

        public override void OnRespawn(Player player)
        {
            if (maxLifeRespawnReward)
            {
                player.statLife = Math.Max(player.statLife, player.statLifeMax2);
            }
        }

        public void ResetArmor()
        {
            setSeraphim = null;
            setGravetender = null;

            ghostChains = 0;
            ghostHealthDR = 0f;
            ghostShadowDash = 0;
            accGlowCore = 0;
            accHyperJet = false;
            accSentrySlot = false;
            accGroundCrownCrit = 0;
            accDarknessCrownDamage = 0f;
            accBloodCrownSlot = -1;
            accShowQuestFish = false;
            accPriceMonocle = false;
            accNeonFish = null;
            accPreciseCrits = 0f;
            accArmFloaties = 0;
            accDavyJonesAnchor = null;
            davyJonesAnchorStack = 0;
            accWarHorn = 0;
            accLittleInferno = 0;
            accRitualSkull = false;
            accRamishroom = null;
            zombieDebuffMultiplier = 0f;
            bloodDiceMoney = 0;
            bloodDiceDamage = 0f;
            accHyperCrystal = null;
            hyperCrystalCooldownMax = 0;
            if (hyperCrystalCooldownMelee > 0)
                hyperCrystalCooldownMelee--;
            if (hyperCrystalCooldown > 0)
                hyperCrystalCooldown--;

            accMendshroom = null;

            accCelesteTorus = null;
            celesteTorusDamage = 1f;

            accAmmoRenewalPack = null;
            accMothmanMask = null;
            accSentryInheritence = null;

            accFaultyCoin = 0f;
            accForgedCard = 0;

            if (cdBlackPhial > 0)
                cdBlackPhial--;
            accBlackPhial = 0;
            accBoneRing = 0;
            dropRerolls = 0f;
            accDevilsTongue = false;
            accGrandReward = false;
            accFoolsGoldRing = 0;

            hasExpertBoost = accExpertBoost;
            accExpertBoost = false;

            accSentrySquid = null;
            if (!InDanger)
            {
                sentrySquidTimer = Math.Min(sentrySquidTimer, 240);
            }
            if (sentrySquidTimer > 0)
            {
                sentrySquidTimer--;
            }

            if (expertBoostWormScarfTimer > 0)
            {
                expertBoostWormScarfTimer--;
            }
            expertBoostBoCProjDefense = expertBoostBoCDefense;
        }

        public void ResetStats()
        {
            if (boundedPotionIDs == null)
            {
                boundedPotionIDs = new List<int>();
            }
            else if (boundedPotionIDs.Count > 0)
            {
                foreach (var buff in boundedPotionIDs)
                {
                    Main.persistentBuff[buff] = false;
                }
            }
            debuffs.ResetEffects(Player);
            buffDuration = 1f;
            debuffDuration = 1f;
            luckRerolls = 0;
            antiGravityItemRadius = 0f;
            soulLimit = 0;
            pickTileDamage = 1f;
            ghostSlotsMax = 1;
            ghostProjExtraUpdates = 0;
            ghostLifespan = 3600;
        }

        public void UpdateInstantShield()
        {
            if ((hurt || instaShieldTime < instaShieldTimeMax) && instaShieldTime > 0)
            {
                if (instaShieldTime == instaShieldTimeMax)
                {
                    SoundEngine.PlaySound(SoundID.Item75.WithPitch(1f).WithVolume(0.75f), Player.Center);
                }
                instaShieldTime--;
                if (instaShieldTime == 0)
                {
                    instaShieldTime = -1;
                }
                if (instaShieldAlpha < 1f)
                {
                    instaShieldAlpha += 0.035f;
                    if (instaShieldAlpha > 1f)
                    {
                        instaShieldAlpha = 1f;
                    }
                }
            }
            else
            {
                if (instaShieldTime == 0)
                {
                    instaShieldTime = instaShieldTimeMax;
                }
                if (instaShieldTime < instaShieldTimeMax)
                {
                    instaShieldTime = -1;
                    int instaShieldCooldownBuffIndex = Player.FindBuffIndex(ModContent.BuffType<FlashwayNecklaceCooldown>());
                    if (instaShieldCooldownBuffIndex == -1)
                    {
                        if (Main.myPlayer == Player.whoAmI)
                            Player.AddBuff(ModContent.BuffType<FlashwayNecklaceCooldown>(), instaShieldCooldown);
                    }
                    else if (Player.buffTime[instaShieldCooldownBuffIndex] <= 2)
                    {
                        instaShieldTime = instaShieldTimeMax;
                    }
                }
                if (instaShieldAlpha > 0f)
                {
                    instaShieldAlpha -= 0.035f;
                    if (instaShieldAlpha < 0f)
                    {
                        instaShieldAlpha = 0f;
                    }
                }
            }
            instaShieldTimeMax = 0;
        }

        public void CheckGravityBlocks()
        {
            bool swapGravity = gravityTile != 0;
            gravityTile = AequusTile.GetGravityTileStatus(Player.Center);
            if (swapGravity)
            {
                int gravity = gravityTile < 0 ? -1 : 1;
                if (Player.gravDir != gravity)
                {
                    Player.gravDir = gravity;
                    SoundEngine.PlaySound(SoundID.Item8, Player.position);
                }
            }
        }

        public void ResetDyables()
        {
            equippedMask = 0;
            cMask = 0;
            equippedHat = 0;
            cHat = 0;
            equippedEyes = 0;
            cEyes = 0;
            equippedEars = 0;
            cEars = 0;
            cGlowCore = -1;
            cHyperCrystal = 0;
            cMendshroom = 0;
            cCelesteTorus = 0;
        }

        public void UpdateItemFields()
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
        }

        public override void ResetEffects()
        {
            PlayerContext = Player.whoAmI;

            UpdateInstantShield();
            ResetDyables();
            ResetArmor();
            ResetStats();
            cursorDye = -1;
            cursorDyeOverride = 0;

            if (gravityTile != 0)
            {
                Player.gravControl = false;
                Player.gravControl2 = false;
            }

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<LeechHookProj>()] <= 0)
                leechHookNPC = -1;

            if (Player.velocity.Length() < 1f)
            {
                idleTime++;
            }
            else
            {
                idleTime = 0;
            }

            UpdateItemFields();
            if (netInteractionCooldown > 0)
            {
                netInteractionCooldown--;
            }

            forceDayState = 0;
            Team = Player.team;
            hurt = false;
        }

        public override void PreUpdate()
        {
            projectileIdentity = -1;
            if (forceDayState == 1)
            {
                AequusSystem.Main_dayTime.StartCaching(true);
            }
            else if (forceDayState == 2)
            {
                AequusSystem.Main_dayTime.StartCaching(false);
            }
            forceDayState = 0;

            eventDemonSiege = DemonSiegeSystem.FindDemonSiege(Player.Center);
        }

        public override void PreUpdateBuffs()
        {
            timeSinceLastHit++;
        }

        public override void PostUpdateBuffs()
        {
            for (int i = 0; i < boundedPotionIDs.Count; i++)
            {
                if (!Player.HasBuff(boundedPotionIDs[i]))
                {
                    boundedPotionIDs.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (setGravetender != null)
            {
                GravetenderHood.ActivateGravetenderWisp(Player, this);
            }
        }

        public override void PostUpdateEquips()
        {
            CheckGravityBlocks();

            if (whitePhial)
            {
                buffDuration += 0.25f;
            }

            if (accRitualSkull)
            {
                ghostSlotsMax += Player.maxMinions - 1;
                Player.maxMinions = 1;
            }

            if (Player.HasItem(ItemID.VoidLens))
            {
                UpdateBank(Player.bank4, 3);
            }
            if (setSeraphim != null && ghostSlots == 0)
            {
                Player.endurance += 0.3f;
            }

            if (accBloodCrownSlot != -1)
            {
                HandleSlotBoost(Player.armor[accBloodCrownSlot], accBloodCrownSlot < 10 ? Player.hideVisibleAccessory[accBloodCrownSlot] : false);
            }

            if (accDarknessCrownDamage > 0f && InDarkness)
            {
                Player.GetDamage(DamageClass.Generic) += accDarknessCrownDamage;
            }
            grounded = false;
            if (accGroundCrownCrit > 0 && Player.velocity.Y == 0f && Player.oldVelocity.Y == 0f)
            {
                grounded = true;
                Player.GetCritChance(DamageClass.Generic) += accGroundCrownCrit;
            }
            if (gravityTile != 0)
            {
                Player.gravity = 0.4f;
            }
        }
        public void HandleSlotBoost(Item item, bool hideVisual)
        {
            if (item.IsAir)
                return;
            int slotBoostCurseOld = accBloodCrownSlot;
            accBloodCrownSlot = -2;
            item.Aequus().accStacks++;
            Player.ApplyEquipFunctional(item, hideVisual);
            accBloodCrownSlot = slotBoostCurseOld;

            if (item.wingSlot != -1)
            {
                Player.wingTimeMax *= 2;
            }
            Player.statDefense += item.defense;
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
                    if (bank.item[i].ModItem is ItemHooks.IUpdateVoidBag b)
                    {
                        b.UpdateBank(Player, this, i, bankType);
                    }
                }
            }
        }

        public override bool PreItemCheck()
        {
            if (AequusSystem.Main_dayTime.IsCaching)
                AequusSystem.Main_dayTime.RepairCachedStatic();
            return true;
        }

        public override void PostItemCheck()
        {
            if (AequusSystem.Main_dayTime.IsCaching)
                AequusSystem.Main_dayTime.DisrepairCachedStatic();
            if (Player.selectedItem != lastSelectedItem)
            {
                lastSelectedItem = Player.selectedItem;
                itemSwitch = Math.Max((ushort)30, itemSwitch);
                itemUsage = 0;
                itemHits = 0;
            }
            CountSentries();
        }

        public override void PostUpdate()
        {
            if (Main.netMode != NetmodeID.Server && !Player.outOfRange)
                DoDebuffEffects();

            if (antiGravityItemRadius > 0f)
            {
                var position = Player.Center;
                for (int i = 0; i < Main.maxItems; i++)
                {
                    if (Main.item[i].active && !ItemID.Sets.ItemNoGravity[Main.item[i].type]
                        && Vector2.Distance(Main.item[i].Center, position) < antiGravityItemRadius)
                    {
                        Main.item[i].Aequus().noGravityTime = 30;
                    }
                }
            }

            if (accGlowCore > 0)
            {
                GlowCore.AddLight(Player.Center, Player, this);
            }

            if (accMendshroom != null && accMendshroom.shoot > ProjectileID.None && ProjectilesOwned(accMendshroom.shoot) <= 10)
            {
                if (Main.rand.NextBool((int)Math.Clamp(360 * LifeRatio / accMendshroom.Aequus().accStacks, 120f, 600f)))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var randomSpot = Player.Center + new Vector2(Main.rand.NextFloat(-280f, 280f), Main.rand.NextFloat(-280f, 280f));
                        if (Player.Distance(randomSpot) < 100f)
                            continue;
                        if (!Collision.SolidCollision(randomSpot, 2, 2) && Collision.CanHitLine(randomSpot, 2, 2, Player.position, Player.width, Player.height))
                        {
                            Projectile.NewProjectile(Player.GetSource_Accessory(accMendshroom), randomSpot, Vector2.Zero, accMendshroom.shoot,
                                0, 0f, Player.whoAmI, ai1: projectileIdentity + 1);
                            break;
                        }
                    }
                }
            }

            if (Main.myPlayer == Player.whoAmI)
            {
                NecromancyNPC.CheckZombies--;
                UpdateMaxZombies();
            }

            ghostSlotsOld = ghostSlots;
            ghostSlots = 0;
            ClosestEnemy();
            Team = 0;

            if (setGravetender != null)
            {
                if (setGravetender.shoot > ProjectileID.None && Player.ownedProjectileCounts[setGravetender.shoot] <= 0)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(setGravetender), Player.Center, -Vector2.UnitY, setGravetender.shoot,
                        Player.GetWeaponDamage(setGravetender), Player.GetWeaponKnockback(setGravetender), Player.whoAmI);
                }
                if (setGravetender.buffType > 0)
                {
                    Player.AddBuff(setGravetender.buffType, 2, quiet: true);
                }
                if (gravetenderGhost > -1 && (!Main.npc[gravetenderGhost].active || !Main.npc[gravetenderGhost].friendly || Main.npc[gravetenderGhost].townNPC || 
                    !Main.npc[gravetenderGhost].TryGetGlobalNPC<NecromancyNPC>(out var zombie) || !zombie.isZombie || zombie.zombieOwner != Player.whoAmI))
                {
                    gravetenderGhost = -1;
                }
            }
            else
            {
                gravetenderGhost = -1;
            }

            if (accSentrySquid != null && sentrySquidTimer == 0)
            {
                SentrySquid.UseEquip(Player, this);
            }

            if (accSentryInheritence != null)
            {
                UpdateSantankSentry();
            }

            if (!accExpertBoost || Player.brainOfConfusionItem == null)
            {
                expertBoostBoCDefense = 0;
                expertBoostBoCTimer = 0;
            }

            UpdateBoundBowRecharge();

            if (Main.myPlayer == Player.whoAmI)
            {
                darkness = GetDarkness();
            }

            PlayerContext = -1;

            if (AequusSystem.Main_dayTime.IsCaching)
            {
                AequusSystem.Main_dayTime.EndCaching();
            }
        }
        public void DoDebuffEffects()
        {
            if (Player.HasBuff<BlueFire>())
            {
                int amt = (int)(Player.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.ParticlesAbovePlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(Player.getRect()) + Main.rand.NextVector2Unit() * 8f, -Player.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(5, 20, 40, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
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

        public void UpdateBoundBowRecharge()
        {
            if (boundBowAmmoTimer > 0)
                boundBowAmmoTimer--;
            if (boundBowAmmoTimer <= 0)
            {
                bool selected = Main.myPlayer == Player.whoAmI && Player.HeldItem.ModItem is BoundBow;
                if (Main.netMode != NetmodeID.Server)
                {
                    float volume = 0.2f;
                    if (selected)
                    {
                        volume = 0.55f;
                        EffectsSystem.Shake.Set(4);
                    }
                    SoundEngine.PlaySound(Aequus.GetSound("Item/boundBowRecharge").WithVolume(volume));

                    Vector2 widthMethod(float p) => new Vector2(16f) * (float)Math.Sin(p * MathHelper.Pi);
                    Color colorMethod(float p) => Color.BlueViolet.UseA(150) * 1.1f;

                    for (int i = 0; i < 8; i++)
                    {
                        var d = Dust.NewDustPerfect(Player.position + new Vector2(Player.width * 2f * Main.rand.NextFloat(1f) - Player.width / 2f, Player.height * Main.rand.NextFloat(0.2f, 1.2f)), ModContent.DustType<MonoSparkleDust>(),
                            new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4.5f, -1f)), newColor: Color.BlueViolet.UseA(25), Scale: Main.rand.NextFloat(0.5f, 1.25f));
                        d.fadeIn = d.scale + 0.5f;
                        d.color *= d.scale;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        var prim = new TrailRenderer(TextureCache.Trail[3].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                        var v = new Vector2(Player.width * 2f / 3f * i - Player.width / 2f + Main.rand.NextFloat(-6f, 6f), Player.height * Main.rand.NextFloat(0.9f, 1.2f));
                        var particle = new BoundBowTrailParticle(prim, Player.position + v, new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-10f, -8f)),
                            scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10, drawDust: false);
                        particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                        particle.prim.GetColor = (p) => colorMethod(p) * Math.Min(particle.Scale, 1.5f);
                        EffectsSystem.ParticlesAbovePlayers.Add(particle);
                        if (i < 2)
                        {
                            prim = new TrailRenderer(TextureCache.Trail[3].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                            particle = new BoundBowTrailParticle(prim, Player.position + new Vector2(Player.width * i, Player.height * Main.rand.NextFloat(0.9f, 1.2f) + 10f), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-12.4f, -8.2f)),
                            scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10, drawDust: false);
                            particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                            particle.prim.GetColor = (p) => new Color(35, 10, 125, 150) * Math.Min(particle.Scale, 1.5f);
                            EffectsSystem.ParticlesBehindPlayers.Add(particle);
                        }
                    }
                }
                boundBowAmmo++;
                boundBowAmmoTimer = BoundBowRegenerationDelay;
            }
            if (boundBowAmmo >= BoundBowMaxAmmo)
            {
                boundBowAmmoTimer = BoundBowRegenerationDelay;
            }
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
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, 9999 + Main.npc[i].lifeMax * 2 + Main.npc[i].defense * 2);
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

                        //Aequus.Instance.Logger.Debug("NPC: " + Lang.GetNPCName(Main.npc[removeNPC].type) + ", WhoAmI: " + removeNPC + ", Tier:" + Main.npc[removeNPC].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier);
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
                    if (Main.projectile[i].active && Main.projectile[i].TryGetGlobalProjectile<SentryAccessoriesManager>(out var sentry))
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
            if (accBloodCrownSlot != -1)
            {
                Player.lifeRegen = Math.Min(Player.lifeRegen, 0);
                Player.lifeRegenTime = Math.Min(Player.lifeRegenTime, 0);
            }
            if (Player.HasBuff<BlueFire>())
                Player.AddLifeRegen(-6);
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            hurt = true;
            if (damage >= 1000)
            {
                return true;
            }

            if (instaShieldTime > 0)
            {
                return false;
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

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            timeSinceLastHit = 0;
            if (Player.HasBuff(ModContent.BuffType<RitualBuff>()))
            {
                SoundEngine.PlaySound(SoundID.NPCDeath39, Player.Center);
                Player.ClearBuff(ModContent.BuffType<RitualBuff>());
            }
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
            return accFaultyCoin > 0f || accForgedCard > 0;
        }
        public bool MoneyBack(NPC vendor, Item[] shopInventory, Item item)
        {
            if (Main.rand.NextFloat() < accFaultyCoin)
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
            EffectsSystem.UpdateScreenPosition();
            Main.screenPosition = Main.screenPosition.Floor();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (npc.HasBuff<Weakness>())
            {
                damage = (int)(damage * WeaknessDamageMultiplier);
            }

            if (ghostHealthDR > 0f)
            {
                float amount = 1f - ghostHealthDR;
                var list = new List<Point>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].IsZombieAndInteractible(Player.whoAmI) && !Main.npc[i].GetGlobalNPC<NecromancyNPC>().statFreezeLifespan)
                    {
                        list.Add(new Point(i, Main.npc[i].GetGlobalNPC<NecromancyNPC>().DespawnPriority(Main.npc[i])));
                    }
                }
                if (list.Count != 0)
                {
                    int damageToRemove = (int)(damage * amount);
                    damage -= damageToRemove;
                    list.Sort((npc, npc2) => npc.Y.CompareTo(npc2.Y));
                    while (list.Count > 0 && damageToRemove > 0)
                    {
                        var n = Main.npc[list[0].X];
                        int life = (int)(n.lifeMax * n.GetGlobalNPC<NecromancyNPC>().ZombieLifespanPercentage);
                        life -= damageToRemove;
                        if (life < 0)
                        {
                            damageToRemove = -life;
                            n.KillMe(quiet: false);
                        }
                        else
                        {
                            damageToRemove = 0;
                            n.GetGlobalNPC<NecromancyNPC>().zombieTimer = (int)(life / (float)n.lifeMax * n.GetGlobalNPC<NecromancyNPC>().zombieTimerMax);
                        }
                        list.RemoveAt(0);
                    }
                    damage += damageToRemove;
                }
            }

            if (npc.Aequus().heatDamage && Player.HasBuff<FrostBuff>())
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
            if (proj.Aequus().heatDamage && Player.HasBuff<FrostBuff>())
            {
                damage = (int)(damage * FrostPotionDamageMultiplier);
            }
        }

        public bool CheckBloodDice(ref int damage)
        {
            if (bloodDiceDamage > 0f)
            {
                if (bloodDiceMoney > 0)
                {
                    if (!Player.CanBuyItem(bloodDiceMoney))
                    {
                        return false;
                    }
                    SoundEngine.PlaySound(SoundID.Coins);
                    Player.BuyItem(bloodDiceMoney);
                }
                damage = (int)(damage * (1f + bloodDiceDamage / 2f));
            }
            return false;
        }

        public void CheckSeraphimSet(NPC target, Projectile proj, ref int damage)
        {
            if (setSeraphim != null && ghostSlots < ghostSlotsMax && target.lifeMax < 1000 && target.defense < 100)
            {
                float threshold = 1f - ghostSlots * 0.2f;
                if (threshold > 0 && LifeRatio <= threshold && NecromancyDatabase.TryGet(target, out var info) && info.EnoughPower(3.1f))
                {
                    var zombie = target.GetGlobalNPC<NecromancyNPC>();
                    zombie.conversionChance = 1;
                    zombie.zombieDebuffTier = 3.1f;
                    zombie.zombieOwner = Player.whoAmI;
                    zombie.renderLayer = ColorTargetID.BloodRed;
                    damage = 2500;
                }
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (!target.immortal && crit)
            {
                CheckBloodDice(ref damage);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (accPreciseCrits > 0f)
            {
                var difference = target.Center - proj.Center;
                var comparisonPosition = proj.Center + Vector2.Normalize(proj.velocity).UnNaN() * difference.Length().UnNaN();
                if (Vector2.Distance(target.Center, comparisonPosition) < 8f * accPreciseCrits)
                {
                    crit = true;
                }
            }
            if (!target.immortal && crit)
            {
                CheckBloodDice(ref damage);
            }
            CheckSeraphimSet(target, proj, ref damage);
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type != NPCID.TargetDummy)
                CheckLeechHook(target, damage);
            OnHitEffects(target, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type != NPCID.TargetDummy)
                CheckLeechHook(target, damage);
            OnHitEffects(target, damage, knockback, crit);
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
        public void OnHitEffects(NPC target, int damage, float knockback, bool crit)
        {
            int deathsEmbrace = Player.FindBuffIndex(ModContent.BuffType<DeathsEmbraceBuff>());
            if (deathsEmbrace != -1)
            {
                Player.buffTime[deathsEmbrace] = Math.Max(Player.buffTime[deathsEmbrace], 300);
            }

            if (accDavyJonesAnchor != null && Main.myPlayer == Player.whoAmI)
            {
                int amt = accDavyJonesAnchor.Aequus().accStacks;
                if (Player.RollLuck(Math.Max((8 - damage / 20 + Player.ownedProjectileCounts[ModContent.ProjectileType<DavyJonesAnchorProj>()] * 4) / amt, 1)) == 0)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(accDavyJonesAnchor), target.Center, Main.rand.NextVector2Unit() * 8f,
                        ModContent.ProjectileType<DavyJonesAnchorProj>(), 15, 2f, Player.whoAmI, ai0: target.whoAmI);
                }
            }

            if (accLittleInferno > 0)
            {
                target.AddBuff(BuffID.OnFire, 240 * accLittleInferno);
                if (crit)
                {
                    target.AddBuff(BuffID.OnFire3, 180 * accLittleInferno);
                }
            }

            if (accMothmanMask != null && Player.statLife >= Player.statLifeMax2 && crit)
            {
                AequusBuff.ApplyBuff<BlueFire>(target, 300 * accMothmanMask.Aequus().accStacks, out bool canPlaySound);
                if (canPlaySound)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        PacketSystem.SyncSound(SoundPacket.InflictBurning, target.Center);
                    }
                    SoundEngine.PlaySound(BlueFire.InflictDebuffSound, target.Center);
                }
            }
            if (accBlackPhial > 0)
            {
                int buffCount = 0;
                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    if (target.buffType[i] > 0 && Main.debuff[target.buffType[i]])
                    {
                        buffCount++;
                    }
                }
                if (Main.rand.NextBool(Math.Max(4 / accBlackPhial + cdBlackPhial / 5 + buffCount * 2, 1)))
                {
                    int buff = Main.rand.Next(BlackPhial.DebuffsAfflicted);
                    if (!target.buffImmune[buff])
                    {
                        cdBlackPhial += 30;
                        target.AddBuff(buff, 150);
                    }
                }
            }
            if (accBoneBurningRing > 0)
            {
                target.AddBuff(BuffID.OnFire3, 360 * accBoneBurningRing);
            }
            if (accBoneRing > 0 && Main.rand.NextBool(Math.Max(4 / accBoneRing, 1)))
            {
                target.AddBuff(ModContent.BuffType<Weakness>(), 360);
            }
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (accRamishroom != null && item.fishingPole > 0)
            {
                int amt = accRamishroom.Aequus().accStacks;
                for (int i = 0; i < amt; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(accRamishroom), position, velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)),
                        ModContent.ProjectileType<RamishroomBobber>(), damage, knockback, Player.whoAmI);
                }
            }
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadData(TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (!Main.gameMenu && Player.HeldItem.ModItem is ItemHooks.IPreDrawPlayer preDrawPlayer)
            {
                preDrawPlayer.PreDrawPlayer(Player, this, ref drawInfo);
            }
            if (Player.front > 0 && Player.front == PumpkingCloak.FrontID)
            {
                drawInfo.hidesBottomSkin = true;
                drawInfo.hidesTopSkin = true;
            }
            if (CustomDrawShadow != null)
            {
                drawInfo.shadow = CustomDrawShadow.Value;
                float val = 1f - CustomDrawShadow.Value;
                drawInfo.colorArmorBody *= val;
                drawInfo.colorArmorHead *= val;
                drawInfo.colorArmorLegs *= val;
                drawInfo.colorBodySkin *= val;
                drawInfo.colorElectricity *= val;
                drawInfo.colorEyes *= val;
                drawInfo.colorEyeWhites *= val;
                drawInfo.colorHair *= val;
                drawInfo.colorHead *= val;
                drawInfo.colorLegs *= val;
                drawInfo.colorMount *= val;
                drawInfo.colorPants *= val;
                drawInfo.colorShirt *= val;
                drawInfo.colorShoes *= val;
                drawInfo.colorUnderShirt *= val;
                drawInfo.ArkhalisColor *= val;
                drawInfo.armGlowColor *= val;
                drawInfo.bodyGlowColor *= val;
                drawInfo.floatingTubeColor *= val;
                drawInfo.headGlowColor *= val;
                drawInfo.itemColor *= val;
                drawInfo.legsGlowColor *= val;
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (Player.front > 0 && Player.front == PumpkingCloak.FrontID)
            {
                PlayerDrawLayers.Torso.Hide();
                PlayerDrawLayers.Leggings.Hide();
            }
        }

        public float GetDarkness()
        {
            if (Main.myPlayer == Player.whoAmI) // Should always be true anyways, but here for safe-ness I guess.
            {
                var tilePosition = Player.Center.ToTileCoordinates();
                return Math.Clamp(1f - Lighting.Brightness(tilePosition.X, tilePosition.Y), 0f, 1f);
            }
            return 0f;
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
            if (info.headOnlyRender)
            {
                return;
            }
            if (DrawScale != null)
            {
                var drawPlayer = info.drawPlayer;
                var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
                to -= Main.screenPosition;
                for (int i = 0; i < info.DrawDataCache.Count; i++)
                {
                    DrawData data = info.DrawDataCache[i];
                    data.position -= (data.position - to) * (1f - DrawScale.Value);
                    data.scale *= DrawScale.Value;
                    info.DrawDataCache[i] = data;
                }
            }
            if (DrawForceDye != null)
            {
                var drawPlayer = info.drawPlayer;
                for (int i = 0; i < info.DrawDataCache.Count; i++)
                {
                    DrawData data = info.DrawDataCache[i];
                    data.shader = DrawForceDye.Value;
                    info.DrawDataCache[i] = data;
                }
            }
            if (instaShieldTimeMax != 0 && instaShieldTime == instaShieldTimeMax)
            {
                int heldItemStart = ModContent.GetInstance<DrawDataTrackers.DrawHeldItem_27_Tracker>().DDIndex;
                int heldItemEnd = ModContent.GetInstance<DrawDataTrackers.ArmOverItem_28_Tracker>().DDIndex;
                var info2 = info;
                info2.DrawDataCache = new List<DrawData>(info.DrawDataCache);
                for (int i = heldItemEnd; i >= heldItemStart; i--)
                {
                    info2.DrawDataCache.RemoveAt(i);
                }
                var ddCache = new List<DrawData>(info2.DrawDataCache);
                foreach (var c in AequusHelpers.CircularVector(4))
                {
                    for (int i = 0; i < info2.DrawDataCache.Count; i++)
                    {
                        var dd = ddCache[i];
                        dd.position += c * 2f;
                        dd.color = Color.SkyBlue.UseA(0) * 0.1f;
                        dd.shader = AequusHelpers.ColorOnlyShaderIndex;
                        info2.DrawDataCache[i] = dd;
                    }
                    PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref info2);
                }
            }
        }

        /// <summary>
        /// Called right after all player layers have been drawn
        /// </summary>
        /// <param name="info"></param>
        public void PostDraw(ref PlayerDrawSet info)
        {
            if (info.headOnlyRender)
            {
                return;
            }
            if (instaShieldAlpha > 0f)
            {
                int heldItemStart = ModContent.GetInstance<DrawDataTrackers.DrawHeldItem_27_Tracker>().DDIndex;
                int heldItemEnd = ModContent.GetInstance<DrawDataTrackers.ArmOverItem_28_Tracker>().DDIndex;
                var info2 = info;
                info2.DrawDataCache = new List<DrawData>(info.DrawDataCache);
                for (int i = heldItemEnd; i >= heldItemStart; i--)
                {
                    info2.DrawDataCache.RemoveAt(i);
                }
                var ddCache = new List<DrawData>(info2.DrawDataCache);
                for (int i = 0; i < info2.DrawDataCache.Count; i++)
                {
                    var dd = ddCache[i];
                    dd.color = Color.SkyBlue * 2f * instaShieldAlpha;
                    dd.shader = AequusHelpers.ColorOnlyShaderIndex;
                    info2.DrawDataCache[i] = dd;
                }
                PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref info2);
            }
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

        public void TryFillSoulGems(EnemyKillInfo npc)
        {
        }

        public void OnKillEffect(EnemyKillInfo npc)
        {
            SoulGem.TryFillSoulGems(Player, this, npc);
            AmmoBackpack.Proc(Player, this, npc);
            ArmFloaties.Proc(Player, this, npc);
        }

        public void DetermineBuffTimeToAdd(int type, ref int amt)
        {
            if (Main.debuff[type] && !AequusBuff.IsActuallyABuff.Contains(type))
            {
                if (debuffDuration != 1f)
                    amt = (int)(amt * debuffDuration);
            }
            else
            {
                if (buffDuration != 1f && !Main.meleeBuff[type])
                    amt = (int)(amt * buffDuration);
            }
        }

        public static bool CanScamNPC(NPC npc)
        {
            return npc.type != ModContent.NPCType<Exporter>();
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
                    Main.mouseY = (int)mousePos.Y;
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

        public static List<Item> GetEquips(Player player, bool armor = true, bool accessories = true, bool sentrySlot = false)
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
                if (sentrySlot && player.Aequus().accSentrySlot)
                {
                    var item = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<MechsSentryAccessorySlot>().Type, player);
                    if (item.FunctionalItem != null && !item.FunctionalItem.IsAir)
                        l.Add(item.FunctionalItem);
                }
            }
            return l;
        }

        #region Hooks
        private static void LoadHooks()
        {
            On.Terraria.Player.PlaceThing_PaintScrapper += Player_PlaceThing_PaintScrapper;
            On.Terraria.Player.TryPainting += Player_TryPainting;
            On.Terraria.GameContent.Golf.FancyGolfPredictionLine.Update += FancyGolfPredictionLine_Update;
            On.Terraria.Player.CheckSpawn += Player_CheckSpawn;
            On.Terraria.Player.JumpMovement += Player_JumpMovement;
            On.Terraria.Player.DropTombstone += Player_DropTombstone;
            On.Terraria.NPC.NPCLoot_DropMoney += NPC_NPCLoot_DropMoney;
            On.Terraria.GameContent.ItemDropRules.ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;
            On.Terraria.Player.RollLuck += Player_RollLuck;
            On.Terraria.Player.GetItemExpectedPrice += Hook_GetItemPrice;
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_RenderAllLayers += PlayerDrawLayers_DrawPlayer_RenderAllLayers;
            On.Terraria.Player.PickTile += Player_PickTile;
        }

        private static void Player_PlaceThing_PaintScrapper(On.Terraria.Player.orig_PlaceThing_PaintScrapper orig, Player player)
        {
            if (!ItemID.Sets.IsPaintScraper[player.inventory[player.selectedItem].type] || !(player.position.X / 16f - (float)Player.tileRangeX - (float)player.inventory[player.selectedItem].tileBoost - (float)player.blockRange <= (float)Player.tileTargetX)
                || !((player.position.X + (float)player.width) / 16f + (float)Player.tileRangeX + (float)player.inventory[player.selectedItem].tileBoost - 1f + (float)player.blockRange >= (float)Player.tileTargetX) || !(player.position.Y / 16f - (float)Player.tileRangeY - (float)player.inventory[player.selectedItem].tileBoost - (float)player.blockRange <= (float)Player.tileTargetY)
                || !((player.position.Y + (float)player.height) / 16f + (float)Player.tileRangeY + (float)player.inventory[player.selectedItem].tileBoost - 2f + (float)player.blockRange >= (float)Player.tileTargetY)
                || Main.tile[Player.tileTargetX, Player.tileTargetY].TileColor > 0 || !Main.tile[Player.tileTargetX, Player.tileTargetY].HasTile)
            {
                orig(player);
                return;
            }

            player.cursorItemIconEnabled = true;
            if (player.ItemTimeIsZero && player.itemAnimation > 0 && player.controlUseItem)
            {
                foreach (var remove in AequusItem.RemoveCustomCoating)
                {
                    if (remove(Player.tileTargetX, Player.tileTargetY, player))
                    {
                        player.ApplyItemTime(player.inventory[player.selectedItem], player.tileSpeed);
                        return;
                    }
                }
            }
        }

        private static void Player_TryPainting(On.Terraria.Player.orig_TryPainting orig, Player player, int x, int y, bool paintingAWall, bool applyItemAnimation)
        {
            orig(player, x, y, paintingAWall, applyItemAnimation);
            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                if (CheckCustomCoatings(x, y, player, player.inventory[i], applyItemAnimation))
                {
                    return;
                }
            }
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                if (CheckCustomCoatings(x, y, player, player.inventory[i], applyItemAnimation))
                {
                    return;
                }
            }
        }
        private static bool CheckCustomCoatings(int x, int y, Player player, Item item, bool applyItemAnimation)
        {
            if (!item.IsAir)
            {
                if (item.paint > 0)
                {
                    return true;
                }
                if (AequusItem.ApplyCustomCoating.TryGetValue(item.type, out var action) && action(x, y, player))
                {
                    player.ConsumeItem(item.type);
                    player.ApplyItemTime(player.inventory[player.selectedItem], player.tileSpeed);
                    return true;
                }
            }
            return false;
        }

        private static void Player_PickTile(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
        {
            pickPower = (int)(pickPower * self.Aequus().pickTileDamage);
            orig(self, x, y, pickPower);
        }

        private static void FancyGolfPredictionLine_Update(On.Terraria.GameContent.Golf.FancyGolfPredictionLine.orig_Update orig, Terraria.GameContent.Golf.FancyGolfPredictionLine self, Entity golfBall, Vector2 impactVelocity, float roughLandResistance)
        {
            bool solid = Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()];
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
            orig(self, golfBall, impactVelocity, roughLandResistance);
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = solid;
        }

        private static bool Player_CheckSpawn(On.Terraria.Player.orig_CheckSpawn orig, int x, int y)
        {
            bool solid = Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()];
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
            bool originalValue = orig(x, y);
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = solid;
            return originalValue;
        }

        private static void Player_JumpMovement(On.Terraria.Player.orig_JumpMovement orig, Player self)
        {
            if (self.Aequus().gravityTile != 0)
            {
                self.gravDir = Math.Sign(self.Aequus().gravityTile);
            }
            orig(self);
        }

        private static void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, NetworkText deathText, int hitDirection)
        {
            if (self.Aequus().ghostTombstones)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPCDirect(self.GetSource_Death("Ghostly Grave"), self.Center, NPCID.Ghost);
                }
                return;
            }

            orig(self, coinsOwned, deathText, hitDirection);
        }

        private static void NPC_NPCLoot_DropMoney(On.Terraria.NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.Aequus().accGrandReward)
            {
                return;
            }
            orig(self, closestPlayer);
        }

        private static ItemDropAttemptResult ItemDropResolver_ResolveRule(On.Terraria.GameContent.ItemDropRules.ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            var result = orig(self, rule, info);
            if (info.player != null && result.State == ItemDropAttemptResultState.FailedRandomRoll)
            {
                if (AequusHelpers.iterations == 0)
                {
                    for (float luckLeft = info.player.Aequus().dropRerolls; luckLeft > 0f; luckLeft--)
                    {
                        if (luckLeft < 1f)
                        {
                            if (Main.rand.NextFloat(1f) > luckLeft)
                            {
                                return result;
                            }
                        }
                        AequusHelpers.iterations++;
                        var result2 = orig(self, rule, info);
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
            calcForBuying = Math.Max(calcForBuying - self.Aequus().accForgedCard, min);
        }

        private static bool customDraws;
        private static void PlayerDrawLayers_DrawPlayer_RenderAllLayers(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
        {
            try
            {
                if (customDraws)
                {
                    orig(ref drawinfo);
                    return;
                }
                customDraws = true;
                drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PreDraw(ref drawinfo);
                orig(ref drawinfo);
                drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PostDraw(ref drawinfo);
            }
            catch
            {

            }
            customDraws = false;
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
                case FlaskID.Venom:
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

                case FlaskID.CursedInferno:
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

                case FlaskID.Fire:
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

                case FlaskID.Midas:
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

                case FlaskID.Ichor:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.IchorTorch, velocity, Alpha: 100, Scale: 2.5f);
                            d.velocity.X += player.direction;
                            d.velocity.Y -= 0.2f;
                        }
                    }
                    break;

                case FlaskID.Nanites:
                    {
                        if (Main.rand.NextBool(2))
                        {
                            var d = Dust.NewDustPerfect(position, DustID.IceTorch, velocity, Alpha: 100, Scale: 2.5f);
                            d.velocity.X += player.direction;
                            d.velocity.Y -= 0.2f;
                        }
                    }
                    break;

                case FlaskID.Party:
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

                case FlaskID.Poison:
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

        public static Player CurrentPlayerContext()
        {
            if (PlayerContext > -1)
            {
                return Main.player[PlayerContext];
            }
            if (AequusProjectile.pWhoAmI != -1 && Main.projectile[AequusProjectile.pWhoAmI].friendly)
            {
                return Main.player[Main.projectile[AequusProjectile.pWhoAmI].owner];
            }
            return null;
        }
    }
}