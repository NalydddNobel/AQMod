using AQMod.Assets;
using AQMod.Buffs;
using AQMod.Buffs.Debuffs;
using AQMod.Buffs.Summon;
using AQMod.Buffs.Temperature;
using AQMod.Common.Graphics;
using AQMod.Content;
using AQMod.Content.World.Events;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.Particles;
using AQMod.Items;
using AQMod.Items.Accessories.Fishing;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Armor.Arachnotron;
using AQMod.Items.Expert;
using AQMod.Items.Tools.GrapplingHooks;
using AQMod.NPCs;
using AQMod.Projectiles;
using AQMod.Projectiles.Summon;
using AQMod.Projectiles.Summon.PassiveHelmets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod
{
    public sealed class AQPlayer : ModPlayer
    {
        public static class Hooks
        {
            internal static void Apply()
            {
                On.Terraria.Player.AddBuff += OnAddBuff;
                On.Terraria.Player.PickTile += HitTile;
                On.Terraria.Player.HorizontalMovement += MovementEffects;
                On.Terraria.Chest.SetupShop += ApplyCustomDiscount;
            }

            internal static void ApplyCustomDiscount(On.Terraria.Chest.orig_SetupShop orig, Chest self, int type)
            {
                var plr = Main.LocalPlayer;
                bool discount = plr.discount;
                plr.discount = false;

                orig(self, type);

                plr.discount = discount;
                if (discount)
                {
                    float discountPercentage = plr.GetModPlayer<AQPlayer>().discountPercentage;
                    for (int i = 0; i < Chest.maxItems; i++)
                    {
                        if (self.item[i] != null && self.item[i].type != ItemID.None)
                            self.item[i].value = (int)(self.item[i].value * discountPercentage);
                    }
                }
                //orig(self, type);
                //var player = Main.LocalPlayer;
                //if (player.discount)
                //{
                //    float discountPercentage = player.GetModPlayer<AQPlayer>().discountPercentage;
                //    if (discountPercentage == 1f)
                //    {
                //        discountPercentage = 0.8f;
                //    }
                //    for (int i = 0; i < Chest.maxItems; i++)
                //    {
                //        if (self.item[i] != null && self.item[i].type != ItemID.None)
                //        {
                //            self.item[i].value = (int)(self.item[i].GetGlobalItem<AQItem>().basePrice * discountPercentage);
                //        }
                //    }
                //}
            }

            internal static void MovementEffects(On.Terraria.Player.orig_HorizontalMovement orig, Player self)
            {
                orig(self);
                var aQPlayer = self.GetModPlayer<AQPlayer>();
                if (aQPlayer.redSpriteWind != 0 && !(self.mount.Active && self.velocity.Y == 0f && (self.controlLeft || self.controlRight)))
                {
                    float windDirection = Math.Sign(aQPlayer.redSpriteWind) * 0.07f;
                    if (Math.Abs(Main.windSpeed) > 0.5f)
                    {
                        windDirection *= 1.37f;
                    }
                    if (self.velocity.Y != 0f)
                    {
                        windDirection *= 1.5f;
                    }
                    if (self.controlLeft || self.controlRight)
                    {
                        windDirection *= 0.8f;
                    }
                    if (Math.Sign(self.direction) != Math.Sign(windDirection))
                    {
                        self.accRunSpeed -= Math.Abs(windDirection) * 20f;
                        self.maxRunSpeed -= Math.Abs(windDirection) * 20f;
                    }
                    if (windDirection < 0f && self.velocity.X > windDirection)
                    {
                        self.velocity.X += windDirection;
                        if (self.velocity.X < windDirection)
                        {
                            self.velocity.X = windDirection;
                        }
                    }
                    if (windDirection > 0f && self.velocity.X < windDirection)
                    {
                        self.velocity.X += windDirection;
                        if (self.velocity.X > windDirection)
                        {
                            self.velocity.X = windDirection;
                        }
                    }

                    if (!self.controlLeft && !self.controlRight)
                    {
                        self.legFrameCounter = -1.0;
                        self.legFrame.Y = 0;
                    }
                }
                aQPlayer.redSpriteWind = 0;
            }

            internal static void HitTile(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
            {
                if (self.GetModPlayer<AQPlayer>().pickBreak)
                {
                    pickPower /= 2;
                }
                orig(self, x, y, pickPower);
            }

            internal static void OnAddBuff(On.Terraria.Player.orig_AddBuff orig, Player self, int type, int time1, bool quiet)
            {
                if (type >= Main.maxBuffTypes)
                {
                    var modBuff = ModContent.GetModBuff(type);
                    if (modBuff is temperatureDebuff)
                    {
                        for (int i = 0; i < Player.MaxBuffs; i++)
                        {
                            if (self.buffTime[i] > 0)
                            {
                                if (self.buffType[i] == type)
                                {
                                    orig(self, type, time1, quiet);
                                    return;
                                }
                                if (self.buffType[i] > Main.maxBuffTypes)
                                {
                                    var otherModBuff = ModContent.GetModBuff(self.buffType[i]);
                                    if (otherModBuff is temperatureDebuff)
                                    {
                                        self.DelBuff(i);
                                        orig(self, type, time1, quiet);
                                        return;
                                    }
                                }
                            }
                        }
                        orig(self, type, time1, quiet);
                        return;
                    }
                }
                if (AQBuff.Sets.FoodBuff.Contains(type))
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (self.buffTime[i] > 16 && self.buffType[i] != type && AQBuff.Sets.FoodBuff.Contains(self.buffType[i]))
                        {
                            self.DelBuff(i);
                            i--;
                        }
                    }
                }
                orig(self, type, time1, quiet);
            }
        }

        public const int MaxCelesteTorusOrbs = 5;
        public const int MaxArmor = 20;
        public const int MaxDye = MaxArmor / 2;
        public const int FRAME_HEIGHT = 56;
        public const int FRAME_COUNT = 20;
        public const float CELESTE_Z_MULT = 0.0157f;
        public const int ARACHNOTRON_OLD_POS_LENGTH = 8;
        public const byte TEMPERATURE_REGEN_NORMAL = 32;
        public const byte TemperatureRegenWhenResistant = 8;
        public const byte TEMPERATURE_REGEN_ON_HIT = 120;

        public static bool forceAutoswing;
        public static byte extractinatorBlipDelay;

        public float discountPercentage = 1f;
        public bool blueSpheres;
        public bool hyperCrystal;
        public bool shimmering;
        public bool ammoRenewal;
        public bool altEvilDrops;
        public bool breadsoul;
        public bool moonShoes;
        public bool reducedSiltDamage;
        public bool copperSeal;
        public bool silverSeal;
        public bool goldSeal;
        public bool dashEquipped;
        public int extraFlightTime;
        public int thunderbirdLightningTimer;
        public bool dreadsoul;
        public bool omori;
        public bool omoriEffect;
        public int omoriDeathTimer;
        public int spelunkerEquipTimer;
        public bool omegaStarite;
        public byte lootIterations;
        public bool featherflightAmulet;
        public bool voodooAmulet;
        public bool extractinatorCounter;
        public float celesteTorusX;
        public float celesteTorusY;
        public float celesteTorusZ;
        public float celesteTorusRadius;
        public int celesteTorusDamage;
        public float celesteTorusKnockback;
        public int celesteTorusMaxRadius;
        public float celesteTorusSpeed;
        public float celesteTorusScale;
        public bool striderPalms;
        public bool striderPalmsOld;
        public bool degenerationRing;
        public ushort shieldLife;
        public bool blueFire;
        public float grabReach; // until 1.4 comes
        public bool mothmanMask;
        public byte mothmanExplosionDelay;
        public sbyte temperature;
        public byte temperatureRegen;
        public bool pickBreak;
        public bool crabAx;
        public sbyte redSpriteWind;
        public byte extraHP;
        public bool fidgetSpinner;
        public bool ignoreMoons;
        public bool canToggleCosmicanon;
        public bool antiGravityItems;
        public bool hotAmulet;
        public bool coldAmulet;
        public bool shockCollar;
        public bool healBeforeDeath;
        public bool glowString;
        public bool pearlAmulet;
        public bool bloodthirst;
        public byte bloodthirstDelay;
        public bool spreadDebuffs;
        public bool shade;
        public bool undetectable;
        public bool doubleExtractorLoot;

        public bool helmetFlowerCrown;
        public bool helmetDartTrap;
        public int dartHeadType;
        public int passiveSummonDelay;
        public int passiveSummonTimer;

        public bool setLightbulb;
        public bool arachnotronArms;
        public bool setArachnotron;
        public bool setArachnotronCooldown;

        public bool leechHook;
        public bool meathookUI;
        public int meathookTarget;
        public bool meathook;
        public bool hasMeathookNPCOld;
        public bool hasMeathookNPC;
        public int meathookNPC;
        public int hookDamage;
        public List<BuffData> hookDebuffs;

        public float evilEnemyDR;
        public float holyEnemyDR;
        public int healEffectValueForSyncingTheThingOnTheServer;

        public bool heartMoth;
        public bool anglerFish;
        public bool dwarfStarite;

        public bool crimsonHands;
        public bool chomper;
        public bool piranhaPlant;
        public bool trapperImp;
        public bool starite;
        public bool monoxiderBird;
        public bool snowsaw;
        public int snowsawLeader = -1;

        public bool hasMinionCarry;
        public int headMinionCarryX;
        public int headMinionCarryY;
        public int headMinionCarryXOld;
        public int headMinionCarryYOld;
        public byte monoxiderCarry;

        public float unholyDamage;
        public float holyDamage;

        public float runSpeedBoost = 1f;
        public float wingSpeedBoost = 1f;

        public ushort itemCooldownMax;
        public ushort itemCooldown;
        public ushort itemCombo;
        public ushort itemSwitch;
        public int lastSelectedItem = -1;
        public int critIncrease;
        public uint interactionDelay;

        public int extractorHelmet;
        public int extractorAirMask;
        public int ExtractinatorCount;
        public bool IgnoreIgnoreMoons;

        public (int, int)[] AmmoUsageData;

        public override void Initialize()
        {
            extractinatorBlipDelay = 0;

            snowsawLeader = -1;
            omoriDeathTimer = 1;
            arachnotronArms = false;
            lootIterations = 0;
            shimmering = false;
            headMinionCarryX = 0;
            headMinionCarryY = 0;
            headMinionCarryXOld = 0;
            headMinionCarryYOld = 0;
            monoxiderCarry = 0;
            blueFire = false;
            grabReach = 1f;
            temperature = 0;
            pickBreak = false;
            fidgetSpinner = false;
            bloodthirstDelay = 0;
            healEffectValueForSyncingTheThingOnTheServer = 0;
            hookDebuffs?.Clear();
            hookDebuffs = new List<BuffData>();
            meathookUI = false;
            interactionDelay = 0;
            AmmoUsageData = new (int, int)[4];
        }

        public override void OnEnterWorld(Player player)
        {
            if (!Main.dayTime && Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                SkyGlimmerEvent.InitNight();
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["extractinatorCount"] = ExtractinatorCount,
                ["IgnoreIgnoreMoons"] = IgnoreIgnoreMoons,
            };
        }

        public override void Load(TagCompound tag)
        {
            IgnoreIgnoreMoons = tag.GetBool("IgnoreIgnoreMoons");
            ExtractinatorCount = tag.GetInt("extractinatorCount");
        }

        public override void UpdateDead()
        {
            ResetEffects_Debuffs();
            itemCooldown = 0;
            itemCooldownMax = 0;
            itemCombo = 0;
            itemSwitch = 0;
            omori = false;
            omoriEffect = false;
            blueSpheres = false;
            monoxiderCarry = 0;
            temperature = 0;
            temperatureRegen = TEMPERATURE_REGEN_ON_HIT;
            mothmanExplosionDelay = 0;
            passiveSummonDelay = 0;
            bloodthirstDelay = 0;
            healEffectValueForSyncingTheThingOnTheServer = 0;
            hookDebuffs = new List<BuffData>();
            hookDamage = 0;
            meathookUI = false;
            snowsawLeader = -1;
            runSpeedBoost = 1f;
            wingSpeedBoost = 1f;
        }

        private void UpdateVisuals_Flashes()
        {
            if (FX.flashLocation != Vector2.Zero)
            {
                LegacyEffectCache.f_Flash.GetShader()
                .UseIntensity(Math.Max(FX.flashBrightness * AQConfigClient.Instance.EffectIntensity, 1f / 18f));
                if (!LegacyEffectCache.f_Flash.IsActive())
                {
                    Filters.Scene.Activate(LegacyEffectCache.fn_Flash, FX.flashLocation, null).GetShader()
                    .UseOpacity(1f)
                    .UseTargetPosition(FX.flashLocation);
                }
                FX.flashBrightness -= FX.flashBrightnessDecrement;
                if (FX.flashBrightness <= 0f)
                {
                    FX.flashLocation = Vector2.Zero;
                    FX.flashBrightness = 0f;
                    FX.flashBrightnessDecrement = 0.05f;
                }
            }
            else
            {
                if (LegacyEffectCache.f_Flash.IsActive())
                {
                    LegacyEffectCache.f_Flash.GetShader()
                        .UseIntensity(0f)
                        .UseProgress(0f)
                        .UseOpacity(0f);
                    Filters.Scene.Deactivate(LegacyEffectCache.fn_Flash, null);
                }
            }
        }
        public override void UpdateBiomeVisuals()
        {
            UpdateVisuals_Flashes();
            FX.Update();

            if (extractinatorBlipDelay > 0)
                extractinatorBlipDelay--;
        }

        private void ResetEffects_Cooldowns()
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
                player.manaRegen = 0;
                player.manaCost += 1f;
                player.manaRegenDelay = (int)player.maxRegenDelay;
            }
            if (interactionDelay > 0)
            {
                interactionDelay--;
            }
        }
        private void ResetEffects_HookBarbs()
        {
            meathook = false;
            if (!hasMeathookNPC)
            {
                leechHook = false;
                meathookNPC = -1;
                meathookUI = !player.miscEquips[4].IsAir && player.miscEquips[4].type == ModContent.ItemType<VampireHook>();
            }
            hasMeathookNPCOld = hasMeathookNPC;
            hasMeathookNPC = false;
            hookDamage = 0;
            if (hookDebuffs == null)
                hookDebuffs = new List<BuffData>();
            else
                hookDebuffs.Clear();
        }
        private void ResetEffects_Temperature()
        {
            if (temperature != 0)
            {
                sbyte minTemp = -100;
                sbyte maxTemp = 100;
                if (coldAmulet)
                {
                    minTemp = -60;
                }
                if (hotAmulet || player.wet)
                {
                    maxTemp = 60;
                }
                if (temperature < minTemp)
                {
                    temperature = minTemp;
                }
                else if (temperature > maxTemp)
                {
                    temperature = maxTemp;
                }
                if (temperatureRegen == 0)
                {
                    temperatureRegen = TEMPERATURE_REGEN_NORMAL;
                    bool resistCold = player.resistCold || coldAmulet;
                    bool resistHeat = (player.wet && !player.lavaWet) || hotAmulet;
                    if ((resistCold && temperature < 0) || (resistHeat && temperature > 0))
                    {
                        temperatureRegen = 8;
                    }
                    if (temperature < 0)
                    {
                        temperature++;
                    }
                    else
                    {
                        if (resistHeat)
                        {
                            temperature--;
                        }
                        temperature--;
                        if (temperature < 0)
                        {
                            temperature = 0;
                        }
                    }
                }
                else
                {
                    temperatureRegen--;
                }
            }
            else
            {
                temperatureRegen = TEMPERATURE_REGEN_ON_HIT;
            }
            if (GaleStreams.EventActive(player))
            {
                if (temperature < -60)
                {
                    player.AddBuff(ModContent.BuffType<Cold60>(), 4);
                }
                else if (temperature < -40)
                {
                    player.AddBuff(ModContent.BuffType<Cold40>(), 4);
                }
                else if (temperature < -20)
                {
                    player.AddBuff(ModContent.BuffType<Cold20>(), 4);
                }
                else if (temperature > 60)
                {
                    player.AddBuff(ModContent.BuffType<Hot60>(), 4);
                }
                else if (temperature > 40)
                {
                    player.AddBuff(ModContent.BuffType<Hot40>(), 4);
                }
                else if (temperature > 20)
                {
                    player.AddBuff(ModContent.BuffType<Hot20>(), 4);
                }
            }
        }
        private void ResetEffects_DashAvailable()
        {
            dashEquipped = !(player.setSolar || player.mount.Active);
            if (dashEquipped)
            {
                int slots = player.AmtAccSlots();
                for (int i = 3; i < slots; i++)
                {
                    if (AQItem.Sets.DashEquip.Contains(player.armor[i].type))
                    {
                        dashEquipped = false;
                        break;
                    }
                }
            }
        }
        private void ResetEffects_Healing()
        {
            if (extraHP > 60) // to cap life max buffs at 60
            {
                extraHP = 60;
            }
            player.statLifeMax2 += extraHP;
            extraHP = 0;
            if (healEffectValueForSyncingTheThingOnTheServer != 0)
            {
                player.HealEffect(healEffectValueForSyncingTheThingOnTheServer, broadcast: true);
            }
            healEffectValueForSyncingTheThingOnTheServer = 0;
        }
        private void ResetEffects_Debuffs()
        {
            blueFire = false;
            shimmering = false;
            pickBreak = false;
        }
        public override void ResetEffects()
        {
            ResetEffects_Cooldowns();

            helmetFlowerCrown = false;
            setLightbulb = false;
            arachnotronArms = false;
            setArachnotron = false;
            setArachnotronCooldown = false;

            blueSpheres = false;
            discountPercentage = 1f;
            hyperCrystal = false;
            monoxiderBird = false;
            moonShoes = false;
            copperSeal = false;
            silverSeal = false;
            goldSeal = false;
            extraFlightTime = 0;
            dreadsoul = false;
            breadsoul = false;
            omori = false;
            omoriEffect = false;
            omegaStarite = false;
            lootIterations = 0;
            featherflightAmulet = false;
            voodooAmulet = false;
            extractinatorCounter = false;
            altEvilDrops = false;
            starite = false;
            striderPalmsOld = striderPalms;
            striderPalms = false;
            shieldLife = 0;

            crimsonHands = false;
            chomper = false;
            piranhaPlant = false;
            trapperImp = false;

            headMinionCarryXOld = headMinionCarryX;
            headMinionCarryYOld = headMinionCarryY;
            headMinionCarryX = 0;
            headMinionCarryY = 0;
            monoxiderCarry = 0;

            heartMoth = false;
            anglerFish = false;
            dwarfStarite = false;

            snowsaw = false;
            snowsawLeader = -1;

            grabReach = 1f;
            mothmanMask = false;
            crabAx = false;
            fidgetSpinner = false;
            canToggleCosmicanon = false;
            ignoreMoons = false;
            antiGravityItems = false;
            shockCollar = false;
            healBeforeDeath = false;
            glowString = false;
            pearlAmulet = false;
            holyDamage = 1f;
            unholyDamage = 1f;
            bloodthirst = false;
            shade = false;
            spreadDebuffs = false;
            ResetEffects_Healing();
            ResetEffects_Debuffs();
            ResetEffects_Temperature();
            hotAmulet = false;
            coldAmulet = false;
            if (mothmanExplosionDelay > 0)
                mothmanExplosionDelay--;
            if (bloodthirstDelay > 0)
                bloodthirstDelay--;
            helmetDartTrap = false;
            if (thunderbirdLightningTimer > 0)
                thunderbirdLightningTimer--;
            ResetEffects_HookBarbs();
            ResetEffects_DashAvailable();

            if (player.selectedItem != lastSelectedItem)
            {
                itemCombo = 0;
                if (itemSwitch < 60)
                    itemSwitch = 60;
            }
            lastSelectedItem = player.selectedItem;

            runSpeedBoost = 1f;
            wingSpeedBoost = 1f;
        }

        private void ProcessTriggers_ToggleCosmicanon()
        {
            IgnoreIgnoreMoons = !IgnoreIgnoreMoons;
            if (IgnoreIgnoreMoons)
            {
                Main.NewText(Language.GetTextValue("Mods.AQMod.ToggleCosmicanon.False"), new Color(230, 230, 255, 255));
            }
            else
            {
                Main.NewText(Language.GetTextValue("Mods.AQMod.ToggleCosmicanon.True"), new Color(230, 230, 255, 255));
            }
        }
        private void ProcessTriggers_ArmorSetBonus()
        {
            if (setArachnotron && !setArachnotronCooldown)
            {
                player.AddBuff(ModContent.BuffType<PrimeTime>(), 600);
                Main.PlaySound(SoundID.Item22, player.Center);
            }
        }
        private void ProcessTriggers_MeathookSnapTarget()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].aiStyle == 7 && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].active = false;
                    Main.projectile[i].netUpdate = true;
                }
            }
            if (player.miscEquips[4].modItem is VampireHook)
            {
                leechHook = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                Main.PlaySound(SoundID.Tink, Main.npc[meathookTarget].Center, 1);
            }
            Projectile.NewProjectile(Main.npc[meathookTarget].Center, Vector2.Zero, player.miscEquips[4].shoot, player.miscEquips[4].damage, player.miscEquips[4].knockBack, player.whoAmI);
            meathookNPC = meathookTarget;
            hasMeathookNPC = true;
            meathookUI = false;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (canToggleCosmicanon && Keybinds.CosmicanonToggle.JustPressed)
            {
                ProcessTriggers_ToggleCosmicanon();
            }
            if (Keybinds.ArmorSetBonus.JustPressed)
            {
                ProcessTriggers_ArmorSetBonus();
            }
            if (!hasMeathookNPC && meathookTarget != -1 && Main.npc[meathookTarget].active && triggersSet.Grapple)
            {
                ProcessTriggers_MeathookSnapTarget();
                triggersSet.Grapple = false;
            }
            meathookTarget = -1;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (AQPlayer)clientClone;
            clone.celesteTorusX = celesteTorusX;
            clone.celesteTorusY = celesteTorusY;
            clone.celesteTorusZ = celesteTorusZ;
            clone.breadsoul = breadsoul;
            clone.dreadsoul = dreadsoul;
            clone.helmetDartTrap = helmetDartTrap;
            clone.dartHeadType = dartHeadType;
            clone.arachnotronArms = arachnotronArms;
            clone.blueSpheres = blueSpheres;
        }

        public override void PreUpdateBuffs()
        {
            int b = player.FindBuffIndex(ModContent.BuffType<PrimeTime>());
            if (b != -1 && player.buffTime[b] < 2)
            {
                player.AddBuff(ModContent.BuffType<ArachnotronCooldown>(), 7200);
                setArachnotronCooldown = true;
            }
        }

        private void ItemCheck_FidgetSpinner(Item item)
        {
            forceAutoswing = false;
            if (fidgetSpinner && player.selectedItem < Main.maxInventory && (Main.mouseItem == null || Main.mouseItem.type <= ItemID.None))
            {
                if (AQItem.CheckAutoswingable(player, item, ignoreChanneled: false))
                {
                    forceAutoswing = true;
                    item.autoReuse = true;
                }
            }
        }
        private void ItemCheck_CrabAx(Item item)
        {
            bool canMine = CanReach(player, item);
            if (player.noBuilding)
                canMine = false;
            if (Main.mouseRight || !canMine)
                crabAx = false;
            else if (!crabAx)
                crabAx = item.type == ModContent.ItemType<Crabax>();
            if (crabAx && (item.axe > 0))
            {
                if (Main.tile[Player.tileTargetX, Player.tileTargetY].active() && player.toolTime <= 1 && player.itemAnimation > 0 && player.controlUseItem)
                {
                    var rectangle = new Rectangle((int)(player.position.X + player.width / 2) / 16, (int)(player.position.Y + player.height / 2) / 16, 30, 30);
                    rectangle.X -= rectangle.Width / 2;
                    rectangle.Y -= rectangle.Height / 2;
                    int hitCount = 0;
                    const int HitCountMax = 8;
                    if (rectangle.X > 10 && rectangle.X < Main.maxTilesX - 10 && rectangle.Y > 10 && rectangle.Y < Main.maxTilesY - 10)
                    {
                        for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
                        {
                            for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                            {
                                if (Main.tile[i, j] == null)
                                {
                                    Main.tile[i, j] = new Tile();
                                    continue;
                                }
                                if (Main.tile[i, j].active() && Main.tileAxe[Main.tile[i, j].type])
                                {
                                    int tileID = player.hitTile.HitObject(i, j, 1);
                                    int tileDamage = 0;
                                    if (Main.tile[i, j].type == 80)
                                    {
                                        tileDamage += item.axe * 3;
                                    }
                                    else
                                    {
                                        TileLoader.MineDamage(item.axe, ref tileDamage);
                                    }
                                    if (Main.tile[i, j].type == TileID.Trees)
                                    {
                                        int treeStumpX = i;
                                        int treeStumpY = j;

                                        if (Main.tile[treeStumpX, treeStumpY].frameY >= 198 && Main.tile[treeStumpX, treeStumpY].frameX == 44)
                                        {
                                            treeStumpX++;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].frameX == 66 && Main.tile[treeStumpX, treeStumpY].frameY <= 44)
                                        {
                                            treeStumpX++;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].frameX == 44 && Main.tile[treeStumpX, treeStumpY].frameY >= 132 && Main.tile[treeStumpX, treeStumpY].frameY <= 176)
                                        {
                                            treeStumpX++;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].frameY >= 198 && Main.tile[treeStumpX, treeStumpY].frameX == 66)
                                        {
                                            treeStumpX--;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].frameX == 88 && Main.tile[treeStumpX, treeStumpY].frameY >= 66 && Main.tile[treeStumpX, treeStumpY].frameY <= 110)
                                        {
                                            treeStumpX--;
                                        }
                                        if (Main.tile[treeStumpX, treeStumpY].frameX == 22 && Main.tile[treeStumpX, treeStumpY].frameY >= 132 && Main.tile[treeStumpX, treeStumpY].frameY <= 176)
                                        {
                                            treeStumpX--;
                                        }

                                        i = treeStumpX + 2; // skips the current index and the next one, since this entire tree has been completed
                                        j = rectangle.Y;

                                        for (; Main.tile[treeStumpX, treeStumpY].active() && Main.tile[treeStumpX, treeStumpY].type == TileID.Trees && Main.tile[treeStumpX, treeStumpY + 1].type == TileID.Trees; treeStumpY++)
                                        {
                                        }

                                        if (Player.tileTargetX == treeStumpX && Player.tileTargetY == treeStumpY)
                                        {
                                            break;
                                        }

                                        AchievementsHelper.CurrentlyMining = true;
                                        if (!WorldGen.CanKillTile(treeStumpX, treeStumpY))
                                        {
                                            tileDamage = 0;
                                        }
                                        tileID = player.hitTile.HitObject(treeStumpX, treeStumpY, 1);
                                        if (player.hitTile.AddDamage(tileID, tileDamage) >= 100)
                                        {
                                            player.hitTile.Clear(tileID);
                                            WorldGen.KillTile(treeStumpX, treeStumpY);
                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                            {
                                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY);
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(i, j, fail: true);
                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                            {
                                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY, 1f);
                                            }
                                        }
                                        if (tileDamage != 0)
                                        {
                                            player.hitTile.Prune();
                                            hitCount++;
                                            if (hitCount > HitCountMax)
                                            {
                                                break;
                                            }
                                        }
                                        AchievementsHelper.CurrentlyMining = false;
                                        continue;
                                    }
                                    else if (Main.tile[i, j].type == TileID.PalmTree)
                                    {
                                        int treeStumpX = i;
                                        int treeStumpY = j;

                                        for (; Main.tile[treeStumpX, treeStumpY].active() && Main.tile[treeStumpX, treeStumpY].type == TileID.PalmTree && Main.tile[treeStumpX, treeStumpY + 1].type == TileID.PalmTree; treeStumpY++)
                                        {
                                        }

                                        i = treeStumpX + 2; // skips the current index and the next one, since this entire tree has been completed
                                        j = rectangle.Y;

                                        if (Player.tileTargetX == treeStumpX && Player.tileTargetY == treeStumpY)
                                        {
                                            break;
                                        }

                                        AchievementsHelper.CurrentlyMining = true;
                                        if (!WorldGen.CanKillTile(treeStumpX, treeStumpY))
                                        {
                                            tileDamage = 0;
                                        }
                                        tileID = player.hitTile.HitObject(treeStumpX, treeStumpY, 1);
                                        if (player.hitTile.AddDamage(tileID, tileDamage) >= 100)
                                        {
                                            player.hitTile.Clear(tileID);
                                            WorldGen.KillTile(treeStumpX, treeStumpY);
                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                            {
                                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY);
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(i, j, fail: true);
                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                            {
                                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY, 1f);
                                            }
                                        }
                                        if (tileDamage != 0)
                                        {
                                            player.hitTile.Prune();
                                            hitCount++;
                                            if (hitCount > HitCountMax)
                                            {
                                                break;
                                            }
                                        }
                                        AchievementsHelper.CurrentlyMining = false;
                                        continue;
                                    }
                                    else
                                    {
                                        AchievementsHelper.CurrentlyMining = true;
                                        if (!WorldGen.CanKillTile(i, j))
                                        {
                                            tileDamage = 0;
                                        }
                                        if (player.hitTile.AddDamage(tileID, tileDamage) >= 100)
                                        {
                                            player.hitTile.Clear(tileID);
                                            WorldGen.KillTile(i, j);
                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                            {
                                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, i, j);
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(i, j, fail: true);
                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                            {
                                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, i, j, 1f);
                                            }
                                        }
                                        if (tileDamage != 0)
                                        {
                                            player.hitTile.Prune();
                                            hitCount++;
                                            if (hitCount > HitCountMax)
                                            {
                                                break;
                                            }
                                        }
                                        AchievementsHelper.CurrentlyMining = false;
                                    }
                                }
                            }
                            if (hitCount > HitCountMax)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        public override bool PreItemCheck()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var item = player.inventory[player.selectedItem];
                ItemCheck_FidgetSpinner(item);
                ItemCheck_CrabAx(item);
            }
            return true;
        }

        public override void PostItemCheck()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (forceAutoswing)
                {
                    player.inventory[player.selectedItem].autoReuse = false;
                    forceAutoswing = false;
                }
            }
        }

        public override float UseTimeMultiplier(Item item)
        {
            if (forceAutoswing && item.damage > 0 && !item.channel)
            {
                if (item.useTime <= 10)
                {
                    return 0.3f;
                }
                if (item.useTime < 32)
                {
                    return 0.6f;
                }
            }
            return 1f;
        }

        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (ammoRenewal && type == ProjectileID.Bullet && Main.rand.NextBool(8))
                type = ProjectileID.ChlorophyteBullet;
            return true;
        }

        private bool PreventDeath_BloodPlasma()
        {
            if (healBeforeDeath && player.potionDelay <= 0)
            {
                if (player.statLife < 0)
                {
                    player.statLife = 1;
                }
                player.QuickHeal();
                return true;
            }
            return false;
        }
        private bool PreventDeath_Omori()
        {
            if (omori && omoriDeathTimer <= 0)
            {
                Main.PlaySound(SoundID.Item60, player.position);
                player.statLife = 1;
                player.immune = true;
                player.immuneTime = 120;
                omoriDeathTimer = 18000;
                return true;
            }
            return false;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            return !PreventDeath_BloodPlasma() && !PreventDeath_Omori();
        }

        public override void PostUpdateBuffs()
        {
            monoxiderCarry = 0;
            var monoxider = ModContent.ProjectileType<MonoxiderMinion>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == monoxider && p.ai[0] > 0f)
                    monoxiderCarry++;
            }
        }

        private void UpdateBanks()
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (player.bank.item[i].type > Main.maxItemTypes && player.bank.item[i].modItem is IUpdatePiggybank update)
                    update.UpdatePiggyBank(player, i);
                if (player.bank2.item[i].type > Main.maxItemTypes && player.bank2.item[i].modItem is IUpdatePlayerSafe update2)
                    update2.UpdatePlayerSafe(player, i);
            }
        }
        private void UpdateCelesteTorus()
        {
            if (blueSpheres)
            {
                float playerPercent = player.statLife / (float)player.statLifeMax2;
                celesteTorusMaxRadius = GetCelesteTorusMaxRadius(playerPercent);
                celesteTorusRadius = MathHelper.Lerp(celesteTorusRadius, celesteTorusMaxRadius, 0.1f);
                celesteTorusDamage = GetCelesteTorusDamage();
                celesteTorusKnockback = GetCelesteTorusKnockback();

                celesteTorusScale = 1f + celesteTorusRadius * 0.006f + celesteTorusDamage * 0.009f + celesteTorusKnockback * 0.0015f;

                var type = ModContent.ProjectileType<CelesteTorusCollider>();
                if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[type] <= 0)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, type, celesteTorusDamage, celesteTorusKnockback, player.whoAmI);
                }
                else
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<CelesteTorusCollider>())
                        {
                            Main.projectile[i].damage = celesteTorusDamage;
                            Main.projectile[i].knockBack = celesteTorusKnockback;
                            break;
                        }
                    }
                }
                var center = player.Center;
                bool danger = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].IsntFriendly() && Vector2.Distance(Main.npc[i].Center, center) < 2000f)
                    {
                        danger = true;
                        break;
                    }
                }

                if (danger)
                {
                    celesteTorusSpeed = 0.04f + (1f - playerPercent) * 0.0314f;
                    celesteTorusX = celesteTorusX.AngleLerp(0f, 0.01f);
                    celesteTorusY = celesteTorusY.AngleLerp(0f, 0.0075f);
                    celesteTorusZ += celesteTorusSpeed;
                }
                else
                {
                    celesteTorusSpeed = 0.0314f;
                    celesteTorusX += 0.0157f;
                    celesteTorusY += 0.01f;
                    celesteTorusZ += celesteTorusSpeed;
                }
            }
            else
            {
                celesteTorusDamage = 0;
                celesteTorusKnockback = 0f;
                celesteTorusMaxRadius = 0;
                celesteTorusRadius = 0f;
                celesteTorusScale = 1f;
                celesteTorusSpeed = 0f;
                celesteTorusX = 0f;
                celesteTorusY = 0f;
                celesteTorusZ = 0f;
            }
        }
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            UpdateBanks();
            UpdateCelesteTorus();
            if (player.wingsLogic > 0)
                player.wingTimeMax += extraFlightTime;
        }

        private void UpdatePassiveSummonHat_CheckTimer()
        {
            if (passiveSummonTimer == -1)
            {
                passiveSummonTimer = passiveSummonDelay;
            }
        }
        private void UpdatePassiveSummonHat()
        {
            if (helmetDartTrap)
            {
                UpdatePassiveSummonHat_CheckTimer();
                if (player.velocity.Y == 0f)
                    passiveSummonTimer--;
                if (passiveSummonTimer <= 0)
                {
                    passiveSummonTimer = passiveSummonDelay;
                    int damage = player.GetWeaponDamage(player.armor[0]);
                    var spawnPosition = player.gravDir == -1
                        ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height)
                        : player.position + new Vector2(player.width / 2f + 8f * player.direction, 0f);
                    int p = Projectile.NewProjectile(spawnPosition, new Vector2(10f, 0f) * player.direction, dartHeadType, damage, player.armor[0].knockBack * player.minionKB, player.whoAmI);
                    Main.projectile[p].hostile = false;
                    Main.projectile[p].friendly = true;
                    Main.projectile[p].trap = false;
                }
            }
            else if (helmetFlowerCrown)
            {
                UpdatePassiveSummonHat_CheckTimer();
                passiveSummonTimer--;
                bool spawn;
                if (passiveSummonTimer < 0)
                {
                    passiveSummonTimer = passiveSummonDelay;
                    spawn = true;
                }
                else
                {
                    int chance = Math.Max(passiveSummonDelay - (int)(player.velocity.Length() * 4f), 10);
                    spawn = Main.rand.NextBool(chance);
                }
                if (spawn)
                {
                    int damage = player.GetWeaponDamage(player.armor[0]);
                    var spawnPosition = player.gravDir == -1
                           ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height - 10)
                           : player.position + new Vector2(player.width / 2f + 8f * player.direction, 10f);
                    int w = Math.Max(player.width / 2 - 8, 4);
                    spawnPosition.X += Main.rand.Next(-w, w);
                    int p = Projectile.NewProjectile(spawnPosition, new Vector2(Main.windSpeed * 2f + player.velocity.X, Main.rand.NextFloat(-0.75f, 0.25f) + player.velocity.Y), ModContent.ProjectileType<FlowerCrownProj>(), damage, player.armor[0].knockBack * player.minionKB, player.whoAmI);
                    Main.projectile[p].ai[1] += Main.rand.Next(-120, 10);
                    Main.projectile[p].timeLeft += Main.rand.Next(-60, 60);
                    Main.projectile[p].scale += Main.rand.NextFloat(-0.1f, 0.05f);
                    Main.projectile[p].frame = Main.rand.Next(3);
                    Main.projectile[p].rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                }
            }
            else
            {
                passiveSummonTimer = -1;
            }
        }
        private void UpdateArachnotronPets()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int type = ModContent.ProjectileType<ArachnotronLegs>();
                if (player.ownedProjectileCounts[type] <= 0)
                {
                    int p = Projectile.NewProjectile(player.Center, Vector2.Zero, type, 33, 1f, player.whoAmI);
                    Main.projectile[p].netUpdate = true;
                }
            }
        }
        private void UpdateOmoriPets()
        {
            if (omoriDeathTimer > 0)
            {
                omoriDeathTimer--;
                if (omoriDeathTimer == 0 && Main.myPlayer == player.whoAmI)
                    Main.PlaySound(SoundID.MaxMana, (int)player.position.X, (int)player.position.Y, 1, 0.85f, -6f);
            }
            int type = ModContent.ProjectileType<FriendMinion>();
            if (player.ownedProjectileCounts[type] < 3)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                        Main.projectile[i].Kill();
                }
                int damage = GetDamage(player, 66, ModContent.ItemType<RustyKnife>(), summon: true);
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, type, 66, 4f, player.whoAmI, 1f + i);
                }
            }
        }
        public override void PostUpdateEquips()
        {
            if (leechHook && Main.myPlayer == player.whoAmI && Main.GameUpdateCount % 50 == 0)
            {
                HealPlayer(player, 2, broadcast: true, merge: true, player.GrapplingHook(), healingItemQuickHeal: false);
            }
            UpdatePassiveSummonHat();
            if (arachnotronArms)
            {
                UpdateArachnotronPets();
            }
            if (omori && Main.myPlayer == player.whoAmI)
            {
                player.AddBuff(ModContent.BuffType<Omori>(), omoriDeathTimer);
                UpdateOmoriPets();
            }
            else
            {
                if (omoriDeathTimer <= 0)
                    omoriDeathTimer = 1;
                if (omoriDeathTimer > 10)
                {
                    player.ClearBuff(ModContent.BuffType<Omori>());
                }
            }
            if (antiGravityItems)
            {
                EquivalenceMachineItemManager.AntiGravityNearbyItems(player.Center, 2f * grabReach, 120, player);
            }
            player.accRunSpeed *= runSpeedBoost;
            player.moveSpeed *= runSpeedBoost;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (setLightbulb && proj.trap)
            {
                damage = (int)(damage * 0.7f);
            }
            if (reducedSiltDamage && AQProjectile.Sets.DamageReductionExtractor.Contains(proj.type))
            {
                damage /= 4;
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            var aQProjectile = proj.GetGlobalProjectile<AQProjectile>();
            if (aQProjectile.temperature != 0)
            {
                InflictTemperature(aQProjectile.temperature);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (mothmanMask && AQNPC.Sets.DealsLessDamageToCata[npc.type])
            {
                damage /= 2;
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            var aQNPC = npc.GetGlobalNPC<AQNPC>();
            if (aQNPC.temperature != 0)
            {
                InflictTemperature(aQNPC.temperature);
            }
        }

        private void HitDamage(NPC target, ref int damage)
        {
            if (holyDamage != 1f && AQNPC.Sets.Hallowed.Contains(target.type))
            {
                damage = (int)(damage * 1.1f);
            }
            if (unholyDamage != 1f && AQNPC.Sets.Unholy.Contains(target.type))
            {
                damage = (int)(damage * unholyDamage);
            }
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            HitDamage(target, ref damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            HitDamage(target, ref damage);
        }

        private bool IsTrueMeleeProjectile(Projectile projectile)
        {
            return projectile.melee && projectile.whoAmI == player.heldProj && projectile.aiStyle != 99;
        }
        private void TrueMeleeHit_HyperCrystal(NPC target, int damage, float knockback, Vector2 playerCenter, Vector2 targetCenter, bool criticalHit)
        {
            target.AddBuff(ModContent.BuffType<Sparkling>(), 120);
            if (criticalHit)
            {
                if (target.SpawnedFromStatue || target.type == NPCID.TargetDummy || CanBossChannel(target))
                    return;
                int boss = -1;
                float closestDist = 1200f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && CanBossChannel(npc))
                    {
                        float dist = (npc.Center - playerCenter).Length();
                        if (dist < closestDist)
                        {
                            boss = i;
                            closestDist = dist;
                        }
                    }
                }
                if (boss != -1)
                {
                    int dmg = damage > target.lifeMax ? target.lifeMax : damage;
                    var normal = Vector2.Normalize(Main.npc[boss].Center - targetCenter);
                    int size = 4;
                    var type = ModContent.DustType<MonoDust>();
                    Vector2 position = target.Center - new Vector2(size / 2);
                    int length = (int)(Main.npc[boss].Center - targetCenter).Length();
                    int dustLength = length / size;
                    const float offset = MathHelper.TwoPi / 3f;
                    for (int i = 0; i < dustLength; i++)
                    {
                        Vector2 pos = position + normal * (i * size);
                        for (int j = 0; j < 6; j++)
                        {
                            int d = Dust.NewDust(pos, size, size, type);
                            float positionLength = Main.dust[d].position.Length() / 32f;
                            Main.dust[d].color = new Color(
                                (float)Math.Sin(positionLength) + 1f,
                                (float)Math.Sin(positionLength + offset) + 1f,
                                (float)Math.Sin(positionLength + offset * 2f) + 1f,
                                0.5f);
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 normal2 = new Vector2(1f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                        for (int j = 0; j < 4; j++)
                        {

                            float positionLength1 = (targetCenter + normal2 * (j * 8f)).Length() / 32f;
                            var color = new Color(
                                (float)Math.Sin(positionLength1) + 1f,
                                (float)Math.Sin(positionLength1 + offset) + 1f,
                                (float)Math.Sin(positionLength1 + offset * 2f) + 1f,
                                0.5f);
                            int d = Dust.NewDust(targetCenter, 1, 1, type, default, default, default, color);
                            Main.dust[d].velocity = normal2 * (j * 3.5f);
                        }
                    }
                    Projectile.NewProjectile(Main.npc[boss].Center, Vector2.Zero, ModContent.ProjectileType<HyperCrystalExplosion>(), dmg * 2, knockback * 2, player.whoAmI);
                }
            }
        }
        private void OnHitNPC_SpreadDebuffs(NPC target, Vector2 targetCenter)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].Distance(targetCenter) < 200f)
                {
                    SpreadDebuffs(target, Main.npc[i]);
                }
            }
        }
        private void OnHitNPC_VoodooAmulet(NPC target, Vector2 targetCenter, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != target.whoAmI && Main.npc[i].active)
                {
                    float distance = Main.npc[i].Distance(targetCenter);
                    if (distance < 300f && AQNPC.AreTheSameNPC(target.type, Main.npc[i].type))
                    {
                        player.ApplyDamageToNPC(Main.npc[i], (int)(damage * (1f - distance / 300f)), knockback, Main.npc[i].position.X + Main.npc[i].width / 2f < player.position.X + player.width / 2f ? -1 : 1, crit);
                    }
                }
            }
        }
        private void OnHitNPCWithAnything(NPC target, Vector2 targetCenter, int damage, float knockback, bool crit)
        {
            if (mothmanMask && mothmanExplosionDelay == 0 && player.statLife >= player.statLifeMax2 && crit && target.type != NPCID.TargetDummy)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 480);
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.PlaySound(SoundID.Item74, targetCenter);
                    int amount = (int)(25 * AQConfigClient.Instance.EffectIntensity);
                    if (AQConfigClient.Instance.EffectQuality < 1f)
                    {
                        amount = (int)(amount * AQConfigClient.Instance.EffectQuality);
                    }
                    var pos = target.position - new Vector2(2f, 2f);
                    var rect = new Rectangle((int)pos.X, (int)pos.Y, target.width + 4, target.height + 4);
                    for (int i = 0; i < amount; i++)
                    {
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        var velocity = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-10f, 2f).Abs());
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f) * 0.2f, 1.5f));
                    }
                    amount = (int)(120 * AQConfigClient.Instance.EffectIntensity);
                    if (AQConfigClient.Instance.EffectQuality < 1f)
                    {
                        amount = (int)(amount * AQConfigClient.Instance.EffectQuality);
                    }
                    if (AQConfigClient.Instance.Screenshakes)
                    {
                        FX.AddShake(AQGraphics.MultIntensity(8), 24f, 16f);
                    }
                    mothmanExplosionDelay = 60;
                    int p = Projectile.NewProjectile(targetCenter, Vector2.Normalize(targetCenter - player.Center), ModContent.ProjectileType<MothmanCritExplosion>(), damage * 2, knockback * 1.5f, player.whoAmI, 0f, target.whoAmI);
                    var size = Main.projectile[p].Size;
                    float radius = size.Length() / 5f;
                    for (int i = 0; i < amount; i++)
                    {
                        var offset = new Vector2(Main.rand.NextFloat(radius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                        var normal = Vector2.Normalize(offset);
                        var dustPos = targetCenter + offset;
                        var velocity = normal * Main.rand.NextFloat(6f, 12f);
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f) * 0.2f, 1.5f));
                        if (Main.rand.NextBool(14))
                        {
                            var sparkleClr = new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f);
                            Particle.PostDrawPlayers.AddParticle(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr, 1.5f));
                            Particle.PostDrawPlayers.AddParticle(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr * 0.5f, 1f)
                                { rotation = MathHelper.PiOver4 });
                        }
                    }
                }
            }
            if (spreadDebuffs)
            {
                OnHitNPC_SpreadDebuffs(target, targetCenter);
            }
            if (voodooAmulet)
            {
                OnHitNPC_VoodooAmulet(target, targetCenter, damage, knockback, crit);
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targetCenter = target.Center;
            if (item.melee)
            {
                if (hyperCrystal)
                {
                    TrueMeleeHit_HyperCrystal(target, damage, knockback, center, targetCenter, crit);
                }
            }
            OnHitNPCWithAnything(target, targetCenter, damage, knockback, crit);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targetCenter = target.Center;
            if (IsTrueMeleeProjectile(proj))
            {
                if (hyperCrystal)
                {
                    TrueMeleeHit_HyperCrystal(target, damage, knockback, center, targetCenter, crit);
                }
            }
            OnHitNPCWithAnything(target, targetCenter, damage, knockback, crit);
        }

        private void DamageOverTime_ClearNaturalRegeneration()
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;
            player.lifeRegenTime = 0;
        }
        private void DamageOverTime_Shimmering()
        {
            DamageOverTime_ClearNaturalRegeneration();
            player.lifeRegen -= 40;
        }
        private void DamageOverTime_BlueFire()
        {
            DamageOverTime_ClearNaturalRegeneration();
            player.lifeRegen -= 10;
        }
        public override void UpdateBadLifeRegen()
        {
            if (shimmering)
            {
                DamageOverTime_Shimmering();
            }
            if (blueFire)
            {
                DamageOverTime_BlueFire();
            }
        }

        private void AnglerReward_ShowNumFinished()
        {
            if (Main.myPlayer == player.whoAmI && AQConfigClient.Instance.ShowCompletedQuestsCount)
                CombatText.NewText(player.getRect(), Color.Aqua, player.anglerQuestsFinished);
        }
        private void AnglerReward_GiveFishingSeal(List<Item> rewardItems)
        {
            if (player.anglerQuestsFinished == 2)
            {
                rewardItems.Add(AQItem.GetDefault(ModContent.ItemType<CopperSeal>()));
            }
            else if (player.anglerQuestsFinished == 10)
            {
                rewardItems.Add(AQItem.GetDefault(ModContent.ItemType<SilverSeal>()));
            }
            else if (player.anglerQuestsFinished == 20)
            {
                rewardItems.Add(AQItem.GetDefault(ModContent.ItemType<GoldSeal>()));
            }
        }
        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            AnglerReward_ShowNumFinished();
            AnglerReward_GiveFishingSeal(rewardItems);
        }

        public static bool CanBossChannel(NPC npc)
        {
            if (npc.chaseable || npc.dontTakeDamage)
            {
                return false;
            }
            return npc.boss || AQNPC.Sets.BossRelatedEnemy[npc.type];
        }

        private static void SpreadDebuffs(NPC spreader, NPC npc)
        {
            List<Color> poofColors = new List<Color>();
            for (int i = 0; i < spreader.buffType.Length; i++)
            {
                if (spreader.buffType[i] > 0 && !AQBuff.Sets.NoSpread.Contains(spreader.buffType[i]) && !npc.buffImmune[spreader.buffType[i]])
                {
                    int b = npc.FindBuffIndex(spreader.buffType[i]);
                    if (b == -1)
                    {
                        npc.AddBuff(spreader.buffType[i], spreader.buffTime[i] * (Main.rand.NextBool() ? 1 : 2));
                        var color = BuffColorCache.GetColorFromBuffID(spreader.buffType[i]);
                        if (color != Color.Transparent)
                        {
                            poofColors.Add(color);
                        }
                    }
                    else
                    {
                        if (npc.buffTime[b] < spreader.buffTime[i])
                        {
                            npc.buffTime[b] = spreader.buffTime[i] * (Main.rand.NextBool() ? 1 : 2);
                        }
                    }
                }
            }
            if (poofColors.Count > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    DustSpawnPatterns.SpawnDustAnMakeVelocityGoAwayFromOrigin(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(),
                        Main.rand.NextFloat(0.1f, 0.25f), poofColors[Main.rand.Next(poofColors.Count)], Main.rand.NextFloat(0.9f, 1.1f));
                }
                for (int i = 0; i < 12; i++)
                {
                    DustSpawnPatterns.SpawnDustAnMakeVelocityGoAwayFromOrigin(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(),
                        Main.rand.NextFloat(npc.width / 16f, npc.width / 12f), poofColors[Main.rand.Next(poofColors.Count)], Main.rand.NextFloat(0.9f, 1.1f));
                }
                for (int i = 0; i < 3; i++)
                {
                    DustSpawnPatterns.SpawnDustAnMakeVelocityGoAwayFromOrigin(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(),
                        Main.rand.NextFloat(npc.width / 12f, npc.width / 8f), poofColors[Main.rand.Next(poofColors.Count)], Main.rand.NextFloat(0.9f, 1.1f));
                }
            }
        }

        public void InflictTemperature(sbyte newTemperature)
        {
            temperatureRegen = TEMPERATURE_REGEN_ON_HIT;
            if (player.resistCold && newTemperature < 0)
            {
                newTemperature /= 2;
            }
            if (temperature < 0)
            {
                if (newTemperature < 0)
                {
                    if (temperature > newTemperature)
                    {
                        temperature = newTemperature;
                    }
                }
                else
                {
                    temperature /= 2;
                }
            }
            else if (temperature > 0)
            {
                if (newTemperature > 0)
                {
                    if (temperature < newTemperature)
                    {
                        temperature = newTemperature;
                    }
                }
                else
                {
                    temperature /= 2;
                }
            }
            else
            {
                temperature = (sbyte)(newTemperature / 2);
            }
            if (Main.expertMode)
            {
                if (newTemperature < 0)
                {
                    temperature -= 3;
                }
                else
                {
                    temperature += 3;
                }
            }
            else
            {
                if (newTemperature < 0)
                {
                    temperature -= 1;
                }
                else
                {
                    temperature += 1;
                }
            }
        }

        public Vector3 GetCelesteTorusPositionOffset(int i)
        {
            return Vector3.Transform(new Vector3(celesteTorusRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(celesteTorusX, celesteTorusY, celesteTorusZ + MathHelper.TwoPi / 5 * i));
        }

        public int GetCelesteTorusMaxRadius(float playerPercent)
        {
            return Math.Min((int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense), 600);
        }

        public int GetCelesteTorusDamage()
        {
            return 25 + (int)(player.statDefense / 1.5f + player.endurance * 80f);
        }

        public float GetCelesteTorusKnockback()
        {
            return 6.5f + player.velocity.Length() * 0.8f;
        }

        public void SetMinionCarryPos(int x, int y)
        {
            hasMinionCarry = true;
            headMinionCarryX = x;
            headMinionCarryY = y;
        }

        public Vector2 GetHeadCarryPosition()
        {
            int x;
            if (headMinionCarryX != 0)
            {
                x = headMinionCarryX;
            }
            else if (headMinionCarryXOld != 0)
            {
                x = headMinionCarryXOld;
            }
            else
            {
                x = (int)player.position.X + player.width / 2;
            }
            int y;
            if (headMinionCarryY != 0)
            {
                y = headMinionCarryY;
            }
            else if (headMinionCarryYOld != 0)
            {
                y = headMinionCarryYOld;
            }
            else
            {
                y = (int)player.position.Y + player.height / 2;
            }
            return new Vector2(x, y);
        }

        /// <summary>
        /// Item is the item, int is the index.
        /// </summary>
        /// <param name="action"></param>
        public void ArmorAction(Action<Item, int> action)
        {
            for (int i = 0; i < 3; i++)
            {
                action(player.armor[i], i);
            }
        }

        /// <summary>
        /// Item is the item, bool is the hide flag, int is the index.
        /// </summary>
        /// <param name="action"></param>
        public void AccessoryAction(Action<Item, bool, int> action)
        {
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                action(player.armor[i], player.hideVisual[i], i);
            }
        }

        public float ItemCooldownMultiplier(Item item)
        {
            float cooldownMult = 1f;
            if (item != null)
            {
                cooldownMult *= item.GetGlobalItem<AQItem>().cooldownMultiplier;
            }
            return cooldownMult;
        }
        public bool ItemCooldownCheck(ushort cooldown, bool effectedByCooldownStats = true, Item item = null)
        {
            if (itemCooldown > 0)
            {
                return false;
            }
            if (effectedByCooldownStats)
            {
                float cooldownMult = ItemCooldownMultiplier(item);
                if (cooldownMult != 1f)
                {
                    cooldown = (ushort)(int)(cooldown * (1f - (cooldownMult - 1f)));
                }
            }
            itemCooldown = cooldown;
            itemCooldownMax = cooldown;
            return true;
        }

        public float ItemComboMultiplier(Item item = null)
        {
            float comboMult = 1f;
            if (item != null)
            {
                comboMult *= item.GetGlobalItem<AQItem>().comboMultiplier;
            }
            return comboMult;
        }
        public void ItemCombo(ushort combo, bool effectedByComboStats = true, bool doVisuals = true, Item item = null)
        {
            if (effectedByComboStats)
            {
                float comboMult = ItemComboMultiplier(item);
                if (comboMult != 1f)
                {
                    combo = (ushort)(int)(combo * comboMult);
                }
            }
            itemCombo += combo;
        }

        public static void HeadMinionSummonCheck(int player, int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type != type && AQProjectile.Sets.MinionHeadType.Contains(Main.projectile[i].type) && Main.projectile[i].owner == player)
                    Main.projectile[i].Kill();
            }
        }

        public static bool HasFoodBuff(int player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Main.player[player].buffTime[i] > 0 && AQBuff.Sets.FoodBuff.Contains(Main.player[player].buffType[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PlayerCrit(int critChance, UnifiedRandom rand)
        {
            if (critChance >= 100)
                return true;
            if (critChance <= 0)
                return false;
            return rand.NextBool(100 - critChance);
        }

        public static bool InVanitySlot(Player player, int type)
        {
            for (int i = MaxDye; i < MaxArmor; i++)
            {
                if (player.armor[i].type == type)
                    return true;
            }
            return false;
        }

        public static bool CanReach(Player player)
            => CanReach(player, Player.tileTargetX, Player.tileTargetY);
        public static bool CanReach(Player player, int x, int y)
            => !player.noBuilding && player.position.X / 16f - Player.tileRangeX - player.blockRange <= x
            && (player.position.X + player.width) / 16f + Player.tileRangeX - 1f + player.blockRange >= x
            && player.position.Y / 16f - Player.tileRangeY - player.blockRange <= y
            && (player.position.Y + player.height) / 16f + Player.tileRangeY + 2f + player.blockRange >= y;
        public static bool CanReach(Player player, Item item)
            => CanReach(player, item, Player.tileTargetX, Player.tileTargetY);
        public static bool CanReach(Player player, Item item, int x, int y)
            => player.position.X / 16f - Player.tileRangeX - item.tileBoost <= x
            && (player.position.X + player.width) / 16f + Player.tileRangeX + item.tileBoost - 1f >= x
            && player.position.Y / 16f - Player.tileRangeY - item.tileBoost <= y
            && (player.position.Y + player.height) / 16f + Player.tileRangeY + item.tileBoost - 2f >= y;

        public static bool ConsumeItem_CheckMouseToo(Player player, int type)
        {
            var item = player.ItemInHand();
            if (item != null && item.type == type)
            {
                if (ItemLoader.ConsumeItem(item, player))
                {
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                    }
                    return true;
                }
            }
            return player.ConsumeItem(type);
        }

        public static bool IgnoreMoons()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Main.player[i].GetModPlayer<AQPlayer>().ignoreMoons) // dead players also allow moons to be disabled
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TileImportantItem(Player player)
        {
            if (player.HeldItem == null || player.HeldItem.type <= ItemID.None)
            {
                return false;
            }
            var item = player.HeldItem;
            return item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.createTile > TileID.Dirt || item.createWall > 0 || item.tileWand > 0;
        }

        public static int GetDamage(Player player, int damage, int itemType = -1, bool melee = false, bool ranged = false, bool magic = false, bool summon = false, bool throwing = false, bool forceIgnoreDamageModifiers = false)
        {
            float add = player.allDamage;
            float mult = player.allDamageMult;
            Item item = null;
            if (itemType != -1)
            {
                item = new Item();
                item.SetDefaults(itemType);
            }

            if (melee)
            {
                add += player.meleeDamage - 1f;
                mult *= player.meleeDamageMult;
            }

            if (ranged)
            {
                add += player.rangedDamage - 1f;
                mult *= player.rangedDamageMult;
                if (itemType != -1)
                {
                    if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake)
                    {
                        mult *= player.arrowDamage;
                    }

                    if (item.useAmmo == AmmoID.Arrow && player.archery)
                    {
                        mult *= 1.2f;
                    }

                    if (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.CandyCorn)
                    {
                        mult *= player.bulletDamage;
                    }

                    if (item.useAmmo == AmmoID.Rocket || item.useAmmo == AmmoID.StyngerBolt || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.NailFriendly)
                    {
                        mult *= player.rocketDamage;
                    }
                }
            }

            if (magic)
            {
                add += player.magicDamage - 1f;
                mult *= player.magicDamageMult;
            }

            if (summon)
            {
                add += player.minionDamage - 1f;
                mult *= player.minionDamageMult;
            }

            if (throwing)
            {
                add += player.thrownDamage - 1f;
                mult *= player.thrownDamageMult;
            }

            if (itemType == -1 || forceIgnoreDamageModifiers)
            {
                return GetDamage(damage, add, mult, 0f);
            }

            if (item.modItem?.IgnoreDamageModifiers ?? false)
            {
                return damage;
            }

            float flat = 0f;
            CombinedHooks.ModifyWeaponDamage(player, item, ref add, ref mult, ref flat);
            damage = GetDamage(damage, add, mult, flat);
            CombinedHooks.GetWeaponDamage(player, item, ref damage);
            return Math.Max(0, damage);
        }

        public static int GetDamage(int damage, float add, float mult, float flat)
        {
            return (int)(damage * add * mult + 5E-06f + flat);
        }

        public static void HealPlayer(Player player, int amt, bool broadcast = true, bool merge = true, Item healingItem = null, bool healingItemQuickHeal = false)
        {
            if (healingItem != null)
            {
                ItemLoader.GetHealLife(healingItem, player, quickHeal: healingItemQuickHeal, ref amt);
            }
            player.statLife = Math.Min(player.statLife + amt, player.statLifeMax2);
            if (!merge)
            {
                player.HealEffect(healAmount: amt, broadcast: broadcast);
            }
            else
            {
                if (!broadcast)
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        player.HealEffect(healAmount: amt, broadcast: false);
                    }
                }
                else
                {
                    if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                    {
                        player.GetModPlayer<AQPlayer>().healEffectValueForSyncingTheThingOnTheServer += amt;
                    }
                }
            }
        }

        public static bool InteractionDelay(uint? apply = null)
        {
            var aQPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
            if (aQPlayer.interactionDelay > 0)
            {
                return false;
            }
            aQPlayer.interactionDelay = apply.GetValueOrDefault(0);
            return true;
        }
    }
}