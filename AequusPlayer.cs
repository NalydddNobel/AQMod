using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Misc;
using Aequus.Common;
using Aequus.Common.Buffs;
using Aequus.Common.DamageClasses;
using Aequus.Common.DataSets;
using Aequus.Common.Effects;
using Aequus.Common.Items;
using Aequus.Common.ModPlayers;
using Aequus.Common.Net.Sounds;
using Aequus.Common.NPCs;
using Aequus.Common.Particles;
using Aequus.Common.PlayerLayers;
using Aequus.Common.PlayerLayers.Equipment;
using Aequus.Common.Preferences;
using Aequus.Common.Projectiles.SentryChip;
using Aequus.Common.Tiles;
using Aequus.Common.UI;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Biomes.GoreNest;
using Aequus.Content.Events.DemonSiege;
using Aequus.Content.Events.GaleStreams;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Events.GlimmerEvent.Peaceful;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Equipment.Accessories.Combat.Dodge.FlashwayNecklace;
using Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BlackPhial;
using Aequus.Items.Equipment.Accessories.Combat.Ranged;
using Aequus.Items.Equipment.Accessories.Combat.Sentry.SentrySquid;
using Aequus.Items.Equipment.Accessories.Misc;
using Aequus.Items.Equipment.Accessories.Misc.Luck;
using Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;
using Aequus.Items.Equipment.Accessories.SentryChip;
using Aequus.Items.Equipment.AccessoriesVanity;
using Aequus.Items.Materials.SoulGem;
using Aequus.Items.Potions.DeathsEmbrace;
using Aequus.Items.Potions.FrostPotion;
using Aequus.Items.Potions.NoonPotion;
using Aequus.Items.Tools;
using Aequus.NPCs;
using Aequus.NPCs.Town.ExporterNPC;
using Aequus.Particles;
using Aequus.Projectiles;
using Aequus.Projectiles.Misc.Bobbers;
using Aequus.Projectiles.Misc.GrapplingHooks;
using Aequus.Tiles.Blocks;
using Aequus.Tiles.Misc.AshTombstones;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {
        public const int AmountHeartsMax = 20;

        /// <summary>
        /// Defaults to 50.
        /// </summary>
        public static int DefaultBuildingBuffRange = 50;
        public static bool EquipmentModifierUpdate;
        public static int PlayerContext;
        public static List<Player> _playerQuickList;
        public static readonly List<NPC> DashImmunityHack = new();
        public static List<(int, Func<Player, bool>, Action<Dust>)> SpawnEnchantmentDusts_Custom { get; set; }
        private static MethodInfo Player_ItemCheck_Shoot;
        public static int TeamContext;

        public float? CustomDrawShadow;
        public float? DrawScale;
        public int? DrawForceDye;

        public int projectileIdentity = -1;

        public int extraHealingPotion;
        [Obsolete("tModLoader removed negative defense.")]
        public int negativeDefense;

        public float darkDamage;
        public float lightDamage;

        public PlayerWingModifiers flightStats;

        public Item accWormScarf;
        public Item accBoneHelm;

        public float addLuck;

        /// <summary>
        /// <see cref="Player.statLife"/> on the previous update.
        /// </summary>
        public int prevLife;
        public int prevMana;

        public int manathirst;
        public int bloodthirst;

        public float maxSpawnsDivider;
        public float spawnrateMultiplier;

        public float shopPriceMultiplier;

        public int cursorDye;
        public int cursorDyeOverride;

        public int CursorDye { get => cursorDyeOverride > 0 ? cursorDyeOverride : cursorDye; set => cursorDye = value; }

        public int buildingBuffRange;

        [SaveData("PermaLootLuck")]
        [SaveDataAttribute.IsListedBoolean]
        public bool usedPermaLootLuck;
        /// <summary>
        /// Enabled by <see cref="VictorsReward"/>
        /// </summary>
        [SaveData("MaxLifeRespawn")]
        [SaveDataAttribute.IsListedBoolean]
        public bool maxLifeRespawnReward;
        /// <summary>
        /// Enabled by <see cref="GhostlyGrave"/>
        /// </summary>
        [SaveData("GravesDisabled")]
        [SaveDataAttribute.IsListedBoolean]
        public bool ghostTombstones;

        //public ShatteringVenus.ItemInfo shatteringVenus;

        public int soulCrystalDamage;

        public int debuffLifeStealDamage;
        public int debuffLifeSteal;

        public bool accResetEnemyDebuffs;

        public float statMeleeScale;

        public DamageClassStatFloat statProjectileSpeed = new();

        public float pickTileDamage;

        public sbyte gravityTile;

        public float darkness;

        public bool accLavaPlace;

        public int accHyperJet;
        public bool accShowQuestFish;
        public bool accPriceMonocle;

        public bool eyeGlint;

        public int equippedMask;
        public int cMask;
        public int equippedHat;
        public int cHat;
        public int equippedEyes;
        public int cEyes;
        public int equippedEars;
        public int cEars;
        public int stackingHat;

        public int grappleNPCOld;
        public int grappleNPC;
        public int leechHookNPC;

        public bool omnibait; // To Do: Make this flag force ALL mod biomes to randomly be toggled on/off or something.

        public int GrappleNPC => grappleNPC == -1 ? grappleNPCOld : grappleNPC;

        public bool ZoneCrabCrevice => Player.InModBiome<CrabCreviceBiome>();
        public bool ZoneGaleStreams => Player.InModBiome<GaleStreamsZone>();
        public bool ZoneGlimmer => Player.InModBiome<GlimmerBiomeManager>();
        public bool ZonePeacefulGlimmer => Player.InModBiome<PeacefulGlimmerBiome>();
        public bool ZoneDemonSiege => Player.InModBiome<DemonSiegeBiome>();
        public bool ZoneGoreNest => Player.InModBiome<GoreNestBiome>();

        /// <summary>
        /// A point determining one of the close gore nests. Prioritized by their order in <see cref="DemonSiegeSystem.ActiveSacrifices"/>
        /// </summary>
        public Point eventDemonSiege;

        public bool hurtAttempted;
        public bool hurtSucceeded;
        public bool grounded;

        /// <summary>
        /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/> -> <see cref="PostUpdate_CheckClosestEnemy"/>
        /// </summary>
        public int closestEnemy;
        public int closestEnemyOld;

        private DebuffInflictionStats debuffs;
        public ref DebuffInflictionStats DebuffsInfliction => ref debuffs;
        public float buffDuration;
        public float debuffDuration;

        public bool accSentrySlot;

        public int instaShieldTime;
        public int instaShieldFrames;
        public int instaShieldCooldown;
        public float instaShieldAlpha;

        /// <summary>
        /// 0 = no force, 1 = force day, 2 = force night
        /// <para>Used by <see cref="NoonBuff"/> and set to 1</para>
        /// </summary>
        public byte forceDayState;

        /// <summary>
        /// A percentage chance for a successful scam, where you don't consume money. Values below or equal 0 mean no scams, Values above or equal 1 mean 100% scam rate. Unused.
        /// </summary>
        public float accFaultyCoin;
        /// <summary>
        /// Unused flat discount on items.
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

        public int accLittleInferno;

        public int accGroundCrownCrit;
        public float accDarknessCrownDamage;

        public float antiGravityItemRadius;

        public int accFrostburnTurretSquid;
        public bool accGrandReward;
        public int accBoneBurningRing;

        public int accBlackPhial;
        public int cdBlackPhial;

        public bool accDevilsTongue;

        public Item accRamishroom;

        public Item accHyperCrystal;
        public int cHyperCrystal;
        public int hyperCrystalCooldown;
        public int hyperCrystalCooldownMelee;
        public int hyperCrystalCooldownMax;

        public Item setbonusRef;

        public Item setGravetender;
        public int gravetenderGhost;

        public float zombieDebuffMultiplier;

        public Item accSentrySquid;
        public int sentrySquidTimer;

        [Obsolete("Mendshroom was removed")]
        public Item accMendshroom;
        [Obsolete("Mendshroom was removed")]
        public int cMendshroom;

        public Item accCelesteTorus;
        public float celesteTorusDamage;
        public int cCelesteTorus;

        /// <summary>
        /// Set by <see cref="Sentry6502"/> and <see cref="Sentinel6510"/>
        /// </summary>
        public Item accSentryInheritence;
        public Item accAmmoRenewalPack;
        public Item accMothmanMask;

        public int accGlowCore;
        public int cGlowCore;

        public bool accRitualSkull;

        /// <summary>
        /// Set to true by <see cref="Aequus.Items.Equipment.Armor.SetTrap.DartTrapHat"/>, <see cref="Aequus.Items.Equipment.Armor.SetTrap.SuperDartTrapHat"/>, <see cref="Aequus.Items.Armor.FlowerCrown.FlowerCrown"/>, <see cref="Aequus.Items.Equipment.Armor.SetTrap.VenomDartTrapHat"/>, <see cref="Aequus.Items.Equipment.Armor.SetWizard.MoonlunaHat"/>
        /// </summary>
        public bool wearingPassiveSummonHelmet;
        /// <summary>
        /// Used by summon helmets (<see cref="Aequus.Items.Equipment.Armor.SetTrap.DartTrapHat"/>, <see cref="Aequus.Items.Equipment.Armor.SetTrap.SuperDartTrapHat"/>, <see cref="Aequus.Items.Armor.FlowerCrown.FlowerCrown"/>) to time projectile spawns and such.
        /// </summary>
        public int summonHelmetTimer;

        public bool HasSkeletonKey => Player.HasItemInInvOrVoidBag(ModContent.ItemType<SkeletonKey>());

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

        [Obsolete]
        public int soulLimit;

        /// <summary>
        /// Defense which works 100% no matter the difficulty mode
        /// </summary>
        public int flatDamageReduction;

        public int turretSlotCount;

        [Obsolete]
        public int ghostShadowDash;
        [Obsolete]
        public int ghostChains;

        [Obsolete]
        public int ghostSlotsMax;
        [Obsolete]
        public int ghostSlotsOld;
        [Obsolete]
        public int ghostSlots;
        [Obsolete]
        public int ghostProjExtraUpdates;
        [Obsolete]
        public int ghostLifespan;

        public int timeSinceLastHit;
        public int idleTime;

        public int sceneInvulnerability;

        private List<int> boundedPotionIDs;
        public List<int> BoundedPotionIDs {
            get {
                boundedPotionIDs ??= new List<int>();
                return boundedPotionIDs;
            }
            set {
                boundedPotionIDs = value;
            }
        }

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

        public override void Load() {
            _playerQuickList = new List<Player>();
            LoadHooks();
            Load_DeathMsgHook();
            Load_TrashMoney();
            Load_Fishing();
            SpawnEnchantmentDusts_Custom = new List<(int, Func<Player, bool>, Action<Dust>)>();
            Player_ItemCheck_Shoot = typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override void Unload() {
            Unload_FishingEffects();
            Unload_TrashMoney();
            DashImmunityHack.Clear();
            SpawnEnchantmentDusts_Custom = null;
            Player_ItemCheck_Shoot = null;
        }

        public override void CopyClientState(ModPlayer clientClone)/* tModPorter Suggestion: Replace Item.Clone usages with Item.CopyNetStateTo */
        {
            var clone = (AequusPlayer)clientClone;
            clone.itemCombo = itemCombo;
            clone.itemSwitch = itemSwitch;
            clone.itemUsage = itemUsage;
            clone.itemCooldown = itemCooldown;
            clone.itemCooldownMax = itemCooldownMax;
            clone.timeSinceLastHit = timeSinceLastHit;
            clone.regenerationFlat = regenerationFlat;
            clone.BoundedPotionIDs = new List<int>(BoundedPotionIDs);
            clone.darkness = darkness;
            clone.gravetenderGhost = gravetenderGhost;
            clone.sceneInvulnerability = sceneInvulnerability;
            clientClone_BoundBow(clone);
            clone.bloodthirst = bloodthirst;
            clone.manathirst = manathirst;
        }

        public override void SendClientChanges(ModPlayer clientPlayer) {
            var client = (AequusPlayer)clientPlayer;

            var bb = new BitsByte(
                client.itemSwitch <= 0 && itemSwitch > 0,
                Math.Abs(timeSinceLastHit - client.timeSinceLastHit) > 60,
                gravetenderGhost != client.gravetenderGhost,
                false,
                (client.itemCombo - itemCombo).Abs() > 20,
                client.instaShieldTime != instaShieldTime,
                !BoundedPotionIDs.IsTheSameAs(client.BoundedPotionIDs),
                ShouldSyncBoundBow(client));

            var bb2 = new BitsByte(
                client.summonHelmetTimer == 0 && summonHelmetTimer != 0,
                client.sceneInvulnerability <= 0 && sceneInvulnerability > 0,
                client.itemCooldown <= 0 && itemCooldown > 0,
                (client.itemUsage - itemUsage).Abs() > 20,
                client.bloodthirst != bloodthirst,
                client.manathirst != manathirst);

            if (bb > 0 || bb2 > 0) {
                var p = Aequus.GetPacket(PacketType.SyncAequusPlayer);
                p.Write((byte)Player.whoAmI);
                p.Write(bb);
                p.Write(bb2);
                p.Write(darkness);
                if (bb[0]) {
                    p.Write(itemSwitch);
                }
                if (bb[1]) {
                    p.Write(timeSinceLastHit);
                }
                if (bb[2]) {
                    p.Write(gravetenderGhost);
                }
                if (bb[4]) {
                    p.Write(itemCombo);
                }
                if (bb[5]) {
                    p.Write(instaShieldTime);
                }
                if (bb[6]) {
                    p.Write(BoundedPotionIDs.Count);
                    for (int i = 0; i < BoundedPotionIDs.Count; i++) {
                        p.Write(BoundedPotionIDs[i]);
                    }
                }
                if (bb[7]) {
                    p.Write(boundBowAmmo);
                    p.Write(boundBowAmmoTimer);
                }
                if (bb2[0]) {
                    p.Write(summonHelmetTimer);
                }
                if (bb2[1]) {
                    p.Write(sceneInvulnerability);
                }
                if (bb2[2]) {
                    p.Write(itemCooldown);
                    p.Write(itemCooldownMax);
                }
                if (bb2[3]) {
                    p.Write(itemUsage);
                }
                if (bb2[4]) {
                    p.Write((ushort)bloodthirst);
                }
                if (bb2[5]) {
                    p.Write((ushort)manathirst);
                }
                p.Send();
            }
        }

        public void RecieveChanges(BinaryReader reader) {
            BitsByte bb = reader.ReadByte();
            BitsByte bb2 = reader.ReadByte();
            darkness = reader.ReadSingle();
            if (bb[1]) {
                itemSwitch = reader.ReadUInt16();
            }
            if (bb[1]) {
                timeSinceLastHit = reader.ReadInt32();
            }
            if (bb[2]) {
                gravetenderGhost = reader.ReadInt32();
            }
            if (bb[4]) {
                itemCombo = reader.ReadUInt16();
            }
            if (bb[5]) {
                instaShieldTime = reader.ReadInt32();
            }
            if (bb[6]) {
                BoundedPotionIDs.Clear();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++) {
                    BoundedPotionIDs.Add(reader.ReadInt32());
                }
            }
            if (bb[7]) {
                boundBowAmmo = reader.ReadInt32();
                boundBowAmmoTimer = reader.ReadInt32();
            }
            if (bb2[0]) {
                summonHelmetTimer = reader.ReadInt32();
            }
            if (bb2[1]) {
                sceneInvulnerability = reader.ReadInt32();
            }
            if (bb2[2]) {
                itemCooldown = reader.ReadUInt16();
                itemCooldownMax = reader.ReadUInt16();
            }
            if (bb2[3]) {
                itemUsage = reader.ReadUInt16();
            }
            if (bb2[4]) {
                bloodthirst = reader.ReadUInt16();
            }
            if (bb2[5]) {
                manathirst = reader.ReadUInt16();
            }
        }

        public override void SetControls() {
            if (Main.myPlayer == Player.whoAmI) {
                if (Aequus.UserInterface?.CurrentState is AequusUIState aequusUI) {
                    aequusUI.ConsumePlayerControls(Player);
                }
            }
        }

        public override bool HoverSlot(Item[] inventory, int context, int slot) {
            bool returnValue = false;
            if (inventory[slot].ModItem is ItemHooks.IHoverSlot hoverSlot) {
                returnValue |= hoverSlot.HoverSlot(inventory, context, slot);
            }
            if (Aequus.UserInterface?.CurrentState is AequusUIState aequusUI) {
                returnValue |= aequusUI.HoverSlot(inventory, context, slot);
            }
            return returnValue;
        }

        public override void Initialize() {
            Initialize_BoundBow();
            Initialize_Vampire();
            crownOfBloodWormScarfTarget = -1;
            veinmineTask = new();
            maxSpawnsDivider = 1f;
            spawnrateMultiplier = 1f;
            BoundedPotionIDs = new List<int>();
            debuffs = new DebuffInflictionStats(0);
            //shatteringVenus = new ShatteringVenus.ItemInfo();
            accGlowCore = 0;
            cGlowCore = -1;
            instaShieldAlpha = 0f;
            gravityTile = 0;
            CursorDye = -1;
            ghostTombstones = false;

            sentrySquidTimer = 120;
            itemCooldown = 0;
            itemCooldownMax = 0;
            itemCombo = 0;
            itemSwitch = 0;
            netInteractionCooldown = 60;
            closestEnemyOld = -1;
            closestEnemy = -1;
        }

        public override void UpdateDead() {
            ClearCrownOfBlood();
            UpdateDead_Vampire();
            if (accHyperJet > 0) {
                HyperJet.RespawnTime(Player, this);
            }
            timeSinceLastHit = 0;
            foreach (var buff in BoundedPotionIDs) {
                Main.persistentBuff[buff] = true;
            }
        }

        public override void OnRespawn() {
            if (maxLifeRespawnReward) {
                Player.statLife = Math.Max(Player.statLife, Player.statLifeMax2);
            }
        }

        public void ResetArmor() {
            extraOresChance.Clear();
            veinminerAbility = 0;
            soulCrystalDamage = 0;
            debuffLifeSteal = 0;
            accResetEnemyDebuffs = false;
            accLavaPlace = false;
            instaShieldFrames = 0;
            instaShieldCooldown = 0;
            eyeGlint = false;
            stackingHat = 0;

            setbonusRef = null;
            setGravetender = null;

            ghostChains = 0;
            ghostShadowDash = 0;
            accGlowCore = 0;
            accHyperJet = 0;
            accSentrySlot = false;
            accGroundCrownCrit = 0;
            accDarknessCrownDamage = 0f;
            accShowQuestFish = false;
            accPriceMonocle = false;
            bulletSpread = 1f;
            accDavyJonesAnchor.Clear();
            accLittleInferno = 0;
            accRitualSkull = false;
            accRamishroom = null;
            zombieDebuffMultiplier = 0f;
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
            accBoneBurningRing = 0;
            accBoneRing.Clear();
            accDevilsTongue = false;
            accGrandReward = false;
            accFoolsGoldRing = 0;

            accSentrySquid = null;
            if (!InDanger) {
                sentrySquidTimer = Math.Min(sentrySquidTimer, 240);
            }
            if (sentrySquidTimer > 0) {
                sentrySquidTimer--;
            }
        }

        public void ResetStats() {
            extraHealingPotion = 0;
            negativeDefense = 0;
            flightStats.Clear();
            maxSpawnsDivider = 1f;
            spawnrateMultiplier = 1f;
            buildingBuffRange = DefaultBuildingBuffRange;
            shopPriceMultiplier = 0f;
            if (BoundedPotionIDs == null) {
                BoundedPotionIDs = new List<int>();
            }
            else if (BoundedPotionIDs.Count > 0) {
                foreach (var buff in BoundedPotionIDs) {
                    Main.persistentBuff[buff] = false;
                }
            }
            statMeleeScale = 0f;
            debuffs.Clear(Player);
            buffDuration = 1f;
            debuffDuration = 1f;
            dropRerolls = usedPermaLootLuck ? 0.05f : 0f;
            luckRerolls = 0f;
            antiGravityItemRadius = 0f;
            soulLimit = 0;
            pickTileDamage = 1f;
            ghostSlotsMax = 1;
            ghostProjExtraUpdates = 0;
            ghostLifespan = 3600;
        }

        public void UpdateInstantShield() {
            if ((hurtAttempted || instaShieldTime < instaShieldFrames) && instaShieldTime > 0) {
                if (instaShieldTime == instaShieldFrames) {
                    SoundEngine.PlaySound(SoundID.Item75.WithPitch(1f).WithVolume(0.75f), Player.Center);
                }
                instaShieldTime--;
                if (instaShieldTime == 0) {
                    instaShieldTime = -1;
                }
                if (instaShieldAlpha < 1f) {
                    instaShieldAlpha += 0.035f;
                    if (instaShieldAlpha > 1f) {
                        instaShieldAlpha = 1f;
                    }
                }
            }
            else {
                if (instaShieldTime == 0) {
                    instaShieldTime = instaShieldFrames;
                }
                if (instaShieldTime < instaShieldFrames) {
                    instaShieldTime = -1;
                    int instaShieldCooldownBuffIndex = Player.FindBuffIndex(ModContent.BuffType<FlashwayNecklaceCooldown>());
                    if (instaShieldCooldownBuffIndex == -1) {
                        Player.AddBuff(ModContent.BuffType<FlashwayNecklaceCooldown>(), instaShieldCooldown);
                    }
                    else if (Player.buffTime[instaShieldCooldownBuffIndex] <= 2) {
                        instaShieldTime = instaShieldFrames;
                    }
                }
                if (instaShieldAlpha > 0f) {
                    instaShieldAlpha -= 0.035f;
                    if (instaShieldAlpha < 0f) {
                        instaShieldAlpha = 0f;
                    }
                }
            }
            instaShieldFrames = 0;
        }

        public void CheckGravityBlocks() {
            bool doEffects = gravityTile == 0;
            gravityTile = GravityBlockHandler.CheckGravityBlocks(Player.position, Player.width, Player.height);
            if (gravityTile != 0) {
                Player.gravDir = gravityTile < 0 ? -1f : 1f;
                if (doEffects) {
                    Player.controlJump = false;
                    Player.velocity.Y = MathHelper.Clamp(Player.velocity.Y, -2f, 2f);
                    SoundEngine.PlaySound(SoundID.Item8, Player.position);
                }
            }
        }

        public void ResetDyables() {
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

        public void UpdateItemFields() {
            if (itemCombo > 0) {
                itemCombo--;
            }
            if (itemSwitch > 0) {
                itemUsage = 0;
                itemSwitch--;
            }
            else if (Player.itemTime > 0) {
                itemUsage++;
            }
            else {
                itemUsage = 0;
            }
            if (itemCooldown > 0) {
                if (itemCooldownMax == 0) {
                    itemCooldown = 0;
                    itemCooldownMax = 0;
                }
                else {
                    itemCooldown--;
                    if (itemCooldown == 0) {
                        itemCooldownMax = 0;
                    }
                }
                Player.manaRegen = 0;
                Player.manaRegenDelay = (int)Player.maxRegenDelay;
            }
        }

        public override void ResetEffects() {
            try {
                PlayerContext = Player.whoAmI;

                UpdateInstantShield();
                ResetDyables();
                ResetArmor();
                ResetStats();
                ResetEffects_WarHorn();
                ResetEffects_CrownOfBlood();
                ResetEffects_EquipDraws();
                ResetEffects_Stormcloak();
                ResetEffects_FaultyCoin();
                ResetEffects_FoolsGoldRing();
                ResetEffects_TrashMoney();
                ResetEffects_HighSteaks();
                ResetEffects_Vampire();
                ResetEffects_Zen();
                hasBlockGlove = false;
                darkDamage = 0f;
                lightDamage = 0f;
                buffPyramidLuck = false;
                statProjectileSpeed.Clear();
                armorAetherialAmmoCost = false;
                accNeonGenesis = null;
                breathConserver = 0;
                accWormScarf = null;
                accBoneHelm = null;
                flatDamageReduction = 0;
                addLuck = 0f;
                accCrownOfBlood = null;
                armorNecromancerBattle = null;
                cursorDye = -1;
                cursorDyeOverride = 0;

                if (sceneInvulnerability > 0)
                    sceneInvulnerability--;

                if (gravityTile != 0) {
                    Player.gravControl = false;
                    Player.gravControl2 = false;
                }

                if (grappleNPC > -1) {
                    if (!Main.npc[grappleNPC].active) {
                        grappleNPCOld = -1;
                    }
                    grappleNPC = -1;
                }

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<LeechHookProj>()] <= 0)
                    leechHookNPC = -1;

                if (Player.velocity.Length() < 1f) {
                    idleTime++;
                }
                else {
                    idleTime = 0;
                }

                UpdateItemFields();
                if (netInteractionCooldown > 0) {
                    netInteractionCooldown--;
                }

                forceDayState = 0;
                TeamContext = Player.team;
                hurtAttempted = false;
                hurtSucceeded = false;

                if (Main.netMode != NetmodeID.Server) {
                    legsOverlayTexture = null;
                    legsOverlayGlowTexture = null;
                }
            }
            catch {
            }
        }

        public override void PreUpdate() {
            EquipmentModifierUpdate = false;
            projectileIdentity = -1;
            if (forceDayState == 1) {
                AequusSystem.Main_dayTime.SetValue(true);
            }
            else if (forceDayState == 2) {
                AequusSystem.Main_dayTime.SetValue(false);
            }
            forceDayState = 0;

            eventDemonSiege = DemonSiegeSystem.FindDemonSiege(Player.Center);
        }

        public override void PreUpdateBuffs() {
            timeSinceLastHit++;
            PreUpdateBuffs_Vampire();
        }

        public override void PostUpdateBuffs() {
            int stariteBottleBuff = Player.FindBuffIndex(ModContent.BuffType<StariteBottleBuff>());
            if (stariteBottleBuff != -1) {
                StariteBottleBuff.UpdateEffects(Player, stariteBottleBuff);
            }
            for (int i = 0; i < BoundedPotionIDs.Count; i++) {
                if (!Player.HasBuff(BoundedPotionIDs[i])) {
                    BoundedPotionIDs.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void UpdateEquips() {
            EquipmentModifierUpdate = true;
            UpdateEquips_Vampire();
        }

        public override void PostUpdateEquips() {
            PostUpdateEquips_EmpoweredEquipAbilities();
            PostUpdateEquips_CrownOfBlood();
            PostUpdateEquips_Vampire();

            if (Player.HasBuff<TonicSpawnratesDebuff>()) {
                Player.ClearBuff(ModContent.BuffType<TonicSpawnratesBuff>());
                Player.buffImmune[ModContent.BuffType<TonicSpawnratesBuff>()] = true;
            }

            CheckGravityBlocks();

            if (accRitualSkull) {
                ghostSlotsMax += Player.maxMinions - 1;
                Player.maxMinions = 1;
            }

            UpdateStormcloak();

            debuffLifeSteal *= 120; // Due to how NPC.lifeRegen is programmed
            if (debuffLifeSteal > 0) {
                int damageOverTimeInflictedThisFrame = 0;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (Main.npc[i].active && !Main.npc[i].immortal && !Main.npc[i].dontTakeDamage && Player.Distance(Main.npc[i].Center) < 1000f) {
                        if (Main.npc[i].lifeRegen < 0)
                            damageOverTimeInflictedThisFrame -= Main.npc[i].lifeRegen;
                    }
                }
                debuffLifeStealDamage += Math.Min(damageOverTimeInflictedThisFrame, debuffLifeSteal / 10);
                //Main.NewText(debuffLifeStealDamage);
                if (Main.myPlayer == Player.whoAmI) {
                    if (debuffLifeStealDamage >= debuffLifeSteal) {
                        int amt = (int)Math.Min(debuffLifeStealDamage / debuffLifeSteal, Player.lifeSteal);
                        if (amt > 0) {
                            Player.Heal(amt);
                            Player.lifeSteal += amt;
                        }
                    }
                }
                debuffLifeStealDamage %= debuffLifeSteal;
            }

            if (accDarknessCrownDamage > 0f && InDarkness) {
                Player.GetDamage(DamageClass.Generic) += accDarknessCrownDamage;
            }
            grounded = false;
            if (accGroundCrownCrit > 0 && Player.velocity.Y == 0f && Player.oldVelocity.Y == 0f) {
                grounded = true;
                Player.GetCritChance(DamageClass.Generic) += accGroundCrownCrit;
            }
            if (gravityTile != 0) {
                Player.gravity = 0.4f;
            }
            PostUpdateEquips_NeonGenesis();
            PostUpdateEquips_BreathConserver();
        }

        public override void PostUpdateMiscEffects() {
            if (GameplayConfig.Instance.DamageReductionCap < 1f) {
                Player.endurance = Math.Min(Player.endurance, GameplayConfig.Instance.DamageReductionCap);
            }
            int talkNPC = Player.talkNPC;
            if (Main.npc.IndexInRange(talkNPC) && Main.npc[talkNPC].active && Main.npc[talkNPC].ModNPC is NPCHooks.ITalkNPCUpdate talkNPCUpdate) {
                talkNPCUpdate.TalkNPCUpdate(Player);
            }
            Player.statDefense -= negativeDefense;
        }

        public override bool PreItemCheck() {
            if (AequusSystem.Main_dayTime.IsCaching)
                AequusSystem.Main_dayTime.PopCachedStatic();
            return true;
        }

        public override void PostItemCheck() {
            if (AequusSystem.Main_dayTime.IsCaching)
                AequusSystem.Main_dayTime.PushCachedStatic();
            if (Player.selectedItem != lastSelectedItem) {
                lastSelectedItem = Player.selectedItem;
                itemSwitch = Math.Max((ushort)30, itemSwitch);
                itemUsage = 0;
                itemHits = 0;
            }
            CountSentries();
        }

        public void CheckThirsts() {
            if (Player.HasBuff<BloodthirstBuff>()) {
                if (prevLife < Player.statLife) {
                    bloodthirst += Player.statLife - prevLife;
                    Player.statLife = prevLife;
                }
            }
            if (Player.HasBuff<ManathirstBuff>()) {
                if (prevMana < Player.statMana) {
                    manathirst += Player.statMana - prevMana;
                    Player.statMana = prevMana;
                }
            }
        }

        public override void PostUpdate() {
            UpdateCrownOfBlood();
            PostUpdate_FaultyCoin();
            PostUpdate_FoolsGoldRing();
            CheckThirsts();
            if (Player.HasBuff<BloodthirstBuff>()) {
                if (bloodthirst > 0 && Main.GameUpdateCount % 2 == 0) {
                    //CombatText.NewText(Player.getRect(), CombatText.HealLife, 1, dot: true);
                    Player.statLife = Math.Min(Player.statLife + 1, Player.statLifeMax2);
                    bloodthirst--;
                }
            }
            else if (bloodthirst > 0) {
                bloodthirst = 0;
            }
            if (Player.HasBuff<ManathirstBuff>()) {
                if (manathirst > 0) {
                    //CombatText.NewText(Player.getRect(), CombatText.HealMana, 1, dot: true);
                    Player.statMana = Math.Min(Player.statMana + 1, Player.statManaMax2);
                    manathirst--;
                }
            }
            else if (manathirst > 0) {
                manathirst = 0;
            }
            prevLife = Player.statLife;
            prevMana = Player.statMana;

            if (Main.netMode != NetmodeID.Server && !Player.outOfRange)
                DoDebuffEffects();

            if (antiGravityItemRadius > 0f) {
                float radius = MathF.Pow(antiGravityItemRadius, 2f);
                var position = Player.Center;
                for (int i = 0; i < Main.maxItems; i++) {
                    if (Main.item[i].active && !ItemID.Sets.ItemNoGravity[Main.item[i].type]
                        && Vector2.DistanceSquared(Main.item[i].Center, position) < radius) {
                        Main.item[i].Aequus().SetNoGrav(Main.item[i], 30);
                    }
                }
            }

            if (accGlowCore > 0) {
                GlowCore.AddLight(Player.Center, Player, this);
            }

            if (accMendshroom != null && accMendshroom.shoot > ProjectileID.None && ProjectilesOwned(accMendshroom.shoot) <= 10) {
                if (Main.rand.NextBool((int)Math.Clamp(360 * LifeRatio, 120f, 600f))) {
                    for (int i = 0; i < 100; i++) {
                        var randomSpot = Player.Center + new Vector2(Main.rand.NextFloat(-280f, 280f), Main.rand.NextFloat(-280f, 280f));
                        if (Player.Distance(randomSpot) < 100f)
                            continue;
                        if (!Collision.SolidCollision(randomSpot, 2, 2) && Collision.CanHitLine(randomSpot, 2, 2, Player.position, Player.width, Player.height)) {
                            Projectile.NewProjectile(Player.GetSource_Accessory(accMendshroom), randomSpot, Vector2.Zero, accMendshroom.shoot,
                                0, 0f, Player.whoAmI, ai1: projectileIdentity + 1);
                            break;
                        }
                    }
                }
            }

            if (Main.myPlayer == Player.whoAmI) {
                PostUpdate_Veinminer();
            }

            ghostSlotsOld = ghostSlots;
            ghostSlots = 0;
            PostUpdate_CheckClosestEnemy();
            TeamContext = 0;

            if (setGravetender != null) {
                if (setGravetender.shoot > ProjectileID.None && Player.ownedProjectileCounts[setGravetender.shoot] <= 0) {
                    Projectile.NewProjectile(Player.GetSource_Accessory(setGravetender), Player.Center, -Vector2.UnitY, setGravetender.shoot,
                        Player.GetWeaponDamage(setGravetender), Player.GetWeaponKnockback(setGravetender), Player.whoAmI);
                }
                if (setGravetender.buffType > 0) {
                    Player.AddBuff(setGravetender.buffType, 2, quiet: true);
                }
            }
            else {
                gravetenderGhost = -1;
            }

            if (accSentrySquid != null && sentrySquidTimer == 0) {
                SentrySquid.UseEquip(Player, this);
            }

            if (accSentryInheritence != null) {
                UpdateSentry6502();
            }

            PostUpdate_BoundBow();

            if (Main.myPlayer == Player.whoAmI) {
                darkness = GetDarkness();
            }

            PlayerContext = -1;
            EquipmentModifierUpdate = false;

            if (AequusSystem.Main_dayTime.IsCaching) {
                AequusSystem.Main_dayTime.ResetValue();
            }
        }
        public void DoDebuffEffects() {
            if (Player.HasBuff<BlueFire>()) {
                int amt = (int)(Player.Size.Length() / 16f);
                for (int i = 0; i < amt; i++) {
                    ParticleSystem.New<MonoBloomParticle>(ParticleLayer.AbovePlayers).Setup(Main.rand.NextCircularFromRect(Player.getRect()) + Main.rand.NextVector2Unit() * 8f, -Player.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(5, 20, 40, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
                }
            }
        }

        private void OnEnterCombat() {
            CheckNecromancerSetbonus();
        }

        private void OnExitCombat() {
        }

        /// <summary>
        /// Finds the closest enemy to the player, and caches its index in <see cref="Main.npc"/>
        /// </summary>
        private void PostUpdate_CheckClosestEnemy() {
            bool safe = closestEnemy == -1;
            closestEnemyOld = closestEnemy;
            closestEnemy = -1;

            var center = Player.Center;
            var checkTangle = new Rectangle((int)Player.position.X + Player.width / 2 - 1000, (int)Player.position.Y + Player.height / 2 - 500, 2000, 1000);
            float distance = 1000f;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy && Main.npc[i].CanBeChasedBy(Player) && !Main.npc[i].IsProbablyACritter()) {
                    if (Main.npc[i].getRect().Intersects(checkTangle)) {
                        float d = Main.npc[i].Distance(center);
                        if (d < distance) {
                            distance = d;
                            closestEnemy = i;
                        }
                    }
                }
            }

            if (safe) {
                if (closestEnemy > -1) {
                    OnEnterCombat();
                }
            }
            else if (closestEnemy == -1) {
                OnExitCombat();
            }
        }

        public void UpdateSentry6502() {
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].TryGetGlobalProjectile<SentryAccessoriesGlobalProj>(out var sentry)) {
                    sentry.UpdateInheritance(Main.projectile[i]);
                }
            }
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo) {
            return !CanConsume_AetherialSet();
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue) {
            if (healValue > 0)
                healValue += extraHealingPotion;
        }

        public override bool FreeDodge(Player.HurtInfo info) {
            if (sceneInvulnerability > 0) {
                return true;
            }
            if (!info.Dodgeable) {
                return false;
            }

            if (instaShieldTime > 0) {
                return true;
            }
            return false;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
        {
            //if (ExpertBoost && expertBoostBoCDefense > 60)
            //{
            //    int def = expertBoostBoCDefense;
            //    expertBoostBoCDefense -= damage;
            //    if (expertBoostBoCDefense < 5)
            //    {
            //        expertBoostBoCDefense = 5;
            //        expertBoostBoCTimer = 0;
            //        damage -= def;
            //    }
            //    else
            //    {
            //        SoundEngine.PlaySound(SoundID.NPCHit4);
            //        damage = 1;
            //    }
            //}
        }

        public override void OnHurt(Player.HurtInfo info) {
            hurtSucceeded = true;
            timeSinceLastHit = 0;
            if (Player.HasBuff(ModContent.BuffType<RitualBuff>())) {
                SoundEngine.PlaySound(SoundID.NPCDeath39, Player.Center);
                Player.ClearBuff(ModContent.BuffType<RitualBuff>());
            }
            if (info.PvP && info.DamageSource.TryGetCausingEntity(out var ent)) {
                LegacyHitEffects(Player, info.Damage, info.SourceDamage, info.Knockback, false);
            }
            InflictCrownOfBloodDownside(info);
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item) {
            CheckScam();
            MoneyBack(vendor, shopInventory, item);
        }
        public bool CheckScam() {
            return accFaultyCoin > 0f || accForgedCard > 0;
        }
        public bool MoneyBack(NPC vendor, Item[] shopInventory, Item item) {
            if (Main.rand.NextFloat() < accFaultyCoin) {
                int oldStack = item.stack;
                item.stack = 1;
                Player.GetItemExpectedPrice(item, out var sellPrice, out var buyPrice);
                item.stack = oldStack;
                item.value = 0; // A janky way to prevent infinite money, although infinite money is still possible lol
                if (buyPrice > 0) {
                    Helper.DropMoney(new EntitySource_Gift(vendor, "Aequus:FaultyCoin"), Player.getRect(), buyPrice, quiet: false);
                    return true;
                }
            }
            return false;
        }

        public override void ModifyScreenPosition() {
            ModContent.GetInstance<CameraFocus>().UpdateScreen(this);
            Main.screenPosition += ScreenShake.ScreenOffset * ClientConfig.Instance.ScreenshakeIntensity;
            Main.screenPosition = Main.screenPosition.Floor();
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) {
            modifiers.IncomingDamageMultiplier *= npc.Aequus().statAttackDamage;
            modifiers.FinalDamage.Flat -= flatDamageReduction;

            if (npc.Aequus().heatDamage && potionFrost) {
                modifiers.IncomingDamageMultiplier *= FrostPotion.DamageMultiplier;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers) {
            var aequus = proj.Aequus();
            if (aequus.HasNPCOwner) {
                modifiers.IncomingDamageMultiplier *= Main.npc[aequus.sourceNPC].Aequus().statAttackDamage;
            }
            if (proj.Aequus().heatDamage && potionFrost) {
                modifiers.IncomingDamageMultiplier *= FrostPotion.DamageMultiplier;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            modifiers.CritDamage += highSteaksDamage;
            ModifyHit_SoulDamage(target, ref modifiers);
            if (NPCSets.IsHallow.Contains(target.whoAmI)) {
                modifiers.FinalDamage += lightDamage;
            }
            if (NPCSets.IsCorrupt.Contains(target.whoAmI) || NPCSets.IsCrimson.Contains(target.whoAmI)) {
                modifiers.FinalDamage += darkDamage;
            }
        }

        private void LegacyHitEffects(Entity target, int damage, int sourceDamage, float kb, bool crit) {
            var entity = new EntityCommons(target);
            int deathsEmbrace = Player.FindBuffIndex(ModContent.BuffType<DeathsEmbraceBuff>());
            if (deathsEmbrace != -1) {
                Player.buffTime[deathsEmbrace] = Math.Max(Player.buffTime[deathsEmbrace], 300);
            }

            if (crit) {
                UseHighSteaks(target);
            }

            if (accLittleInferno > 0) {
                entity.AddBuff(BuffID.OnFire, 240 * accLittleInferno);
                if (crit) {
                    entity.AddBuff(BuffID.OnFire3, 180 * accLittleInferno);
                }
            }
            if (accMothmanMask != null && Player.statLife >= Player.statLifeMax2 && crit) {
                AequusBuff.ApplyBuff<BlueFire>(target, 300 * accMothmanMask.EquipmentStacks(), out bool canPlaySound);
                if (canPlaySound) {
                    ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center);
                }
            }
            if (accBlackPhial > 0) {
                BlackPhial.OnHitEffects(this, target, sourceDamage, kb, crit);
            }
            if (accBoneBurningRing > 0) {
                entity.AddBuff(BuffID.OnFire3, 360 * accBoneBurningRing);
            }
        }
        private void CheckLeechHook(NPC target, int damage) {
            if (leechHookNPC == target.whoAmI) {
                int lifeHealed = Math.Min(Math.Max(damage / 5, 1), Math.Clamp((int)Player.lifeSteal, 1, 10));
                int lifeSteal = CalcHealing(Player, lifeHealed);
                Player.Heal(lifeHealed);
                if (lifeSteal > 0) {
                    Player.lifeSteal -= lifeSteal;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            LegacyHitEffects(target, hit.Damage, hit.SourceDamage, hit.Knockback, hit.Crit);
            OnHit_SoulDamage(target, hit, damageDone);
            OnHit_BoneRing(target);
            OnHit_Anchor(target, hit);
            if (target.life <= 0) {
                return;
            }
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) {
            if (!target.immortal)
                CheckLeechHook(target, hit.SourceDamage);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) {
            if (!target.immortal && proj.type != ModContent.ProjectileType<LeechHookProj>())
                CheckLeechHook(target, hit.SourceDamage);
        }

        public override void OnHitAnything(float x, float y, Entity victim) {
            OnHitAnything_Vampire(x, y, victim);
        }

        public override bool ConsumableDodge(Player.HurtInfo info) {
            if (crownOfBloodCD <= 0) {
                return TryWormScarfDodge() || TryBoCDodge();
            }
            return false;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            if (item.DamageType != null) {
                velocity *= 1f + Math.Min(statProjectileSpeed.GetTotalStats(item.DamageType), 0.5f);
            }
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (accRamishroom != null && item.fishingPole > 0) {
                int amt = accRamishroom.HasAbilityBoost() ? 2 : 1;
                for (int i = 0; i < amt; i++) {
                    Projectile.NewProjectile(Player.GetSource_Accessory(accRamishroom), position, velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)),
                        ModContent.ProjectileType<RamishroomBobber>(), damage, knockback, Player.whoAmI);
                }
            }
            return true;
        }

        public override void ModifyItemScale(Item item, ref float scale) {
            if (item.DamageType != null && item.DamageType.CountsAsClass(DamageClass.Melee)) {
                scale += Math.Min(statMeleeScale, 0.5f);
            }
        }

        public override void SaveData(TagCompound tag) {
            SaveDataAttribute.SaveData(tag, this);
            SaveData_Vampire(tag);
        }

        public override void LoadData(TagCompound tag) {
            SaveDataAttribute.LoadData(tag, this);
            LoadData_Vampire(tag);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
            if (!Main.gameMenu && Player.HeldItem.ModItem is ItemHooks.IPreDrawPlayer preDrawPlayer) {
                preDrawPlayer.PreDrawPlayer(Player, this, ref drawInfo);
            }
            if (Player.front > 0 && Player.front == PumpkingCloak.FrontID) {
                drawInfo.hidesBottomSkin = true;
                drawInfo.hidesTopSkin = true;
            }
            if (stackingHat > 0 && StackingHatEffect.Blacklist.Contains(drawInfo.drawPlayer.head)) {
                stackingHat = 0;
            }
            if (stackingHat > 0) {
                drawInfo.hideHair = false;
                drawInfo.fullHair = true;
                drawInfo.hatHair = true;
            }
            if (eyeGlint) {
                drawInfo.colorEyeWhites = Color.White;
                drawInfo.colorEyes = drawInfo.drawPlayer.eyeColor;
            }
            if (CustomDrawShadow != null) {
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
            ModifyDrawInfo_Vampire(ref drawInfo);
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo) {
            if (Player.front > 0 && Player.front == PumpkingCloak.FrontID) {
                PlayerDrawLayers.Torso.Hide();
                PlayerDrawLayers.Leggings.Hide();
            }
        }

        public float GetDarkness() {
            if (Main.myPlayer == Player.whoAmI) // Should always be true anyways, but here for safe-ness I guess.
            {
                var tilePosition = Player.Center.ToTileCoordinates();
                return Math.Clamp(1f - Lighting.Brightness(tilePosition.X, tilePosition.Y), 0f, 1f);
            }
            return 0f;
        }

        /// <summary>
        /// Called right before all player layers have been drawn
        /// </summary>
        /// <param name="info"></param>
        public void PreDraw(ref PlayerDrawSet info) {
            if (info.headOnlyRender) {
                return;
            }

            if (!Main.gameMenu) {
                DrawEquip<HoverCrownEquip>(ref info);
                PreDraw_Stormcloak();
            }
            if (DrawScale != null) {
                var drawPlayer = info.drawPlayer;
                var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
                to -= Main.screenPosition;
                for (int i = 0; i < info.DrawDataCache.Count; i++) {
                    DrawData data = info.DrawDataCache[i];
                    data.position -= (data.position - to) * (1f - DrawScale.Value);
                    data.scale *= DrawScale.Value;
                    info.DrawDataCache[i] = data;
                }
            }
            if (DrawForceDye != null) {
                var drawPlayer = info.drawPlayer;
                for (int i = 0; i < info.DrawDataCache.Count; i++) {
                    DrawData data = info.DrawDataCache[i];
                    data.shader = DrawForceDye.Value;
                    info.DrawDataCache[i] = data;
                }
            }
            if (instaShieldFrames != 0 && instaShieldTime == instaShieldFrames) {
                int heldItemStart = ModContent.GetInstance<DrawDataTrackers.DrawHeldItem_27_Tracker>().DDIndex;
                int heldItemEnd = ModContent.GetInstance<DrawDataTrackers.ArmOverItem_28_Tracker>().DDIndex;
                var info2 = info;
                info2.DrawDataCache = new List<DrawData>(info.DrawDataCache);
                for (int i = heldItemEnd; i >= heldItemStart; i--) {
                    info2.DrawDataCache.RemoveAt(i);
                }
                var ddCache = new List<DrawData>(info2.DrawDataCache);
                foreach (var c in Helper.CircularVector(4)) {
                    for (int i = 0; i < info2.DrawDataCache.Count; i++) {
                        var dd = ddCache[i];
                        dd.position += c * 2f;
                        dd.color = Color.SkyBlue.UseA(0) * 0.1f;
                        dd.shader = Helper.ShaderColorOnlyIndex;
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
        public void PostDraw(ref PlayerDrawSet info) {
            if (info.headOnlyRender) {
                return;
            }
            if (instaShieldAlpha > 0f) {
                int heldItemStart = ModContent.GetInstance<DrawDataTrackers.DrawHeldItem_27_Tracker>().DDIndex;
                int heldItemEnd = ModContent.GetInstance<DrawDataTrackers.ArmOverItem_28_Tracker>().DDIndex;
                var info2 = info;
                info2.DrawDataCache = new List<DrawData>(info.DrawDataCache);
                for (int i = heldItemEnd; i >= heldItemStart; i--) {
                    info2.DrawDataCache.RemoveAt(i);
                }
                var ddCache = new List<DrawData>(info2.DrawDataCache);
                for (int i = 0; i < info2.DrawDataCache.Count; i++) {
                    var dd = ddCache[i];
                    dd.color = Color.SkyBlue * 2f * instaShieldAlpha;
                    dd.shader = Helper.ShaderColorOnlyIndex;
                    info2.DrawDataCache[i] = dd;
                }
                PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref info2);
            }
            PostDraw_Stormcloak();
            FoolsGoldRing.DrawCounter(ref info);
        }

        public void RefreshJumpOption() {
            if (Player.hasJumpOption_Cloud && !Player.isPerformingJump_Cloud && !Player.canJumpAgain_Cloud) {
                Player.canJumpAgain_Cloud = true;
            }
            else if (Player.hasJumpOption_Blizzard && !Player.isPerformingJump_Blizzard && !Player.canJumpAgain_Blizzard) {
                Player.canJumpAgain_Blizzard = true;
            }
            else if (Player.hasJumpOption_Sandstorm && !Player.isPerformingJump_Sandstorm && !Player.canJumpAgain_Sandstorm) {
                Player.canJumpAgain_Sandstorm = true;
            }
            else if (Player.hasJumpOption_Fart && !Player.isPerformingJump_Fart && !Player.canJumpAgain_Fart) {
                Player.canJumpAgain_Fart = true;
            }
            else if (Player.hasJumpOption_Sail && !Player.isPerformingJump_Sail && !Player.canJumpAgain_Sail) {
                Player.canJumpAgain_Sail = true;
            }
            else if (Player.hasJumpOption_Basilisk && !Player.isPerformingJump_Basilisk && !Player.canJumpAgain_Basilisk) {
                Player.canJumpAgain_Basilisk = true;
            }
            else if (Player.hasJumpOption_Unicorn && !Player.isPerformingJump_Unicorn && !Player.canJumpAgain_Unicorn) {
                Player.canJumpAgain_Unicorn = true;
            }
            else if (Player.hasJumpOption_WallOfFleshGoat && !Player.isPerformingJump_WallOfFleshGoat && !Player.canJumpAgain_WallOfFleshGoat) {
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
        public void SetCooldown(int cooldown, bool ignoreStats = false, Item itemReference = null) {
            if (cooldown < itemCooldown) {
                return;
            }

            itemCooldownMax = (ushort)cooldown;
            itemCooldown = (ushort)cooldown;
        }

        public int ProjectilesOwned(int type) {
            int count = 0;
            if (projectileIdentity != -1) {
                int myProj = Helper.FindProjectileIdentity(Player.whoAmI, projectileIdentity);
                if (myProj != -1) {
                    for (int i = 0; i < Main.maxProjectiles; i++) {
                        if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].type == type
                            && Main.projectile[i].Aequus().sourceProjIdentity == projectileIdentity) {
                            count++;
                        }
                    }
                }
                return count;
            }
            if (Main.myPlayer != Player.whoAmI) {
                return count + 1;
            }
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].type == type
                    && Main.projectile[i].Aequus().sourceProjIdentity == -1) {
                    count++;
                }
            }
            return count;
        }

        public void OnKillEffect(EnemyKillInfo npc) {
            SoulGem.TryFillSoulGems(Player, this, npc);
            AmmoBackpack.Proc(Player, this, npc);
        }

        public bool PreCreatureSpawns() {
            if (Player.InModBiome<FakeUnderworldBiome>()) {
                float y = Player.position.Y;
                Player.position.Y = Math.Max(Player.position.Y, (Main.UnderworldLayer + 80) * 16);
                Player.ZoneUnderworldHeight = true;
                AequusNPC.spawnNPCYOffset = y - Player.position.Y;
                return true;
            }
            return false;
        }

        public void DetermineBuffTimeToAdd(int type, ref int amt) {
            if (amt < 3600)
                return;
            if (Main.debuff[type]) {
                if (debuffDuration != 1f)
                    amt = (int)(amt * debuffDuration);
            }
            else {
                if (buffDuration != 1f && !Main.meleeBuff[type])
                    amt = (int)(amt * buffDuration);
            }
        }

        public static bool CanScamNPC(NPC npc) {
            return npc.type != ModContent.NPCType<Exporter>();
        }

        public void CountSentries() {
            turretSlotCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].WipableTurret) {
                    turretSlotCount++;
                }
            }
        }

        public override void ModifyLuck(ref float luck) {
            luck += addLuck;
        }

        public static void ShootProj(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 location, Vector2 velocity, int projType, int projDamage, float projKB, Vector2? setMousePos) {
            if (Player_ItemCheck_Shoot != null) {
                int mouseX = Main.mouseX;
                int mouseY = Main.mouseY;

                if (setMousePos != null) {
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
        private static int LegacyShootProj(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 location, Vector2 velocity, int projType, int projDamage, float projKB, Vector2? setMousePos) {
            int mouseX = Main.mouseX;
            int mouseY = Main.mouseY;

            if (source == null) {
                source = new EntitySource_ItemUse_WithAmmo(player, item, 0);
            }

            if (setMousePos != null) {
                var mousePos = setMousePos.Value - Main.screenPosition;
                Main.mouseX = (int)mousePos.X;
                Main.mouseX = (int)mousePos.Y;
            }

            CombinedHooks.ModifyShootStats(player, item, ref location, ref velocity, ref projType, ref projDamage, ref projKB);

            int result;
            if (CombinedHooks.Shoot(player, item, source, location, velocity, projType, projDamage, projKB)) {
                result = Projectile.NewProjectile(source, location, velocity, projType, projDamage, projKB, player.whoAmI);
            }
            else {
                result = -2;
            }

            Main.mouseX = mouseX;
            Main.mouseY = mouseY;
            return result;
        }

        /// <summary>
        /// Creates a cloned player for a fake projectile-player clone.
        /// </summary>
        /// <param name="basePlayer"></param>
        /// <returns></returns>
        public static Player ProjectileClone(Player basePlayer) {
            var p = basePlayer.clientClone();
            p.boneGloveItem = basePlayer.boneGloveItem?.Clone();
            p.boneGloveTimer = basePlayer.boneGloveTimer;
            p.volatileGelatin = basePlayer.volatileGelatin;
            p.volatileGelatinCounter = basePlayer.volatileGelatinCounter;
            return p;
        }

        private static List<Item> _equipList;

        /// <summary>
        /// Gets a list of equipment from the player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="armor"></param>
        /// <param name="accessories"></param>
        /// <param name="sentrySlot"></param>
        /// <returns></returns>
        public static IEnumerable<Item> GetEquips(Player player, bool armor = true, bool accessories = true, bool sentrySlot = false) {
            var _equipList = new List<Item>();
            if (armor) {
                for (int i = 0; i < 3; i++)
                    _equipList.Add(player.armor[i]);
            }
            if (accessories) {
                for (int i = 3; i < 10; i++) {
                    if (player.IsItemSlotUnlockedAndUsable(i))
                        _equipList.Add(player.armor[i]);
                }
                if (sentrySlot && player.Aequus().accSentrySlot) {
                    var item = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<Sentinel6510AccessorySlot>().Type, player);
                    if (item.FunctionalItem != null && !item.FunctionalItem.IsAir)
                        _equipList.Add(item.FunctionalItem);
                }
            }
            return _equipList;
        }

        #region Hooks
        private static void LoadHooks() {
            On_Player.DashMovement += Player_DashMovement;
            On_Player.PlaceThing_Tiles_CheckLavaBlocking += Player_PlaceThing_Tiles_CheckLavaBlocking;
            On_Player.KeyDoubleTap += Player_KeyDoubleTap;
            Terraria.GameContent.Golf.On_FancyGolfPredictionLine.Update += FancyGolfPredictionLine_Update;
            On_Player.CheckSpawn += Player_CheckSpawn;
            On_Player.JumpMovement += Player_JumpMovement;
            On_Player.DropTombstone += Player_DropTombstone;
            On_NPC.NPCLoot_DropMoney += NPC_NPCLoot_DropMoney;
            On_Player.RollLuck += Player_RollLuck;
            On_Player.GetItemExpectedPrice += Hook_GetItemPrice;
            On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += PlayerDrawLayers_DrawPlayer_RenderAllLayers;
            On_Player.PickTile += Player_PickTile;
            On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick;
            On_ItemSlot.OverrideLeftClick += ItemSlot_OverrideLeftClick;
        }

        private static void Player_DashMovement(On_Player.orig_DashMovement orig, Player self) {
            foreach (var n in DashImmunityHack) {
                n.dontTakeDamage = true;
            }
            try {
                orig(self);
            }
            catch {
            }
            foreach (var n in DashImmunityHack) {
                n.dontTakeDamage = false;
            }
        }

        private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            if (Main.mouseRight && Main.mouseRightRelease) {
                var player = Main.LocalPlayer;
                var aequus = player.Aequus();
                if (Main.mouseItem.ModItem is ItemHooks.IRightClickOverrideWhenHeld rightClickOverride && rightClickOverride.RightClickOverrideWhileHeld(ref Main.mouseItem, inv, context, slot, player, aequus)) {
                    return;
                }

                if (context == ItemSlot.Context.InventoryItem) {
                    if (SkeletonKey.UseSkeletonKey(inv, slot, player, aequus)) {
                        return;
                    }
                }
            }

            orig(inv, context, slot);
        }

        private static bool ItemSlot_OverrideLeftClick(On_ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot) {

            if (ItemSlot_OverrideLeftClick_MoneyTrashcan(inv, context, slot)) {
                return true;
            }
            //ItemSlot.Context.EquipAccessory
            if (ItemSlot_OverrideLeftClick_FaultyCoin(inv, context, slot)) {
                return true;
            }

            return orig(inv, context, slot);
        }

        private static bool Player_PlaceThing_Tiles_CheckLavaBlocking(On_Player.orig_PlaceThing_Tiles_CheckLavaBlocking orig, Player player) {
            if (player.Aequus().accLavaPlace)
                return false;
            return orig(player);
        }

        private static void Player_KeyDoubleTap(On_Player.orig_KeyDoubleTap orig, Player player, int keyDir) {
            orig(player, keyDir);
            if ((Main.ReversedUpDownArmorSetBonuses ? 1 : 0) != keyDir) {
                return;
            }
            var aequus = player.Aequus();
            if (aequus.setbonusRef?.ModItem is ItemHooks.ISetbonusDoubleTap doubleTap) {
                doubleTap.OnDoubleTap(player, aequus, keyDir);
            }
        }

        private static void Player_PickTile(On_Player.orig_PickTile orig, Player self, int x, int y, int pickPower) {
            pickPower = (int)(pickPower * self.Aequus().pickTileDamage);
            orig(self, x, y, pickPower);
        }

        private static void FancyGolfPredictionLine_Update(Terraria.GameContent.Golf.On_FancyGolfPredictionLine.orig_Update orig, Terraria.GameContent.Golf.FancyGolfPredictionLine self, Entity golfBall, Vector2 impactVelocity, float roughLandResistance) {
            bool solid = Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()];
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
            orig(self, golfBall, impactVelocity, roughLandResistance);
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = solid;
        }

        private static bool Player_CheckSpawn(On_Player.orig_CheckSpawn orig, int x, int y) {
            bool solid = Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()];
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
            bool originalValue = orig(x, y);
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = solid;
            return originalValue;
        }

        private static void Player_JumpMovement(On_Player.orig_JumpMovement orig, Player self) {
            if (self.Aequus().gravityTile != 0) {
                self.gravDir = Math.Sign(self.Aequus().gravityTile);
            }
            orig(self);
        }

        private static void Player_DropTombstone(On_Player.orig_DropTombstone orig, Player player, long coinsOwned, NetworkText deathText, int hitDirection) {
            if (player.Aequus().ghostTombstones) {
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    NPC.NewNPCDirect(player.GetSource_Death("Ghostly Grave"), player.Center, NPCID.Ghost);
                }
                return;
            }
            if (player.position.Y > Main.UnderworldLayer * 16f) {
                if (Main.myPlayer == player.whoAmI) {
                    int projType = Main.rand.NextFromList(
                        ModContent.ProjectileType<AshTombstoneProj>(),
                        ModContent.ProjectileType<AshGraveMarkerProj>(),
                        ModContent.ProjectileType<AshCrossGraveMarkerProj>(),
                        ModContent.ProjectileType<AshHeadstoneProj>(),
                        ModContent.ProjectileType<AshGravestoneProj>(),
                        ModContent.ProjectileType<AshObeliskProj>()
                    );
                    Projectile.NewProjectile(player.GetSource_Death(), player.Center,
                        new(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-12f, -4f)),
                        projType, 40, 12f, player.whoAmI);
                }
                return;
            }

            orig(player, coinsOwned, deathText, hitDirection);
        }

        private static void NPC_NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer) {
            if (closestPlayer.Aequus().accGrandReward) {
                return;
            }
            orig(self, closestPlayer);
        }

        private static int Player_RollLuck(On_Player.orig_RollLuck orig, Player self, int range) {
            int rolled = orig(self, range);
            if (Helper.iterations == 0) {
                Helper.iterations++;
                try {
                    rolled = self.Aequus().RerollLuck(rolled, range);
                }
                catch {
                }
                Helper.iterations = 0;
            }
            return rolled;
        }
        public int RerollLuck(int rolledAmt, int range) {
            for (float luckLeft = luckRerolls; luckLeft > 0f; luckLeft--) {
                if (luckLeft < 1f) {
                    if (Main.rand.NextFloat(1f) > luckLeft) {
                        return rolledAmt;
                    }
                }
                rolledAmt = Math.Min(rolledAmt, Player.RollLuck(range));
                if (rolledAmt <= 0) {
                    return 0;
                }
            }
            return rolledAmt;
        }

        private static void Hook_GetItemPrice(On_Player.orig_GetItemExpectedPrice orig, Player self, Item item, out long calcForSelling, out long calcForBuying) {
            orig(self, item, out calcForSelling, out calcForBuying);

            if (self.talkNPC != -1 && !CanScamNPC(Main.npc[self.talkNPC])) {
                return;
            }

            var aequus = self.Aequus();
            calcForBuying = (int)(calcForBuying + item.value * aequus.increasedSellPrice);
            if (item.shopSpecialCurrency != -1 || self.talkNPC == -1) {
                return;
            }

            int min = item.shopCustomPrice.GetValueOrDefault(item.value) / 5;
            if (calcForBuying < min) // shrug
            {
                return;
            }
            calcForBuying = Math.Max(calcForBuying - aequus.accForgedCard, min);
        }

        private static bool customDraws;
        private static void PlayerDrawLayers_DrawPlayer_RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo) {
            try {
                if (customDraws) {
                    orig(ref drawinfo);
                    return;
                }
                customDraws = true;
                drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PreDraw(ref drawinfo);
                orig(ref drawinfo);
                drawinfo.drawPlayer.GetModPlayer<AequusPlayer>().PostDraw(ref drawinfo);
            }
            catch {

            }
            customDraws = false;
        }
        #endregion

        /// <summary>
        /// Spawns Flask and other "Enchantment" dusts, like the Magma Stone flames.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="player"></param>
        /// <param name="showMagmaStone"></param>
        public static void SpawnEnchantmentDusts(Vector2 position, Vector2 velocity, Player player, bool showMagmaStone = true) {
            if (player.magmaStone && showMagmaStone) {
                var d = Dust.NewDustPerfect(position, DustID.Torch, velocity * 2f, Alpha: 100, Scale: 2.5f);
                d.noGravity = true;
            }
            switch (player.meleeEnchant) {
                case FlaskID.Venom: {
                        if (Main.rand.NextBool(3)) {
                            var d = Dust.NewDustPerfect(position, DustID.Venom, velocity * 2f, Alpha: 100);
                            d.noGravity = true;
                            d.fadeIn = 1.5f;
                            d.velocity *= 0.25f;
                        }
                    }
                    break;

                case FlaskID.CursedInferno: {
                        if (Main.rand.NextBool(2)) {
                            var d = Dust.NewDustPerfect(position, DustID.CursedTorch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                            d.noGravity = true;
                            d.velocity *= 0.7f;
                            d.velocity.Y -= 0.5f;
                        }
                    }
                    break;

                case FlaskID.Fire: {
                        if (Main.rand.NextBool(2)) {
                            var d = Dust.NewDustPerfect(position, DustID.Torch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                            d.noGravity = true;
                            d.velocity *= 0.7f;
                            d.velocity.Y -= 0.5f;
                        }
                    }
                    break;

                case FlaskID.Midas: {
                        if (Main.rand.NextBool(2)) {
                            var d = Dust.NewDustPerfect(position, DustID.Enchanted_Gold, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                            d.noGravity = true;
                            d.velocity *= 0.7f;
                            d.velocity.Y -= 0.5f;
                        }
                    }
                    break;

                case FlaskID.Ichor: {
                        if (Main.rand.NextBool(2)) {
                            var d = Dust.NewDustPerfect(position, DustID.IchorTorch, velocity, Alpha: 100, Scale: 2.5f);
                            d.velocity.X += player.direction;
                            d.velocity.Y -= 0.2f;
                        }
                    }
                    break;

                case FlaskID.Nanites: {
                        if (Main.rand.NextBool(2)) {
                            var d = Dust.NewDustPerfect(position, DustID.IceTorch, velocity, Alpha: 100, Scale: 2.5f);
                            d.velocity.X += player.direction;
                            d.velocity.Y -= 0.2f;
                        }
                    }
                    break;

                case FlaskID.Party: {
                        if (Main.rand.NextBool(40)) {
                            var g = Gore.NewGorePerfect(player.GetSource_ItemUse(player.HeldItem), position, velocity, Main.rand.Next(276, 283));
                            g.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            g.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            g.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                            g.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                            g.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        }
                        else if (Main.rand.NextBool(20)) {
                            var d = Dust.NewDustPerfect(position, Main.rand.Next(139, 143), velocity, Scale: 1.2f);
                            d.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            d.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                            d.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                            d.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                            d.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                        }
                    }
                    break;

                case FlaskID.Poison: {
                        if (Main.rand.NextBool(3)) {
                            var d = Dust.NewDustPerfect(position, DustID.Poisoned, velocity * 2f, Alpha: 100);
                            d.noGravity = true;
                            d.fadeIn = 1.5f;
                            d.velocity *= 0.25f;
                        }
                    }
                    break;
            }
            foreach (var c in SpawnEnchantmentDusts_Custom) {
                if (c.Item2(player)) {
                    c.Item3(Dust.NewDustPerfect(position, c.Item1, velocity));
                }
            }
        }

        /// <summary>
        /// Gets a player context for applying specific effects.
        /// </summary>
        /// <returns></returns>
        public static Player CurrentPlayerContext() {
            if (PlayerContext > -1) {
                return Main.player[PlayerContext];
            }
            if (AequusProjectile.pWhoAmI != -1 && Main.projectile[AequusProjectile.pWhoAmI].friendly) {
                return Main.player[Main.projectile[AequusProjectile.pWhoAmI].owner];
            }
            return null;
        }

        /// <summary>
        /// Calculates how much to actually heal.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="healAmt"></param>
        /// <returns></returns>
        public static int CalcHealing(Player player, int healAmt) {
            if (player.statLife + healAmt > player.statLifeMax2)
                return player.statLifeMax2 - player.statLife;
            return healAmt;
        }

        public bool TryGetBoostedItem(Item item, out int stacks) {
            stacks = 0;
            if (item == null || item.IsAir) {
                return false;
            }

            stacks = item.EquipmentStacks();
            return true;
        }
    }
}